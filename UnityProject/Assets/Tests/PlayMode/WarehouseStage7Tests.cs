using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
public class WarehouseStage7Tests
{
    [UnityTest] public IEnumerator ReadableScenesItemsAndRewardsAtStandingEye()
    {
        yield return SceneManager.LoadSceneAsync("Warehouse");yield return null;
        Object.FindFirstObjectByType<RigSelector>().Apply(false);
        GameManager.Instance.StartGame();GameManager.Instance.SkipPresentation();yield return null;
        foreach(var gate in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None))
        {
            if(gate.kind==ClearingKind.Slide)gate.MoveWithinRails(gate.name.EndsWith("IV")?gate.minPosition:gate.maxPosition);
            else if(gate.kind==ClearingKind.Cloth)
            {
                var ray=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
                gate.interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)ray,gate);
                gate.interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)ray,gate);
            }
        }
        yield return new WaitForSeconds(.8f);
        foreach(string roman in new[]{"I","II","III","IV"})
        {
            var frame=GameObject.Find("Painting_"+roman).transform;
            ArtTests.View(new Vector3(-2.9f,0,roman=="III"?-.85f:frame.position.z),frame.position);
            yield return WarehouseDarkeningTests.Capture("stage8-frame-"+roman+"-close");
        }
        var lid=GameObject.Find("ChestLidHinge").GetComponent<XriClearable>();
        var desktopRay=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
        lid.interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)desktopRay,lid);
        lid.interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)desktopRay,lid);
        yield return new WaitForSeconds(.8f);
        ArtTests.View(new Vector3(-2.9f,0,-1.75f),new Vector3(-4.2f,1.65f,-1.75f));
        yield return WarehouseDarkeningTests.Capture("stage8-frame-III-chest-open");
        var altar=Object.FindFirstObjectByType<RunePuzzle>().transform;
        ArtTests.View(new Vector3(altar.position.x,0,altar.position.z-1.5f),altar.position);
        yield return WarehouseDarkeningTests.Capture("stage8-altar-pc");
        var pivot=Scenario.Player.GetComponent<PlayerController>().CameraPivot;pivot.localPosition=new Vector3(0,1.75f,0);
        ArtTests.View(new Vector3(altar.position.x,0,altar.position.z-1.5f),altar.position);
        yield return WarehouseDarkeningTests.Capture("stage8-altar-standing");
        pivot.localPosition=new Vector3(0,1.6f,0);
        // Check actual wrong-material respawn at newly authored homes, not just serialized positions.
        var mushroom=Scenario.Pickup(ItemIds.Toadstool);var home=mushroom.transform.position;
        mushroom.transform.position+=Vector3.left;mushroom.Respawn();Assert.Less(Vector3.Distance(home,mushroom.transform.position),.001f);
        yield return Scenario.Rune();yield return new WaitForSeconds(.2f);
        var sapphire=Scenario.Pickup(ItemIds.CrystalSapphire);Assert.True(sapphire.IsAvailable);
        var sapphireBeacon=Object.FindObjectsByType<WarehouseItemBeacon>(FindObjectsSortMode.None).Single(b=>b.pickup==sapphire);
        Assert.AreEqual(1,sapphireBeacon.AnnouncementCount);
        yield return ItemView(ItemIds.CrystalSapphire,new Vector3(-.8f,0,-1.8f));
        yield return ItemView(ItemIds.Moonpetal,new Vector3(2.85f,0,-1.9f));
        yield return ItemView(ItemIds.SalamanderScale,new Vector3(2.45f,0,-.35f));
        yield return ItemView(ItemIds.CrystalDust,new Vector3(-2.15f,0,-1.8f));
        yield return ItemView(ItemIds.Toadstool,new Vector3(2.25f,0,-1.5f));
        yield return Scenario.Potion();yield return new WaitForSeconds(1);
        foreach(var b in Object.FindObjectsByType<WarehouseItemBeacon>(FindObjectsSortMode.None).Where(b=>b.reward))
            Assert.AreEqual(1,b.AnnouncementCount,b.pickup.ItemId+" reward appearance announced once");
        yield return ItemView(ItemIds.CrystalRuby,new Vector3(1.8f,0,.9f));
        yield return ItemView(ItemIds.PotionRevelation,new Vector3(2.8f,0,.4f));
        Object.FindFirstObjectByType<WarehouseDarkening>().SetStage(3,false);yield return null;
        yield return ItemView(ItemIds.CrystalRuby,new Vector3(1.8f,0,.9f),"-darkest");
        // Cutaway map of the authored scene, not a hand-reconstructed diagram.
        Object.FindFirstObjectByType<WarehouseDarkening>().SetStage(0,false);
        var hidden=Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(r=>r.enabled&&r.bounds.center.y>2.6f&&r.bounds.size.y<.8f).ToArray();
        foreach(var r in hidden)r.enabled=false;
        int mapIndex=0;
        foreach(var item in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).OrderBy(i=>i.ItemId))
        {
            item.Respawn();
            var marker=new GameObject("MapLabel").AddComponent<TMPro.TextMeshPro>();
            marker.text=(++mapIndex).ToString();marker.fontSize=2.8f;marker.alignment=TMPro.TextAlignmentOptions.Center;marker.color=Color.white;
            marker.rectTransform.sizeDelta=new Vector2(.3f,.3f);
            marker.transform.SetPositionAndRotation(item.transform.position+new Vector3(-.25f,.45f,-.10f),Quaternion.Euler(90,0,0));
        }
        var mapFill=new GameObject("MapOnlyFill").AddComponent<Light>();mapFill.type=LightType.Directional;mapFill.intensity=.65f;mapFill.transform.rotation=Quaternion.Euler(90,0,0);
        var controller=Scenario.Player.GetComponent<PlayerController>();controller.ControlEnabled=false;
        var camera=Camera.main;camera.transform.SetPositionAndRotation(new Vector3(0,10,0),Quaternion.Euler(90,0,0));camera.orthographic=true;camera.orthographicSize=5.1f;
        yield return WarehouseDarkeningTests.Capture("stage8-item-map");
        camera.orthographic=false;foreach(var r in hidden)r.enabled=true;
    }
    IEnumerator ItemView(string id,Vector3 stand,string suffix="")
    {
        var item=Scenario.Pickup(id);Assert.True(item.IsAvailable,id);
        ArtTests.View(stand,item.transform.position);yield return null;yield return null;
        var screen=Camera.main.WorldToViewportPoint(item.transform.position);
        Assert.That(screen.x,Is.InRange(0f,1f));Assert.That(screen.y,Is.InRange(0f,1f));Assert.Greater(screen.z,0);
        var beacon=Object.FindObjectsByType<WarehouseItemBeacon>(FindObjectsSortMode.None).Single(b=>b.pickup==item);
        Assert.True(beacon.source.enabled);Assert.Greater(beacon.source.intensity,.1f);
        yield return WarehouseDarkeningTests.Capture("stage8-item-"+id+suffix);
    }
}
