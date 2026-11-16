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
        var aim=target.transform.position;
        if(target is XriClearable clear && clear.kind==ClearingKind.Cloth) aim=clear.GetComponentInChildren<Collider>().bounds.center;
        ArtTests.View(stand, aim); yield return null; yield return null; yield return null;
        // Re-aim from the collision-settled camera, not a pre-depenetration teleport pose.
        ArtTests.View(Scenario.Player.transform.position, aim); yield return null; yield return null;
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
        yield return Chest();
        var socket = Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None).First(s => s.cauldron != null);
        string[] ids = PotionPuzzleLogic.DefaultRecipe;
        Vector3[] stands = { new Vector3(3.15f,0,-1.8f), new Vector3(2.8f,0,-.1f), new Vector3(-2.1f,0,-2.0f) };
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

    IEnumerator Chest()
    {
        var lid = GameObject.Find("ChestLidHinge").GetComponent<XriClearable>();
        if (!lid.IsCleared)
        {
            ArtTests.View(new Vector3(-2.1f,0,-2.0f),new Vector3(-3.25f,.61f,-2.3f));
            yield return null; yield return null;
            Driver.Click(); yield return new WaitForSeconds(.7f);
            Assert.True(lid.IsCleared,"PC opens the shared hinged lid");
        }
    }
    IEnumerator Slide(string roman)
    {
        var crate=GameObject.Find("ClearingCrate_"+roman).GetComponent<XriClearable>();
        ArtTests.View(new Vector3(-2.6f,0,crate.transform.position.z),crate.transform.position);
        yield return null; yield return null; yield return null;
        InputSystem.QueueStateEvent(mouse,new MouseState().WithButton(MouseButton.Left));
        yield return null;yield return null;
        Assert.True(crate.isSelected,"PC holds shared XRI slide selection");
        float startY=crate.transform.position.y;
        float elapsed=0;
        while(!crate.IsCleared && elapsed<2.5f)
        {
            InputSystem.QueueStateEvent(mouse,new MouseState().WithButton(MouseButton.Left));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(roman=="I" ? Key.D : Key.A));yield return null;elapsed+=Time.deltaTime;
        }
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.QueueStateEvent(mouse,new MouseState());
        yield return null;yield return null;
        Assert.True(crate.IsCleared,"Hold-click + strafe moves crate off painting: "+crate.Progress+" "+crate.transform.position);
        Assert.AreEqual(startY,crate.transform.position.y,.001f,"Heavy crate never lifts");Assert.False(crate.isSelected);
    }
    IEnumerator CaptureClearing(string name)
    {
        yield return ArtTests.Capture(name);
        System.IO.File.Copy(System.IO.Path.Combine(Application.dataPath,"../../docs/validation/s10f/"+name+".png"),System.IO.Path.Combine(Application.dataPath,"../../docs/validation/warehouse/"+name+".png"),true);
    }
    IEnumerator ClearRoom()
    {
        ArtTests.View(new Vector3(-1.9f,0,.5f),new Vector3(-4.1f,.9f,.6f));yield return CaptureClearing("stage3-frames-before");
        yield return Slide("I");yield return Slide("IV");
        foreach(string roman in new[]{"I","IV"})
        {
            var frame=GameObject.Find("Painting_"+roman).GetComponent<InspectableObject>();
            yield return ClickAt(frame,new Vector3(-2.7f,0,frame.transform.position.z));Assert.True(UIManager.Instance.IsReading,"Cleared frame is actually readable");UIManager.Instance.CloseRead();
            yield return CaptureClearing("stage3-frame-"+roman+"-after");
        }
        ArtTests.View(new Vector3(-1.9f,0,.5f),new Vector3(-4.1f,.9f,.6f));yield return CaptureClearing("stage3-frames-after");
        var cloth=GameObject.Find("ElementTableCloth").GetComponent<XriClearable>();
        // E is a real keyboard alias through PlayerInteraction -> DesktopXriDriver -> XRI.
        ArtTests.View(new Vector3(-2.9f,0,.65f),cloth.GetComponentInChildren<Collider>().bounds.center);
        yield return null; yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E)); yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());
        yield return new WaitForSeconds(.7f);Assert.True(cloth.IsCleared,"E opens the shared cloth interactable");
        var table=GameObject.Find("ElementTable").GetComponent<InspectableObject>();
        yield return ClickAt(table,new Vector3(-2.9f,0,.65f));Assert.True(UIManager.Instance.IsReading);
        yield return CaptureClearing("stage3-element-table-close");UIManager.Instance.CloseRead();
        yield return Chest();
        var books=GameObject.Find("MovableBookStack").GetComponent<XRGrabInteractable>();
        yield return ClickAt(books,new Vector3(-1.8f,0,-1.9f));Assert.True(books.isSelected);
        yield return new WaitForSeconds(.3f);
        Assert.Less(Vector3.Distance(books.transform.position,Driver.ray.attachTransform.position),.08f,"Books carry at the PC camera-front point");
        Driver.Drop();yield return null;yield return null;
        Assert.True(books.GetComponent<XriLooseProp>().IsCleared);Assert.True(GameObject.Find("BookStackClue").GetComponent<ClearingGate>().IsAccessible);
        Assert.AreEqual(PuzzleState.Active,GameManager.Instance.Progression.GetState(PuzzleId.Rune));
        Assert.AreEqual(PuzzleState.Locked,GameManager.Instance.Progression.GetState(PuzzleId.Potion));
        Assert.AreEqual(PuzzleState.Locked,GameManager.Instance.Progression.GetState(PuzzleId.Seal));
    }
    [UnityTest] public IEnumerator DesktopClearingAllFourAndHiddenTargetsCannotBeBypassed()
    {
        foreach(var gate in Object.FindObjectsByType<ClearingGate>(FindObjectsSortMode.None))
        {
            Assert.False(gate.IsAccessible,gate.name);
            foreach(var x in gate.GetComponents<XRBaseInteractable>())Assert.False(x.enabled,gate.name);
            foreach(var c in gate.GetComponents<Collider>())Assert.False(c.enabled,gate.name);
            var inspect=gate.GetComponent<InspectableObject>();if(inspect!=null) { inspect.Interact(Scenario.Player);Assert.False(UIManager.Instance.IsReading); }
        }
        Assert.False(Scenario.Pickup(ItemIds.CrystalDust).IsAvailable);
        Assert.AreEqual(string.Empty,Scenario.Pickup(ItemIds.CrystalDust).GetPrompt(Scenario.Player));
        Scenario.Pickup(ItemIds.CrystalDust).Interact(Scenario.Player);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.CrystalDust));
        yield return ClearRoom();
        foreach(var gate in Object.FindObjectsByType<ClearingGate>(FindObjectsSortMode.None))Assert.True(gate.IsAccessible,gate.name);
    }
    [UnityTest] public IEnumerator CrateRailsCannotBlockPuzzlesExitOrWalls()
    {
        foreach(var c in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None).Where(c=>c.kind==ClearingKind.Slide))
        {
            foreach(var extreme in new[]{Vector3.one*999,Vector3.one*-999,new Vector3(3,100,3)})
            {
                c.MoveWithinRails(extreme);yield return null;
                var b=c.GetComponent<Collider>().bounds;
                Assert.Greater(b.min.x,-4.425f);Assert.Less(b.max.x,-2.5f);Assert.That(b.min.y,Is.EqualTo(0).Within(.01f));
                Assert.Greater(b.min.z,-3.45f);Assert.Less(b.max.z,3.2f);
                foreach(var socket in Object.FindObjectsByType<XriPuzzleSocket>(FindObjectsSortMode.None))Assert.False(b.Contains(socket.transform.position));
                Assert.False(b.Intersects(new Bounds(new Vector3(0,1,3.2f),new Vector3(2.25f,2,1))));
            }
            c.ResetProp(); yield return null; yield return null;
            var frame = GameObject.Find("Painting_"+c.name.Replace("ClearingCrate_", ""));
            Assert.False(frame.GetComponent<ClearingGate>().IsAccessible,"Replacing a crate re-covers its clue");
            Assert.False(frame.GetComponent<Collider>().enabled);
            Assert.False(frame.GetComponent<XRSimpleInteractable>().enabled);
        }
    }
    [UnityTest] public IEnumerator FrameAndStoneUseIdenticalBrassMotifs()
    {
        foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
        {
            var corner=GameObject.Find("ArtB_CornerRune_"+stone.Rune);
            if(corner==null)continue;
            Assert.AreSame(corner.GetComponent<Renderer>().sharedMaterial,stone.transform.Find("ArtB_RuneIcon_"+stone.Rune).GetComponent<Renderer>().sharedMaterial);
        }
        // Actual scene assets in one readable reference view (no replacement drawings).
        var root=new GameObject("GlyphComparisonCapture");int index=0;
        foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).OrderBy(s=>s.Rune))
        {
            var corner=GameObject.Find("ArtB_CornerRune_"+stone.Rune);if(corner==null)continue;
            float x=-.8f+index++*.53f;
            foreach(var src in new[]{corner,stone.transform.Find("ArtB_RuneIcon_"+stone.Rune).gameObject})
            {
                var copy=Object.Instantiate(src,root.transform);copy.transform.position=new Vector3(x,src==corner?1.85f:1.3f,-.5f);copy.transform.rotation=Quaternion.identity;copy.transform.localScale=new Vector3(.42f,.42f,1);
            }
        }
        ArtTests.View(new Vector3(0,0,-2.35f),new Vector3(0,1.6f,-.5f));yield return CaptureClearing("stage3-frame-stone-comparison");Object.Destroy(root);
    }

    [UnityTest] public IEnumerator DesktopRunePressUsesXri() { yield return Runes(); Assert.AreEqual(2, Object.FindFirstObjectByType<RoomPresentation>().LitSealCount); }
    [UnityTest] public IEnumerator DesktopGrabIngredientsIntoCauldronSocket() { yield return Scenario.Rune(); yield return Brew(); }
    [UnityTest] public IEnumerator DesktopCrystalLeftAndRightSockets() { yield return Scenario.Rune(); yield return Chest(); yield return Scenario.Potion(); yield return Seal(); }
    [UnityTest] public IEnumerator DesktopFullFlowTurnsOffThreeSealsAndOpensExit()
    {
        var p = Object.FindFirstObjectByType<RoomPresentation>(); var ambient = RenderSettings.ambientLight;
        Assert.AreEqual(3, p.LitSealCount); yield return ClearRoom(); yield return Runes(); Assert.AreEqual(2, p.LitSealCount);
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
        yield return Grab(ItemIds.Toadstool,new Vector3(-3.1f,0,3.1f));
        yield return ClickAt(socket,new Vector3(2.65f,0,-.5f));
        Assert.AreEqual(1,socket.cauldron.Logic.FailCount);
        Assert.True(Scenario.Pickup(ItemIds.Moonpetal).IsAvailable);
        Assert.True(Scenario.Pickup(ItemIds.Toadstool).IsAvailable);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.Moonpetal));
    }
    [UnityTest] public IEnumerator SharedPressRetiresUnheldConsumedPotion()
    {
        yield return Scenario.Rune(); yield return Chest(); yield return Scenario.Potion();
        var potion = Scenario.Pickup(ItemIds.PotionRevelation); Assert.True(potion.IsAvailable);
        Scenario.Select(ItemIds.PotionRevelation);
        yield return ClickAt(Object.FindFirstObjectByType<SealInscription>(),new Vector3(2.1f,0,1.0f));
        Assert.AreEqual(SealStage.AwaitingCrystals,Object.FindFirstObjectByType<SealPuzzle>().Stage);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.PotionRevelation));
        Assert.False(potion.IsAvailable,"Consumed potion cannot be picked up again by either rig");
    }
    [UnityTest] public IEnumerator VrTrackedHandPullsAndPushesSameHeavyCrate()
    {
        var selector=Object.FindFirstObjectByType<RigSelector>();selector.Apply(true);
        var controller=InputSystem.AddDevice<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedController>();
        InputSystem.SetDeviceUsage(controller,UnityEngine.InputSystem.CommonUsages.RightHand);
        var crate=GameObject.Find("ClearingCrate_I").GetComponent<XriClearable>();
        var origin=selector.vrRig.transform;
        try
        {
            Vector3 position=crate.transform.position+Vector3.right*.8f;
            var state=new UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedControllerState {
                isTracked=true,trackingState=3,devicePosition=origin.InverseTransformPoint(position),
                deviceRotation=Quaternion.Inverse(origin.rotation)*Quaternion.LookRotation(crate.transform.position-position) };
            InputSystem.QueueStateEvent(controller,state);yield return null;yield return null;
            state=state.WithButton(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.ControllerButton.GripButton);
            InputSystem.QueueStateEvent(controller,state);yield return null;yield return null;yield return null;
            Assert.True(crate.isSelected);float y=crate.transform.position.y;
            state.devicePosition+=Vector3.forward*1.1f;InputSystem.QueueStateEvent(controller,state);
            yield return null;yield return null;yield return null;
            Assert.True(crate.IsCleared);Assert.AreEqual(y,crate.transform.position.y,.001f);
            state=state.WithButton(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.ControllerButton.GripButton,false);
            InputSystem.QueueStateEvent(controller,state);yield return null;yield return null;yield return null;
            crate.ResetProp();yield return null;
            position=crate.transform.position+Vector3.right*.47f;
            state.devicePosition=origin.InverseTransformPoint(position);InputSystem.QueueStateEvent(controller,state);
            yield return null;yield return null;yield return null;
            state.devicePosition+=Vector3.forward*.06f;InputSystem.QueueStateEvent(controller,state);
            yield return null;yield return null;yield return null;
            Assert.Greater(crate.transform.position.z,crate.closedPosition.z+.03f,"Unselected tracked palm contact pushes along the rail");
        }
        finally {InputSystem.RemoveDevice(controller);selector.Apply(false);}
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
