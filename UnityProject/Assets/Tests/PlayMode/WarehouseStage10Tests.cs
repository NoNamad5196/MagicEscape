using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object=UnityEngine.Object;

public static class Stage10Actions
{
    public static IEnumerator ClearNew()
    {
        foreach(string name in new[]{"ScaleBookStack","ToadstoolBarrelLid","DustPackingRoll"})
        {
            var go=GameObject.Find(name);if(go==null)continue;
            var loose=go.GetComponent<XriLooseProp>();
            var clear=go.GetComponent<XriClearable>();
            if((loose!=null && loose.IsCleared)||(clear!=null && clear.IsCleared))continue;
            var x=go.GetComponent<XRBaseInteractable>();var ray=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
            x.interactionManager.SelectEnter((IXRSelectInteractor)ray,(IXRSelectInteractable)x);
            x.interactionManager.SelectExit((IXRSelectInteractor)ray,(IXRSelectInteractable)x);
        }
        yield return new WaitForSeconds(.8f);
    }
}
public class WarehouseStage10Tests
{
    [UnitySetUp] public IEnumerator Load()
    {
        yield return SceneManager.LoadSceneAsync("Warehouse");yield return null;
        Object.FindFirstObjectByType<RigSelector>().Apply(false);
        GameManager.Instance.StartGame();GameManager.Instance.SkipPresentation();yield return null;yield return null;
    }
    [UnityTest] public IEnumerator NewCoversBlockThenRevealAndResetAllThreeTargets()
    {
        Assert.AreEqual(8,Object.FindObjectsByType<ClearingFeedback>(FindObjectsSortMode.None).Length);
        foreach(string name in new[]{"ScaleBookStack","ToadstoolBarrelLid","DustPackingRoll"})
        {
            var prop=GameObject.Find(name);
            var gate=Object.FindObjectsByType<ClearingGate>(FindObjectsSortMode.None).Single(g=>g.blocker?.gameObject==prop || g.books?.gameObject==prop);
            var reveal=Object.FindObjectsByType<ClearingReveal>(FindObjectsSortMode.None).Single(r=>r.gate==gate);
            AssertBlocked(gate); Assert.AreEqual(0,reveal.AnnouncementCount);
            Vector3 stand=prop.transform.position.x>0 ? new Vector3(2.2f,0,prop.transform.position.z-.25f) : new Vector3(-2.7f,0,prop.transform.position.z);
            if(name=="DustPackingRoll")
            {
                var chest=GameObject.Find("ChestLidHinge").GetComponent<XriClearable>();
                var ray=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
                chest.interactionManager.SelectEnter((IXRSelectInteractor)ray,chest);chest.interactionManager.SelectExit((IXRSelectInteractor)ray,chest);
                yield return new WaitForSeconds(.75f);AssertBlocked(gate);
                stand=new Vector3(-2.1f,0,-2.0f);
            }
            var aim=prop.GetComponentInChildren<Collider>().bounds.center;
            ArtTests.View(stand,aim);yield return new WaitForSeconds(.15f);
            ArtTests.View(Scenario.Player.transform.position,aim);yield return null;yield return null;
            Assert.True(Object.FindFirstObjectByType<DesktopXriDriver>().ray.TryGetCurrent3DRaycastHit(out var hit));
            Assert.True(hit.collider.transform.IsChildOf(prop.transform),name+" occluded by "+hit.collider.name);
            yield return WarehouseDarkeningTests.Capture("stage10-"+name+"-before");
            Assert.True(prop.GetComponent<ClearingFeedback>().Highlighted);
            if(name=="ScaleBookStack")
            {
                Assert.That(GameObject.Find("PromptText").GetComponent<TMPro.TMP_Text>().text,Does.Contain("[좌클릭] 들기"));
                yield return WarehouseDarkeningTests.Capture("stage10-operation-prompt");
            }
            var driver=Object.FindFirstObjectByType<DesktopXriDriver>();driver.Click();yield return null;yield return null;
            if(prop.GetComponent<XriLooseProp>()!=null) { Assert.True(prop.GetComponent<XRGrabInteractable>().isSelected);driver.Drop(); }
            yield return new WaitForSeconds(1.5f);
            Assert.True(gate.IsAccessible,name);
            foreach(var x in gate.GetComponents<XRBaseInteractable>())Assert.True(x.enabled);
            foreach(var c in gate.GetComponents<Collider>())Assert.True(c.enabled);
            Assert.AreEqual(1,reveal.AnnouncementCount);Assert.Greater(reveal.source.intensity,.7f);
            Assert.Greater(prop.GetComponent<ClearingFeedback>().ImpactCount,0);
            // Select the exposed target through the actual shared manager (also used by VR).
            var target=gate.GetComponent<XRBaseInteractable>();
            target.interactionManager.SelectEnter((IXRSelectInteractor)driver.ray,(IXRSelectInteractable)target);
            yield return null;Assert.True(target.isSelected);
            target.interactionManager.SelectExit((IXRSelectInteractor)driver.ray,(IXRSelectInteractable)target);
            UIManager.Instance.CloseRead();
            ArtTests.View(stand,gate.transform.position);yield return WarehouseDarkeningTests.Capture("stage10-"+name+"-after");
            // Pickup is not consumed by the manual test selection after releasing it; respawn if needed.
            var pickup=gate.GetComponent<ItemPickup>();if(pickup!=null){InventoryManager.Instance.Remove(pickup.ItemId);pickup.Respawn();}
            if(prop.GetComponent<XriLooseProp>() is XriLooseProp loose)loose.ResetProp();else prop.GetComponent<XriClearable>().ResetProp();
            yield return null;yield return null;AssertBlocked(gate);
        }
        Assert.AreEqual(PuzzleState.Active,GameManager.Instance.Progression.GetState(PuzzleId.Rune));
    }
    static void AssertBlocked(ClearingGate gate)
    {
        Assert.False(gate.IsAccessible);
        foreach(var x in gate.GetComponents<XRBaseInteractable>())Assert.False(x.enabled);
        foreach(var c in gate.GetComponents<Collider>())Assert.False(c.enabled);
        var inspect=gate.GetComponent<InspectableObject>();
        if(inspect!=null){inspect.Interact(Scenario.Player);Assert.False(UIManager.Instance.IsReading);}
        var pickup=gate.GetComponent<ItemPickup>();
        if(pickup!=null)
        {
            Assert.False(pickup.IsAvailable);Assert.IsEmpty(pickup.GetPrompt(Scenario.Player));
            pickup.Interact(Scenario.Player);Assert.False(InventoryManager.Instance.Contains(pickup.ItemId));
        }
    }
    [UnityTest] public IEnumerator NewArtAndSharedRevealRemainVisibleInDarkness()
    {
        yield return Stage10Actions.ClearNew();
        foreach(var clear in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None))
        {
            if(clear.kind==ClearingKind.Slide)clear.MoveWithinRails(clear.name.EndsWith("IV")?clear.minPosition:clear.maxPosition);
            else if(clear.kind==ClearingKind.Cloth)
            {
                var ray=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
                clear.interactionManager.SelectEnter((IXRSelectInteractor)ray,clear);clear.interactionManager.SelectExit((IXRSelectInteractor)ray,clear);
            }
        }
        yield return new WaitForSeconds(1.5f);
        foreach(string roman in new[]{"I","II","III","IV"})
        {
            var frame=GameObject.Find("Painting_"+roman).transform;
            ArtTests.View(new Vector3(-2.9f,0,roman=="III"?-.85f:frame.position.z),frame.position);
            yield return WarehouseDarkeningTests.Capture("stage10-frame-"+roman+"-close");
        }
        var controller=Scenario.Player.GetComponent<PlayerController>();controller.enabled=false;
        float boardFov=Camera.main.fieldOfView;Camera.main.fieldOfView=38;
        ArtTests.View(new Vector3(-2.4f,0,-3.05f),GameObject.Find("Chalkboard").transform.position);
        yield return WarehouseDarkeningTests.Capture("stage10-chalk-close");
        Camera.main.fieldOfView=boardFov;controller.enabled=true;
        GameObject.Find("Chalkboard").GetComponent<InspectableObject>().Interact(Scenario.Player);
        yield return WarehouseDarkeningTests.Capture("stage10-chalk-inspect");UIManager.Instance.CloseRead();
        var pivot=Scenario.Player.GetComponent<PlayerController>().CameraPivot;pivot.localPosition=new Vector3(0,2.75f,0);
        float fov=Camera.main.fieldOfView;Camera.main.fieldOfView=82;
        ArtTests.View(new Vector3(-2.25f,0,-.7f),new Vector3(-4.25f,1.45f,-.8f));
        yield return WarehouseDarkeningTests.Capture("stage10-chalk-frames-wide");
        pivot.localPosition=new Vector3(0,1.6f,0);Camera.main.fieldOfView=fov;
        var chest=GameObject.Find("ChestLidHinge").GetComponent<XriClearable>();
        var desktopRay=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
        chest.interactionManager.SelectEnter((IXRSelectInteractor)desktopRay,chest);chest.interactionManager.SelectExit((IXRSelectInteractor)desktopRay,chest);
        yield return new WaitForSeconds(1.5f);
        Object.FindFirstObjectByType<WarehouseDarkening>().SetStage(3,false);yield return null;
        foreach(var reveal in Object.FindObjectsByType<ClearingReveal>(FindObjectsSortMode.None))
        {Assert.Greater(reveal.source.intensity,.7f);Assert.AreEqual(1,reveal.AnnouncementCount);}
        Assert.False(Object.FindFirstObjectByType<RoomPresentation>().InputBlocked);
    }
}
