using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

public class FifthPassTests
{
    static RoomPresentation Presentation => Object.FindFirstObjectByType<RoomPresentation>();
    static string Toast => Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(t=>t.name=="ToastText").text;
    static IEnumerator Load(bool skip=true)
    {
        yield return SceneManager.LoadSceneAsync("Classroom");yield return null;
        GameManager.Instance.StartGame();if(skip)GameManager.Instance.SkipPresentation();yield return null;
    }
    static void Reveal()
    {
        Scenario.Select(ItemIds.PotionRevelation);Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player);
    }
    [UnityTest] public IEnumerator OpeningAnyKeySkipsWithoutGameplayInput()
    {
        var oldEditor=InputSystem.settings.editorInputBehaviorInPlayMode;var oldBackground=InputSystem.settings.backgroundBehavior;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        var keyboard=InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return Load(false);Assert.True(Presentation.InputBlocked);Assert.False(Scenario.Player.GetComponent<PlayerController>().ControlEnabled);
            Assert.AreEqual(0,Presentation.LitSealCount);float elapsed=GameManager.Instance.Progression.ElapsedSeconds;
            yield return new WaitForSeconds(.15f);Assert.AreEqual(elapsed,GameManager.Instance.Progression.ElapsedSeconds);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return null;yield return null;
            Assert.False(Presentation.InputBlocked);Assert.True(Scenario.Player.GetComponent<PlayerController>().ControlEnabled);Assert.AreEqual(3,Presentation.LitSealCount);
            Assert.AreEqual(GameFlowState.Playing,GameManager.Instance.Progression.Flow);Assert.False(UIManager.Instance.IsReading);
        }
        finally {InputSystem.RemoveDevice(keyboard);InputSystem.settings.editorInputBehaviorInPlayMode=oldEditor;InputSystem.settings.backgroundBehavior=oldBackground;}
        yield return Load(false);yield return new WaitForSecondsRealtime(12.4f);
        Assert.False(Presentation.InputBlocked,"Unskipped opening ends after its 12-second timeline");Assert.AreEqual(3,Presentation.LitSealCount);
        Assert.Less(GameManager.Instance.Progression.ElapsedSeconds,1,"Opening is not counted as player clear time");
    }
    [UnityTest] public IEnumerator SealReleaseChangesLampsBrightnessAndFrameFeedback()
    {
        yield return Load();var clues=Object.FindFirstObjectByType<VisualClues>();var rune=Object.FindFirstObjectByType<RunePuzzle>();
        var stones=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None);Color ambient=RenderSettings.ambientLight;
        rune.Press(stones.First(s=>s.Rune==RuneId.Earth),Scenario.Player);Assert.AreEqual(1,clues.LitFrames);Assert.True(clues.frameLights[0].enabled);
        rune.Press(stones.First(s=>s.Rune==RuneId.Wind),Scenario.Player);Assert.AreEqual(0,clues.LitFrames);Assert.True(clues.frameLights.All(l=>!l.enabled));
        yield return Scenario.Rune();Assert.AreEqual(1,Presentation.BrightnessStage);Assert.AreEqual(2,Presentation.LitSealCount);Assert.Greater(RenderSettings.ambientLight.grayscale,ambient.grayscale);
        Assert.AreEqual(4,clues.LitFrames);ambient=RenderSettings.ambientLight;Scenario.Collect(ItemIds.CrystalSapphire);
        yield return Scenario.Potion();Assert.AreEqual(2,Presentation.BrightnessStage);Assert.AreEqual(1,Presentation.LitSealCount);Assert.Greater(RenderSettings.ambientLight.grayscale,ambient.grayscale);
        Reveal();foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None)){Scenario.Select(socket.Side==SocketSide.Left?ItemIds.CrystalSapphire:ItemIds.CrystalRuby);socket.Interact(Scenario.Player);}
        Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);yield return new WaitForSeconds(.65f);
        Assert.AreEqual(3,Presentation.BrightnessStage);Assert.AreEqual(0,Presentation.LitSealCount);Assert.True(Presentation.dawn.enabled);Assert.True(Presentation.coreRenderers.All(r=>!r.enabled));
        Assert.True(Presentation.dawnRays.activeSelf);Assert.AreSame(Presentation.dawnWindowMaterial,Presentation.dawnWindow.sharedMaterial);Assert.NotNull(Presentation.sealLatch.clip);
        Assert.True(Presentation.InputBlocked,"Exit opening is also a protected cinematic");
        yield return new WaitForSeconds(3.3f);Assert.True(Object.FindFirstObjectByType<DoorController>().PassageReady);Assert.False(Presentation.InputBlocked);
    }
    [UnityTest] public IEnumerator ClueSentencesNeverAutomaticallyAppearBeforeFailure()
    {
        yield return Load();GameObject.Find("Chalkboard").GetComponent<InspectableObject>().Interact(Scenario.Player);
        Assert.True(UIManager.Instance.IsReading);Assert.AreEqual(string.Empty,GameObject.Find("ReadBody").GetComponent<TMP_Text>().text);
        Assert.That(GameObject.Find("PictureClue").GetComponent<RawImage>().texture.name,Is.EqualTo("ChalkSequence"));UIManager.Instance.CloseRead();
        Assert.IsNull(GameManager.Instance.GetCurrentHint());UIManager.Instance.ToggleObjective();Assert.That(GameObject.Find("HintText").GetComponent<TMP_Text>().text,Does.Contain(GameText.Chalkboard));UIManager.Instance.ToggleObjective();
        yield return Scenario.Rune();Scenario.Collect(ItemIds.CrystalSapphire);GameObject.Find("RecipeBook").GetComponent<InspectableObject>().Interact(Scenario.Player);
        Assert.AreEqual(string.Empty,GameObject.Find("ReadBody").GetComponent<TMP_Text>().text);Assert.That(GameObject.Find("PictureClue").GetComponent<RawImage>().texture.name,Is.EqualTo("PictureRecipe"));UIManager.Instance.CloseRead();
        yield return Scenario.Potion();Reveal();Assert.False(UIManager.Instance.IsReading);Assert.True(Object.FindFirstObjectByType<VisualClues>().socketElements.activeSelf);
        Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player);Assert.False(UIManager.Instance.IsReading);
        foreach(var text in Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))if(text.GetComponentInParent<Canvas>()==null)Assert.False(new[]{GameText.Chalkboard,GameText.Recipe,GameText.InscriptionRevealed}.Contains(text.text));
        Assert.IsNull(GameManager.Instance.GetCurrentHint());
    }
    [UnityTest] public IEnumerator FailureThresholdsShowTextOnlyAtThreeTwoTwo()
    {
        yield return Load();var wrong=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(s=>s.Rune==RuneId.Wind);
        for(int i=1;i<=3;i++){wrong.Interact(Scenario.Player);Assert.AreEqual(i,Object.FindFirstObjectByType<RunePuzzle>().Logic.FailCount);Assert.AreEqual(i>=3,Toast.Contains(GameText.HintRune));}
        yield return Scenario.Rune();Scenario.Collect(ItemIds.CrystalSapphire);var potion=Object.FindFirstObjectByType<PotionPuzzle>();
        for(int i=1;i<=2;i++){Scenario.Collect(ItemIds.Toadstool);Scenario.Select(ItemIds.Toadstool);potion.Interact(Scenario.Player);Assert.AreEqual(i,potion.Logic.FailCount);Assert.AreEqual(i>=2,Toast.Contains(GameText.HintPotion));}
        yield return Scenario.Potion();Reveal();var left=Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None).First(s=>s.Side==SocketSide.Left);Scenario.Select(ItemIds.CrystalRuby);
        for(int i=1;i<=2;i++){left.Interact(Scenario.Player);Assert.AreEqual(i,Object.FindFirstObjectByType<SealPuzzle>().Logic.FailCount);Assert.AreEqual(i>=2,Toast.Contains(GameText.HintSeal));Assert.True(InventoryManager.Instance.Contains(ItemIds.CrystalRuby));}
    }
    [UnityTest] public IEnumerator EndingShowsActualClearTimeAfterWhiteFade()
    {
        yield return Load();var gm=GameManager.Instance;gm.Progression.Tick(125);gm.ReportSolved(PuzzleId.Rune);gm.ReportSolved(PuzzleId.Potion);gm.ReportSolved(PuzzleId.Seal);
        gm.NotifyPlayerEscaped();Assert.AreEqual(GameFlowState.Cleared,gm.Progression.Flow);Assert.True(Presentation.InputBlocked);Assert.False(Presentation.clearGroup.blocksRaycasts);
        yield return new WaitForSecondsRealtime(2.3f);Assert.False(Presentation.InputBlocked);Assert.AreEqual(1,Presentation.clearGroup.alpha);Assert.True(Presentation.clearGroup.blocksRaycasts);
        int seconds=Mathf.FloorToInt(gm.Progression.ElapsedSeconds);Assert.AreEqual(string.Format("탈출 시간 {0:00}:{1:00}",seconds/60,seconds%60),GameObject.Find("ClearTime").GetComponent<TMP_Text>().text);
        Assert.AreEqual(GameText.Ending,GameObject.Find("ClearMessage").GetComponent<TMP_Text>().text);
        Assert.NotNull(GameObject.Find("Button_Restart2").GetComponent<Button>());Assert.NotNull(GameObject.Find("Button_Quit2").GetComponent<Button>());
    }
}
