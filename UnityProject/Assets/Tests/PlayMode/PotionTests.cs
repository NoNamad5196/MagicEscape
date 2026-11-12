using System.Collections;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class PotionTests
{
    [UnityTest] public IEnumerator S7_LocksWrongRespawnsAndRewards()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null; GameManager.Instance.StartGame(); GameManager.Instance.SkipPresentation();
        var p=Object.FindFirstObjectByType<PotionPuzzle>(); var book=GameObject.Find("RecipeBook").GetComponent<InspectableObject>(); var barrier=GameObject.Find("Barrier");
        Scenario.Collect(ItemIds.Moonpetal); Scenario.Select(ItemIds.Moonpetal); p.Interact(Scenario.Player);
        Assert.AreEqual(0,p.Logic.FailCount); Assert.True(InventoryManager.Instance.Contains(ItemIds.Moonpetal)); Assert.AreEqual(GameText.CauldronCold,GameObject.Find("ToastText").GetComponent<TMP_Text>().text);
        book.Interact(Scenario.Player); Assert.False(UIManager.Instance.IsReading); Assert.True(barrier.activeSelf);
        yield return Scenario.Rune(); Assert.False(barrier.activeSelf); book.Interact(Scenario.Player); Assert.AreEqual(string.Empty,GameObject.Find("ReadBody").GetComponent<TMP_Text>().text); Assert.That(GameObject.Find("PictureClue").GetComponent<UnityEngine.UI.RawImage>().texture.name,Is.EqualTo("PictureRecipe")); UIManager.Instance.CloseRead();
        Scenario.Collect(ItemIds.CrystalSapphire); Scenario.Select(ItemIds.CrystalSapphire); p.Interact(Scenario.Player); Assert.True(InventoryManager.Instance.Contains(ItemIds.CrystalSapphire)); Assert.AreEqual(0,p.Logic.FailCount);
        Scenario.Select(ItemIds.Moonpetal); p.Interact(Scenario.Player);
        var dust=Scenario.Pickup(ItemIds.CrystalDust); var home=dust.transform.position;
        Scenario.Collect(ItemIds.CrystalDust); dust.transform.position=Vector3.zero; Scenario.Select(ItemIds.CrystalDust); p.Interact(Scenario.Player);
        Assert.True(Scenario.Pickup(ItemIds.Moonpetal).IsAvailable); Assert.True(dust.IsAvailable); Assert.AreEqual(home,dust.transform.position); Assert.AreEqual(1,p.Logic.FailCount);
        Scenario.Collect(ItemIds.Toadstool); Scenario.Select(ItemIds.Toadstool); p.Interact(Scenario.Player); Assert.True(Scenario.Pickup(ItemIds.Toadstool).IsAvailable); Assert.AreEqual(GameText.HintPotion,p.CurrentHint);
        yield return Scenario.Potion(); Assert.True(p.Logic.IsSolved); Assert.True(InventoryManager.Instance.Contains(ItemIds.CrystalRuby)); Assert.True(InventoryManager.Instance.Contains(ItemIds.PotionRevelation)); Assert.AreEqual(PuzzleState.Active,GameManager.Instance.Progression.GetState(PuzzleId.Seal));
        Assert.False(GameObject.Find("ExitDoor").transform.Find("SealLight_2").gameObject.activeSelf);
    }
}
