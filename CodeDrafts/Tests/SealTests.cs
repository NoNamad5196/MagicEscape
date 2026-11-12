using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
public class SealTests
{
    [UnityTest] public IEnumerator S8_SealRejectionDoorPhysicalTriggerClearRestart()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null; GameManager.Instance.StartGame();
        var p=Object.FindFirstObjectByType<SealPuzzle>(); var inscription=Object.FindFirstObjectByType<SealInscription>(); var core=Object.FindFirstObjectByType<SealCore>();
        var sockets=Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None); var left=sockets.First(x=>x.Side==SocketSide.Left); var right=sockets.First(x=>x.Side==SocketSide.Right);
        var exit=Object.FindFirstObjectByType<ExitTrigger>(FindObjectsInactive.Include);
        inscription.Interact(Scenario.Player); left.Interact(Scenario.Player); core.Interact(Scenario.Player); Assert.AreEqual(SealStage.Dormant,p.Stage); Assert.AreEqual(GameText.SealRemaining,GameObject.Find("ToastText").GetComponent<TMP_Text>().text);
        yield return Scenario.Rune(); Scenario.Collect(ItemIds.CrystalSapphire); yield return Scenario.Potion(); Assert.AreEqual(SealStage.AwaitingReveal,p.Stage);
        Scenario.Select(ItemIds.CrystalSapphire); inscription.Interact(Scenario.Player); Assert.AreEqual(GameText.InscriptionBlurred,GameObject.Find("ToastText").GetComponent<TMP_Text>().text);
        left.Interact(Scenario.Player); Assert.AreEqual(SealStage.AwaitingReveal,p.Stage);
        core.Interact(Scenario.Player); Assert.AreEqual(GameText.CoreNotReady,GameObject.Find("ToastText").GetComponent<TMP_Text>().text);
        Scenario.Select(ItemIds.PotionRevelation); inscription.Interact(Scenario.Player); Assert.False(InventoryManager.Instance.Contains(ItemIds.PotionRevelation)); Assert.AreEqual(SealStage.AwaitingCrystals,p.Stage); Assert.False(UIManager.Instance.IsReading); Assert.True(Object.FindFirstObjectByType<VisualClues>().socketElements.activeSelf);
        Scenario.Select(ItemIds.CrystalRuby); left.Interact(Scenario.Player); left.Interact(Scenario.Player); Assert.True(InventoryManager.Instance.Contains(ItemIds.CrystalRuby)); Assert.AreEqual(GameText.HintSeal,p.CurrentHint);
        Scenario.Select(ItemIds.CrystalSapphire); left.Interact(Scenario.Player); Assert.False(InventoryManager.Instance.Contains(ItemIds.CrystalSapphire)); Scenario.Select(ItemIds.CrystalRuby); right.Interact(Scenario.Player); Assert.AreEqual(SealStage.ReadyToRelease,p.Stage);
        Assert.True(left.transform.Find("PlacedCrystal").gameObject.activeSelf); Assert.True(right.transform.Find("PlacedCrystal").gameObject.activeSelf);
        core.Interact(Scenario.Player); Assert.AreEqual(SealStage.Released,p.Stage); Assert.False(exit.gameObject.activeSelf); yield return new WaitForSeconds(3.8f);
        Assert.True(exit.gameObject.activeSelf); foreach(Transform t in GameObject.Find("ExitDoor").transform) if(t.name.StartsWith("SealLight_")) Assert.False(t.gameObject.activeSelf);
        var cc=Scenario.Player.GetComponent<CharacterController>(); cc.enabled=false; cc.transform.position=new Vector3(0,.05f,5.1f); cc.enabled=true; Physics.SyncTransforms();
        for(int i=0;i<15 && GameManager.Instance.Progression.Flow!=GameFlowState.Cleared;i++){cc.Move(Vector3.forward*.1f); yield return new WaitForFixedUpdate();}
        Assert.AreEqual(GameFlowState.Cleared,GameManager.Instance.Progression.Flow,"Actual physics trigger must clear"); Assert.NotNull(GameObject.Find("ClearPanel")); Assert.That(GameObject.Find("ClearTime").GetComponent<TMP_Text>().text,Does.Match(@"탈출 시간 \d\d:\d\d"));
        GameObject.Find("Button_Restart2").GetComponent<Button>().onClick.Invoke(); yield return null; yield return null;
        Assert.AreEqual(GameFlowState.Title,GameManager.Instance.Progression.Flow); Assert.AreEqual(0,InventoryManager.Instance.Model.Count); Assert.AreEqual(PuzzleState.Active,GameManager.Instance.Progression.GetState(PuzzleId.Rune)); Assert.AreEqual(SealStage.Dormant,Object.FindFirstObjectByType<SealPuzzle>().Stage);
    }
}
