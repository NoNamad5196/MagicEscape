using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public static class Scenario
{
    public static PlayerInteraction Player=>Object.FindFirstObjectByType<PlayerInteraction>();
    public static void Select(string id) { var inv=InventoryManager.Instance; var i=inv.Model.Items.ToList().IndexOf(id); Assert.GreaterOrEqual(i,0,id); inv.Select(i); }
    public static IEnumerator Rune()
    { foreach(var r in RunePuzzleLogic.DefaultAnswer) Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(x=>x.Rune==r).Interact(Player); yield return new WaitForSeconds(2.2f); }
    public static ItemPickup Pickup(string id)=>Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(x=>x.ItemId==id);
    public static void Collect(string id){ var p=Pickup(id); Assert.True(p.IsAvailable,id); p.Interact(Player); }
    public static IEnumerator Potion()
    { foreach(var id in PotionPuzzleLogic.DefaultRecipe) { Collect(id); Select(id); Object.FindFirstObjectByType<PotionPuzzle>().Interact(Player); } yield return new WaitForSeconds(1); }
}
public class RuneTests
{
    [UnityTest] public IEnumerator S6_WrongHintCorrectRewardSeal()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null; GameManager.Instance.StartGame();
        var puzzle=Object.FindFirstObjectByType<RunePuzzle>(); Assert.False(Scenario.Pickup(ItemIds.CrystalSapphire).IsAvailable);
        var stones=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None);
        for(int i=0;i<3;i++){ foreach(var r in new[]{RuneId.Earth,RuneId.Water,RuneId.Wind}) stones.First(x=>x.Rune==r).Interact(Scenario.Player); }
        Assert.AreEqual(3,puzzle.Logic.FailCount); Assert.AreEqual(0,puzzle.Logic.ProgressCount); Assert.AreEqual(GameText.HintRune,puzzle.CurrentHint);
        yield return Scenario.Rune(); Assert.True(puzzle.IsSolved); Assert.AreEqual(PuzzleState.Active,GameManager.Instance.Progression.GetState(PuzzleId.Potion));
        foreach(var s in stones) Assert.IsNull(s.GetPrompt(Scenario.Player));
        Assert.False(GameObject.Find("ExitDoor").transform.Find("SealLight_1").gameObject.activeSelf);
        Scenario.Collect(ItemIds.CrystalSapphire); Assert.True(InventoryManager.Instance.Contains(ItemIds.CrystalSapphire));
    }
}
