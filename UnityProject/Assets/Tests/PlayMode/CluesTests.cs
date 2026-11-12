using System.Collections;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class CluesTests
{
    [UnityTest] public IEnumerator S5_ClueReadsAndVisibleOrder()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null; GameManager.Instance.StartGame(); GameManager.Instance.SkipPresentation();
        var player=Object.FindFirstObjectByType<PlayerInteraction>();
        var names=new[]{"III","I","IV","II"}; float last=-100;
        foreach(var roman in names)
        {
            var painting=GameObject.Find("Painting_"+roman); Assert.Greater(painting.transform.position.z,last); last=painting.transform.position.z;
            painting.GetComponent<InspectableObject>().Interact(player); Assert.True(UIManager.Instance.IsReading);
            Assert.AreEqual(string.Empty,GameObject.Find("ReadBody").GetComponent<TMP_Text>().text); UIManager.Instance.CloseRead();
            Assert.That(GameObject.Find("PaintingLabel_"+roman).GetComponent<TMP_Text>().text,Does.Contain(roman));
        }
        GameObject.Find("Chalkboard").GetComponent<InspectableObject>().Interact(player);
        Assert.AreEqual(string.Empty,GameObject.Find("ReadBody").GetComponent<TMP_Text>().text); Assert.NotNull(GameObject.Find("PictureClue").GetComponent<UnityEngine.UI.RawImage>().texture); UIManager.Instance.CloseRead();
    }
}
