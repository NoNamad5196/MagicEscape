using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object = UnityEngine.Object;
public class WarehouseXriTests
{
    Mouse mouse; Keyboard keyboard;
    InputSettings.EditorInputBehaviorInPlayMode editorInput;
    InputSettings.BackgroundBehavior background;
    DesktopXriDriver Driver => Object.FindFirstObjectByType<DesktopXriDriver>();
    [UnitySetUp] public IEnumerator Load()
    {
        editorInput = InputSystem.settings.editorInputBehaviorInPlayMode; background = InputSystem.settings.backgroundBehavior;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        keyboard = InputSystem.AddDevice<Keyboard>(); mouse = InputSystem.AddDevice<Mouse>();
        SceneManager.sceneLoaded += ForceDesktop;
        yield return SceneManager.LoadSceneAsync("Warehouse"); yield return null;
        SceneManager.sceneLoaded -= ForceDesktop;
        GameManager.Instance.StartGame(); GameManager.Instance.SkipPresentation(); yield return null; yield return null;
        Assert.False(Object.FindFirstObjectByType<RigSelector>().IsVR);
    }
    void ForceDesktop(Scene scene, LoadSceneMode mode) { Object.FindFirstObjectByType<RigSelector>().mode = RigMode.Desktop; }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        SceneManager.sceneLoaded -= ForceDesktop;
        if (Driver != null) Driver.Drop();
        if (mouse != null) InputSystem.RemoveDevice(mouse); if (keyboard != null) InputSystem.RemoveDevice(keyboard);
        InputSystem.settings.editorInputBehaviorInPlayMode = editorInput; InputSystem.settings.backgroundBehavior = background;
        Time.timeScale = 1; yield return null;
    }
    IEnumerator ClickAt(Component target, Vector3 stand)
    {
        ArtTests.View(stand, target.transform.position); yield return null; yield return null; yield return null;
        // Re-aim from the collision-settled camera, not a pre-depenetration teleport pose.
        ArtTests.View(Scenario.Player.transform.position, target.transform.position); yield return null; yield return null;
        Assert.True(Driver.ray.TryGetCurrent3DRaycastHit(out var hit), target.name + " ray hit");
        Assert.True(hit.collider.transform.IsChildOf(target.transform) || hit.collider.transform == target.transform,
            target.name + " occluded by " + hit.collider.name + " at " + hit.point);
        InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(MouseButton.Left)); yield return null; yield return null;
        InputSystem.QueueStateEvent(mouse, new MouseState()); yield return null; yield return null; yield return null;
    }
    IEnumerator Runes()
    {
        var altar = Object.FindFirstObjectByType<RunePuzzle>().transform;
        foreach (var id in RunePuzzleLogic.DefaultAnswer)
        {
            var stone = Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(s => s.Rune == id);
            var radial = stone.transform.position - altar.position; radial.y = 0;
            yield return ClickAt(stone, altar.position + radial.normalized * 1.25f);
        }
        yield return new WaitForSeconds(2.2f);
        Assert.AreEqual(PuzzleState.Solved, GameManager.Instance.Progression.GetState(PuzzleId.Rune));
    }
    IEnumerator Grab(string id, Vector3 stand)
    {
        var pickup = Scenario.Pickup(id); yield return ClickAt(pickup, stand);
        Assert.NotNull(Driver.Held, id + " selected through camera XRRayInteractor"); Assert.AreEqual(id, Driver.Held.ItemId);
        yield return new WaitForSeconds(.25f);
        Assert.Less(Vector3.Distance(Driver.Held.transform.position, Driver.ray.attachTransform.position), .2f, "Force grab holds at camera attach point");
    }
    IEnumerator Brew()
    {
        var socket = Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None).First(s => s.cauldron != null);
        string[] ids = PotionPuzzleLogic.DefaultRecipe;
        Vector3[] stands = { new Vector3(3.15f,0,-1.8f), new Vector3(2.8f,0,-.1f), new Vector3(-2.3f,0,-1.4f) };
        for (int i = 0; i < ids.Length; i++)
        {
            yield return Grab(ids[i], stands[i]);
            yield return ClickAt(socket, new Vector3(2.65f,0,-.5f));
            Assert.IsNull(Driver.Held); Assert.False(InventoryManager.Instance.Contains(ids[i])); Assert.False(Scenario.Pickup(ids[i]).IsAvailable);
        }
        yield return new WaitForSeconds(1);
        Assert.AreEqual(PuzzleState.Solved, GameManager.Instance.Progression.GetState(PuzzleId.Potion));
    }
    IEnumerator Seal()
    {
        yield return Grab(ItemIds.PotionRevelation, new Vector3(2.9f,0,.4f));
        var inscription = Object.FindFirstObjectByType<SealInscription>();
        yield return ClickAt(inscription, new Vector3(2.1f,0,1.0f));
        Assert.AreEqual(SealStage.AwaitingCrystals, Object.FindFirstObjectByType<SealPuzzle>().Stage);
        yield return Grab(ItemIds.CrystalSapphire, new Vector3(-.8f,0,-1.4f));
        var left = Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None).First(s => s.crystalSocket != null && s.crystalSocket.Side == SocketSide.Left);
        yield return ClickAt(left, new Vector3(left.transform.position.x,0,1.0f));
        yield return Grab(ItemIds.CrystalRuby, new Vector3(2.5f,0,.4f));
        var right = Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None).First(s => s.crystalSocket != null && s.crystalSocket.Side == SocketSide.Right);
        yield return ClickAt(right, new Vector3(right.transform.position.x,0,1.0f));
        Assert.AreEqual(SealStage.ReadyToRelease, Object.FindFirstObjectByType<SealPuzzle>().Stage);
    }
    [UnityTest] public IEnumerator DesktopRunePressUsesXri() { yield return Runes(); Assert.AreEqual(2, Object.FindFirstObjectByType<RoomPresentation>().LitSealCount); }
    [UnityTest] public IEnumerator DesktopGrabIngredientsIntoCauldronSocket() { yield return Scenario.Rune(); yield return Brew(); }
    [UnityTest] public IEnumerator DesktopCrystalLeftAndRightSockets() { yield return Scenario.Rune(); yield return Scenario.Potion(); yield return Seal(); }
    [UnityTest] public IEnumerator DesktopFullFlowTurnsOffThreeSealsAndOpensExit()
    {
        var p = Object.FindFirstObjectByType<RoomPresentation>(); var ambient = RenderSettings.ambientLight;
        Assert.AreEqual(3, p.LitSealCount); yield return Runes(); Assert.AreEqual(2, p.LitSealCount);
        yield return Brew(); Assert.AreEqual(1, p.LitSealCount); yield return Seal();
        var core = Object.FindFirstObjectByType<SealCore>(); yield return ClickAt(core, new Vector3(2.1f,0,1.0f));
        yield return new WaitForSeconds(3.8f);
        Assert.AreEqual(0, p.LitSealCount); Assert.True(Object.FindFirstObjectByType<DoorController>().PassageReady);
        Assert.AreEqual(ambient, RenderSettings.ambientLight, "Stage 4 darkening not enabled");
        // W-only collision-tested exit, not a direct completion call.
        ArtTests.View(new Vector3(0,0,1.8f), new Vector3(0,1.6f,5));
        float elapsed = 0;
        while (GameManager.Instance.IsPlaying && elapsed < 4)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W)); yield return null; elapsed += Time.unscaledDeltaTime;
        }
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        Assert.AreEqual(GameFlowState.Cleared, GameManager.Instance.Progression.Flow);
    }
    [UnityTest] public IEnumerator VrSimulatedControllerSharesRuneAndGrabComponents()
    {
        var selector = Object.FindFirstObjectByType<RigSelector>(); selector.mode = RigMode.Desktop; selector.Apply(true);
        var controller = InputSystem.AddDevice<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedController>();
        InputSystem.SetDeviceUsage(controller, UnityEngine.InputSystem.CommonUsages.RightHand);
        var origin = selector.vrRig.transform;
        var driver = Object.FindFirstObjectByType<VrXriDriver>();
        try
        {
            var rune = Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(r => r.Rune == RuneId.Earth);
            Vector3 position = rune.transform.position + new Vector3(0,.8f,-.9f);
            var state = new UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedControllerState {
                isTracked = true, trackingState = 3, devicePosition = origin.InverseTransformPoint(position),
                deviceRotation = Quaternion.Inverse(origin.rotation) * Quaternion.LookRotation(rune.transform.position - position) };
            InputSystem.QueueStateEvent(controller,state); yield return null; yield return null;
            InputSystem.QueueStateEvent(controller,state.WithButton(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.ControllerButton.TriggerButton));
            yield return null; yield return null; yield return null;
            Assert.AreEqual(1,Object.FindFirstObjectByType<RunePuzzle>().Logic.ProgressCount);
            state = state.WithButton(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.ControllerButton.TriggerButton, false);
            InputSystem.QueueStateEvent(controller,state); yield return null; yield return null; yield return null;
            Assert.False(driver.right.hasSelection, "Trigger release ends rune selection");
            var pickup = Scenario.Pickup(ItemIds.Moonpetal);
            position = pickup.transform.position + new Vector3(0,.6f,.6f);
            state.devicePosition = origin.InverseTransformPoint(position);
            state.deviceRotation = Quaternion.Inverse(origin.rotation) * Quaternion.LookRotation(pickup.transform.position - position);
            InputSystem.QueueStateEvent(controller,state); yield return null; yield return null;
            InputSystem.QueueStateEvent(controller,state.WithButton(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.ControllerButton.GripButton));
            yield return null; yield return null; yield return null;
            Assert.True(driver.right.hasSelection);
            Assert.AreSame(pickup.GetComponent<XRGrabInteractable>(),driver.right.interactablesSelected[0]);
            yield return new WaitForSeconds(.25f);
            Assert.Less(Vector3.Distance(pickup.transform.position,driver.right.transform.position),.1f,"VR carries at the hand, not the desktop camera offset");
        }
        finally { InputSystem.RemoveDevice(controller); selector.Apply(false); }
    }
    [UnityTest] public IEnumerator WrongPhysicalIngredientRespawnsEntireAttempt()
    {
        yield return Scenario.Rune();
        var socket = Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None).First(s => s.cauldron != null);
        yield return Grab(ItemIds.Moonpetal,new Vector3(3.15f,0,-1.8f));
        yield return ClickAt(socket,new Vector3(2.65f,0,-.5f));
        yield return Grab(ItemIds.Toadstool,new Vector3(-2.8f,0,2.4f));
        yield return ClickAt(socket,new Vector3(2.65f,0,-.5f));
        Assert.AreEqual(1,socket.cauldron.Logic.FailCount);
        Assert.True(Scenario.Pickup(ItemIds.Moonpetal).IsAvailable);
        Assert.True(Scenario.Pickup(ItemIds.Toadstool).IsAvailable);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.Moonpetal));
    }
    [UnityTest] public IEnumerator SharedPressRetiresUnheldConsumedPotion()
    {
        yield return Scenario.Rune(); yield return Scenario.Potion();
        var potion = Scenario.Pickup(ItemIds.PotionRevelation); Assert.True(potion.IsAvailable);
        Scenario.Select(ItemIds.PotionRevelation);
        yield return ClickAt(Object.FindFirstObjectByType<SealInscription>(),new Vector3(2.1f,0,1.0f));
        Assert.AreEqual(SealStage.AwaitingCrystals,Object.FindFirstObjectByType<SealPuzzle>().Stage);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.PotionRevelation));
        Assert.False(potion.IsAvailable,"Consumed potion cannot be picked up again by either rig");
    }
    [UnityTest] public IEnumerator AutomaticSelectionFallsBackWithoutHmd()
    {
        var selector = Object.FindFirstObjectByType<RigSelector>(); selector.mode = RigMode.Automatic;
        yield return selector.InitializeAndSelect();
        if (!UnityEngine.XR.XRSettings.isDeviceActive) Assert.False(selector.IsVR);
    }
    [UnityTest] public IEnumerator WarehouseArchitectureAndRigSwitch()
    {
        Assert.That(GameObject.Find("Floor").GetComponent<Collider>().bounds.size.x, Is.EqualTo(9).Within(.001f));
        Assert.That(GameObject.Find("Floor").GetComponent<Collider>().bounds.size.z, Is.EqualTo(7).Within(.001f));
        Assert.That(GameObject.Find("Ceiling").transform.position.y, Is.InRange(3,3.1f));
        Assert.AreEqual(6, Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).Length);
        foreach (var item in Object.FindObjectsByType<XriItem>(FindObjectsInactive.Include,FindObjectsSortMode.None)) Assert.AreEqual(1,item.GetComponents<XRGrabInteractable>().Length);
        var selector = Object.FindFirstObjectByType<RigSelector>(); selector.mode = RigMode.Desktop;
        selector.Apply(true); yield return null; Assert.True(selector.IsVR); Assert.True(selector.vrCamera.isActiveAndEnabled); Assert.False(selector.desktopCamera.isActiveAndEnabled);
        selector.Apply(false); yield return null; Assert.False(selector.IsVR); Assert.True(selector.desktopCamera.isActiveAndEnabled);
        ArtTests.View(new Vector3(0,0,-2.25f), new Vector3(0,1.35f,2.5f));
        yield return ArtTests.Capture("warehouse-overview");
        System.IO.File.Copy(System.IO.Path.Combine(Application.dataPath,"../../docs/validation/s10f/warehouse-overview.png"), System.IO.Path.Combine(Application.dataPath,"../../docs/validation/warehouse/overview.png"),true);
        ArtTests.View(new Vector3(-1.8f,0,.7f),new Vector3(-4.08f,1.4f,.8f));
        yield return ArtTests.Capture("warehouse-frames");
        System.IO.File.Copy(System.IO.Path.Combine(Application.dataPath,"../../docs/validation/s10f/warehouse-frames.png"), System.IO.Path.Combine(Application.dataPath,"../../docs/validation/warehouse/frames.png"),true);
        ArtTests.View(new Vector3(-2.15f,0,-1.7f),Scenario.Pickup(ItemIds.CrystalDust).transform.position);
        yield return ArtTests.Capture("warehouse-chest");
        System.IO.File.Copy(System.IO.Path.Combine(Application.dataPath,"../../docs/validation/s10f/warehouse-chest.png"), System.IO.Path.Combine(Application.dataPath,"../../docs/validation/warehouse/chest.png"),true);
    }
}
