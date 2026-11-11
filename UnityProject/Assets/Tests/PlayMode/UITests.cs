using System.Collections;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
public class UITests
{
    [UnityTest] public IEnumerator S3_PanelsButtonsReadRestart()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        Assert.AreEqual(GameFlowState.Title,GameManager.Instance.Progression.Flow);
        Assert.NotNull(GameObject.Find("TitlePanel"));
        GameObject.Find("Button_Start").GetComponent<Button>().onClick.Invoke(); yield return null;
        Assert.True(GameManager.Instance.IsPlaying); Assert.NotNull(GameObject.Find("HUDPanel"));
        UIManager.Instance.ShowRead("칠판",GameText.Chalkboard); Assert.True(UIManager.Instance.IsReading);
        Assert.False(Object.FindFirstObjectByType<PlayerController>().ControlEnabled);
        UIManager.Instance.CloseRead(); Assert.True(Object.FindFirstObjectByType<PlayerController>().ControlEnabled);
        UIManager.Instance.ToggleObjective(); Assert.AreEqual(GameText.ObjectiveRune,GameObject.Find("ObjectiveText").GetComponent<TMP_Text>().text);
        GameManager.Instance.TogglePause(); Assert.NotNull(GameObject.Find("PausePanel"));
        GameObject.Find("Button_Resume").GetComponent<Button>().onClick.Invoke(); Assert.True(GameManager.Instance.IsPlaying);
        foreach(var text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None)) Assert.That(text.font.name,Does.Contain("NotoSansKR"));
        GameManager.Instance.TogglePause(); GameObject.Find("Button_Restart").GetComponent<Button>().onClick.Invoke(); yield return null; yield return null;
        Assert.AreEqual(GameFlowState.Title,GameManager.Instance.Progression.Flow); Assert.AreEqual(1,Time.timeScale); Assert.AreEqual(0,InventoryManager.Instance.Model.Count);
    }
}
