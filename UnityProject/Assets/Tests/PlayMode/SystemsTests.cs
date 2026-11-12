using System.Collections;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class SystemsTests
{
    [UnityTest] public IEnumerator S2_PlayerSystems()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        var gm=GameManager.Instance; Assert.NotNull(gm);
        var p=Object.FindFirstObjectByType<PlayerController>(); Assert.NotNull(p.CameraPivot);
        Assert.AreEqual(8,p.gameObject.layer); Assert.AreEqual(1,Object.FindObjectsByType<Camera>(FindObjectsSortMode.None).Length);
        Assert.NotNull(Object.FindFirstObjectByType<InputSystemUIInputModule>());
        Assert.AreEqual(8,InventoryManager.Instance.Model.Capacity);
        gm.StartGame(); gm.SkipPresentation(); yield return null;
        Assert.True(p.ControlEnabled); Assert.AreEqual(GameFlowState.Playing,gm.Progression.Flow);
        gm.TogglePause(); Assert.AreEqual(0,Time.timeScale); Assert.False(p.ControlEnabled);
        gm.Resume(); Assert.AreEqual(1,Time.timeScale); Assert.True(p.ControlEnabled);
        var cc=p.GetComponent<CharacterController>();
        for(int i=0;i<20;i++) cc.Move(Vector3.right);
        Assert.Less(p.transform.position.x,5.7f,"Wall collider stops controller");
        // Unity Test Framework automatically fails unexpected Error/Exception logs.
    }
    [UnityTearDown] public IEnumerator Cleanup() { Time.timeScale=1; yield return null; }
}

