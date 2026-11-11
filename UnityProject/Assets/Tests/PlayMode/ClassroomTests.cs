using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ClassroomTests
{
    [UnityTest]
    public IEnumerator S1_RoomAndDoorway()
    {
        yield return SceneManager.LoadSceneAsync("Classroom");
        Assert.That(GraphicsSettings.currentRenderPipeline, Is.Not.Null);
        Assert.That(LayerMask.NameToLayer("Player"), Is.EqualTo(8));
        Assert.That(LayerMask.NameToLayer("Interactable"), Is.EqualTo(9));
        foreach (var name in new[] {"Floor","Ceiling","Wall_W","Wall_E","Wall_S","Wall_N_Left","Wall_N_Right","Wall_N_Top","CorridorFloor"})
            Assert.That(GameObject.Find(name).GetComponent<BoxCollider>(), Is.Not.Null, name);
        Assert.That(Physics.Raycast(new Vector3(0,1.3f,4), Vector3.forward, 2), Is.False, "Doorway must remain open");
        Assert.That(Physics.Raycast(new Vector3(2,1.3f,4), Vector3.forward, 2), Is.True);
        LogAssert.NoUnexpectedReceived();
    }
}
