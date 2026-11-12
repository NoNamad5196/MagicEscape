using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;
[SetUpFixture]
public class ConsoleAudit
{
    static int errors;
    [OneTimeSetUp] public void Begin(){errors=0; Application.logMessageReceived+=OnLog;}
    static void OnLog(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert) errors++;}
    [OneTimeTearDown] public void End(){Application.logMessageReceived-=OnLog; File.WriteAllText(Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/validation/s10e/console-audit.txt")),"PlayMode Error/Exception/Assert events="+errors+"\nNo expected-error suppression. UTC="+DateTime.UtcNow.ToString("O")); Assert.AreEqual(0,errors);}
}
public class FullInputTests
{
    Keyboard keyboard; Mouse mouse; InputSettings.EditorInputBehaviorInPlayMode previousEditor; InputSettings.BackgroundBehavior previousBackground;
    static readonly Key[] digits={Key.Digit1,Key.Digit2,Key.Digit3,Key.Digit4,Key.Digit5,Key.Digit6,Key.Digit7,Key.Digit8};
    IEnumerator Tap(Key key){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key)); yield return null; yield return null; InputSystem.QueueStateEvent(keyboard,new KeyboardState()); yield return null; yield return null;}
    IEnumerator Slot(string id){int i=InventoryManager.Instance.Model.Items.ToList().IndexOf(id); Assert.GreaterOrEqual(i,0,id); yield return Tap(digits[i]); Assert.AreEqual(id,InventoryManager.Instance.SelectedItemId);}
    void Place(Vector3 position)
    {var p=Scenario.Player; var cc=p.GetComponent<CharacterController>(); cc.enabled=false; p.transform.position=position; cc.enabled=true; Physics.SyncTransforms();}
    void Aim(Vector3 target)
    {
        var p=Scenario.Player.GetComponent<PlayerController>(); Vector3 d=target-p.CameraPivot.position;
        p.transform.rotation=Quaternion.Euler(0,Mathf.Atan2(d.x,d.z)*Mathf.Rad2Deg,0);
        float pitch=-Mathf.Atan2(d.y,new Vector2(d.x,d.z).magnitude)*Mathf.Rad2Deg;
        typeof(PlayerController).GetField("_pitch",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(p,pitch); p.CameraPivot.localRotation=Quaternion.Euler(pitch,0,0);
    }
    IEnumerator Use(Component target,Vector3 stand)
    {
        Place(stand); Aim(target.transform.position); yield return null; yield return null;
        var cam=Camera.main; Assert.True(Physics.Raycast(cam.ViewportPointToRay(new Vector3(.5f,.5f)),out var hit,2.5f,~(1<<8),QueryTriggerInteraction.Ignore),target.name+" reachable");
        Assert.AreSame(target,hit.collider.GetComponentInParent<IInteractable>(),target.name+" focus hit "+hit.collider.name);
        yield return Tap(Key.E);
    }
    IEnumerator Walk(Vector3 target)
    {
        float elapsed=0,simulated=0; int frames=0; var p=Scenario.Player.transform;
        while(Vector2.Distance(new Vector2(p.position.x,p.position.z),new Vector2(target.x,target.z))>.035f && GameManager.Instance.IsPlaying && elapsed<5)
        {
            Vector3 d=target-p.position; p.rotation=Quaternion.Euler(0,Mathf.Atan2(d.x,d.z)*Mathf.Rad2Deg,0); InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W)); yield return null; elapsed+=Time.unscaledDeltaTime; simulated+=Time.deltaTime; frames++;
        }
        InputSystem.QueueStateEvent(keyboard,new KeyboardState()); yield return null; yield return null;
        File.WriteAllText(Path.GetFullPath(Path.Combine(Application.dataPath,"../../docs/validation/s10e/walk-audit.txt")),"frames="+frames+" real="+elapsed+" simulated="+simulated+" position="+p.position+" doorEuler="+GameObject.Find("DoorHinge").transform.localEulerAngles);
        if(GameManager.Instance.IsPlaying) Assert.Less(Vector2.Distance(new Vector2(p.position.x,p.position.z),new Vector2(target.x,target.z)),.2f,"Walk blocked at "+p.position+" toward "+target);
    }
    [UnityTest] public IEnumerator S9_KeyboardRaycastAllPuzzlesWalkExit()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        previousEditor=InputSystem.settings.editorInputBehaviorInPlayMode; previousBackground=InputSystem.settings.backgroundBehavior; InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView; InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus; keyboard=InputSystem.AddDevice<Keyboard>(); mouse=InputSystem.AddDevice<Mouse>();
        GameObject.Find("Button_Start").GetComponent<Button>().onClick.Invoke(); yield return null;
        yield return Tap(Key.Escape); Assert.AreEqual(GameFlowState.Paused,GameManager.Instance.Progression.Flow); yield return Tap(Key.Escape); Assert.True(GameManager.Instance.IsPlaying);
        yield return Tap(Key.Tab); Assert.NotNull(GameObject.Find("ObjectivePanel")); yield return Tap(Key.Tab);
        yield return Use(GameObject.Find("Painting_I").GetComponent<InspectableObject>(),new Vector3(-4.2f,0,-.8f)); Assert.True(UIManager.Instance.IsReading); yield return Tap(Key.E); Assert.False(UIManager.Instance.IsReading);
        foreach(var rune in RunePuzzleLogic.DefaultAnswer)
        {
            var stone=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(x=>x.Rune==rune); var radial=stone.transform.position-new Vector3(0,stone.transform.position.y,.5f); var stand=new Vector3(0,0,.5f)+radial.normalized*1.7f;
            yield return Use(stone,stand);
        }
        yield return new WaitForSeconds(2.2f); Assert.AreEqual(PuzzleState.Solved,GameManager.Instance.Progression.GetState(PuzzleId.Rune));
        yield return Use(Scenario.Pickup(ItemIds.CrystalSapphire),new Vector3(0,0,-1.1f));
        yield return Use(GameObject.Find("RecipeBook").GetComponent<InspectableObject>(),new Vector3(4.2f,0,2.6f)); Assert.True(UIManager.Instance.IsReading); yield return Tap(Key.E);
        Vector3[] stands={new Vector3(4.1f,0,-3.4f),new Vector3(4.4f,0,1.5f),new Vector3(-3.5f,0,-4.4f)};
        for(int i=0;i<3;i++) yield return Use(Scenario.Pickup(PotionPuzzleLogic.DefaultRecipe[i]),stands[i]);
        yield return Slot(ItemIds.Moonpetal); InputSystem.QueueDeltaStateEvent(mouse.scroll,new Vector2(0,-120)); yield return null; yield return null; Assert.AreEqual(ItemIds.SalamanderScale,InventoryManager.Instance.SelectedItemId);
        foreach(var id in PotionPuzzleLogic.DefaultRecipe){yield return Slot(id); yield return Use(Object.FindFirstObjectByType<PotionPuzzle>(),new Vector3(3,0,1.2f));}
        yield return new WaitForSeconds(1); Assert.AreEqual(PuzzleState.Solved,GameManager.Instance.Progression.GetState(PuzzleId.Potion));
        yield return Slot(ItemIds.PotionRevelation); yield return Use(Object.FindFirstObjectByType<SealInscription>(),new Vector3(-3,0,2.3f)); Assert.False(UIManager.Instance.IsReading);
        var sockets=Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None);
        yield return Slot(ItemIds.CrystalSapphire); yield return Use(sockets.First(x=>x.Side==SocketSide.Left),new Vector3(-3.6f,0,2.3f));
        yield return Slot(ItemIds.CrystalRuby); yield return Use(sockets.First(x=>x.Side==SocketSide.Right),new Vector3(-2.4f,0,2.3f));
        yield return Use(Object.FindFirstObjectByType<SealCore>(),new Vector3(-3,0,2.3f)); yield return new WaitForSeconds(3.8f);
        // Interaction travel is teleported to deterministic reachable stations; the final exit approach uses real W input and collisions.
        Place(new Vector3(0,0,2)); yield return Walk(new Vector3(0,0,6));
        Assert.AreEqual(GameFlowState.Cleared,GameManager.Instance.Progression.Flow); Assert.NotNull(GameObject.Find("ClearPanel"));
    }
    [UnityTest] public IEnumerator ExitStraightCenter(){yield return StraightExit(0);}
    [UnityTest] public IEnumerator ExitStraightLeft(){yield return StraightExit(-.8f);}
    [UnityTest] public IEnumerator ExitStraightRight(){yield return StraightExit(.8f);}
    IEnumerator StraightExit(float offset)
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        previousEditor=InputSystem.settings.editorInputBehaviorInPlayMode; previousBackground=InputSystem.settings.backgroundBehavior;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView; InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        keyboard=InputSystem.AddDevice<Keyboard>(); mouse=InputSystem.AddDevice<Mouse>(); GameManager.Instance.StartGame();
        yield return Scenario.Rune(); Scenario.Collect(ItemIds.CrystalSapphire); yield return Scenario.Potion();
        Scenario.Select(ItemIds.PotionRevelation); Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player); UIManager.Instance.CloseRead();
        foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None)) {Scenario.Select(socket.Side==SocketSide.Left?ItemIds.CrystalSapphire:ItemIds.CrystalRuby); socket.Interact(Scenario.Player);}
        Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player); yield return new WaitForSeconds(3.8f);
        Place(new Vector3(offset,0,1)); Scenario.Player.transform.rotation=Quaternion.identity;
        float elapsed=0,stalled=0; var previous=Scenario.Player.transform.position;
        while(GameManager.Instance.IsPlaying && elapsed<5)
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W)); yield return null;
            float dt=Time.unscaledDeltaTime; elapsed+=dt; var now=Scenario.Player.transform.position;
            float speed=new Vector2(now.x-previous.x,now.z-previous.z).magnitude/Mathf.Max(dt,.0001f);
            stalled=speed<.05f?stalled+dt:0; Assert.Less(stalled,.5f,"Forward movement stalled at "+now); previous=now;
        }
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());
        Assert.AreEqual(GameFlowState.Cleared,GameManager.Instance.Progression.Flow,"W-only exit timeout, x="+offset);
    }
    [UnityTearDown] public IEnumerator Cleanup(){InputSystem.settings.editorInputBehaviorInPlayMode=previousEditor; InputSystem.settings.backgroundBehavior=previousBackground; if(keyboard!=null)InputSystem.RemoveDevice(keyboard); if(mouse!=null)InputSystem.RemoveDevice(mouse); Time.timeScale=1; yield return null;}
}





