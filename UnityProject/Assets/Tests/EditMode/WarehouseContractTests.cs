using MagicEscape.Core;
using NUnit.Framework;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.OpenXR;
public class WarehouseContractTests
{
    [Test] public void ProposalDoesNotChangeAnswersOrIds()
    {
        CollectionAssert.AreEqual(new[] { RuneId.Earth, RuneId.Water, RuneId.Fire, RuneId.Light }, RunePuzzleLogic.DefaultAnswer);
        CollectionAssert.AreEqual(new[] { "ing_moonpetal", "ing_salamander_scale", "ing_crystal_dust" }, PotionPuzzleLogic.DefaultRecipe);
        Assert.AreEqual("crystal_sapphire", SealPuzzleLogic.ExpectedCrystal(SocketSide.Left));
        Assert.AreEqual("crystal_ruby", SealPuzzleLogic.ExpectedCrystal(SocketSide.Right));
    }
    [Test] public void ProgressionStillRequiresAllThreePuzzles()
    {
        var flow = new ProgressionModel(); flow.StartGame(); Assert.False(flow.MarkSolved(PuzzleId.Seal));
        Assert.True(flow.MarkSolved(PuzzleId.Rune)); Assert.True(flow.MarkSolved(PuzzleId.Potion)); Assert.False(flow.ExitOpen);
        Assert.True(flow.MarkSolved(PuzzleId.Seal)); Assert.AreEqual(3, flow.SolvedCount); Assert.True(flow.ExitOpen);
    }
    [Test] public void InstalledVersionsAreTheUnity63ReleasedPackages()
    {
        Assert.AreEqual("3.3.2", PackageInfo.FindForAssembly(typeof(XRGrabInteractable).Assembly).version);
        Assert.AreEqual("1.16.1", PackageInfo.FindForAssembly(typeof(OpenXRLoader).Assembly).version);
    }
    [Test] public void WarehouseIsStartupAndClassroomIsRegressionFixture()
    {
        Assert.AreEqual("Assets/Scenes/Warehouse.unity", EditorBuildSettings.scenes[0].path);
        Assert.True(EditorBuildSettings.scenes[0].enabled);
        Assert.True(System.Array.Exists(EditorBuildSettings.scenes, s => s.path == "Assets/Scenes/Classroom.unity" && s.enabled));
    }
}
