using System.Collections;
using System.IO;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;
public class WarehouseDarkeningTests
{
    [UnitySetUp] public IEnumerator Load()
    {
        yield return SceneManager.LoadSceneAsync("Warehouse");yield return null;
        Object.FindFirstObjectByType<RigSelector>().Apply(false);
        GameManager.Instance.StartGame();GameManager.Instance.SkipPresentation();yield return null;
    }
    public static IEnumerator Capture(string name)
    {
        string path = Path.Combine(Application.dataPath, "../../docs/validation/warehouse/" + name + ".png");
        if (File.Exists(path)) File.Delete(path);
        yield return null; yield return null;
        // Batchmode has no presented backbuffer. Render the actual gameplay camera and HUD
        // at a fixed size instead; do not use a SceneView/editor camera.
        var camera = Camera.main;
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).Where(c => c.isRootCanvas && c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32);
        var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        try
        {
            foreach (var canvas in canvases) { canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = .1f; }
            Canvas.ForceUpdateCanvases(); camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply(); File.WriteAllBytes(path, image.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
            foreach (var canvas in canvases) { canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.worldCamera = null; }
            RenderTexture.ReleaseTemporary(target); Object.Destroy(image);
        }
        Assert.True(File.Exists(path), "Game-view screenshot " + path);
        Assert.Greater(new FileInfo(path).Length, 1000);
    }

    [UnityTearDown] public IEnumerator ResetTime(){Time.timeScale=1;yield return null;}
    float Sum(WarehouseDarkening d) => d.backgroundLights.Concat(d.wickLights).Concat(d.localLights)
        .Where(l=>l!=null&&l.enabled).Sum(l=>l.intensity)+Object.FindFirstObjectByType<RoomPresentation>().roomFill.intensity;
    void Guides(WarehouseDarkening d)
    {
        Assert.AreEqual(6,d.localLights.Length);
        Assert.True(d.localLights.All(l=>l.enabled && l.intensity>.1f));
        Assert.False(Object.FindFirstObjectByType<RoomPresentation>().InputBlocked);
        Assert.True(Scenario.Player.GetComponent<PlayerController>().ControlEnabled);
    }
    [UnityTest] public IEnumerator StagesDecreaseLightKeepGuidesAndCaptureSameCamera()
    {
        var d=Object.FindFirstObjectByType<WarehouseDarkening>();
        var p=Object.FindFirstObjectByType<RoomPresentation>();
        Assert.True(p.progressiveDarkening);Assert.AreEqual(8,d.wickLights.Length,"Six floor / two sconces");
        float previous=float.MaxValue;
        for(int stage=0;stage<4;stage++)
        {
            if(stage==1){yield return Scenario.Rune();Scenario.Collect(ItemIds.CrystalSapphire);}
            if(stage==2)
            {
                ArtTests.View(new Vector3(-2.1f,0,-2),new Vector3(-3.25f,.61f,-2.3f));yield return null;yield return null;
                Object.FindFirstObjectByType<DesktopXriDriver>().Click();yield return new WaitForSeconds(.7f);
                yield return Scenario.Potion();yield return new WaitForSeconds(1.1f);
            }
            if(stage==3)
            {
                Scenario.Select(ItemIds.PotionRevelation);Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player);
                foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
                {Scenario.Select(socket.Side==SocketSide.Left?ItemIds.CrystalSapphire:ItemIds.CrystalRuby);socket.Interact(Scenario.Player);}
                Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);yield return new WaitForSeconds(4.2f);
            }
            yield return new WaitForSeconds(3.2f); // settle blend and feedback toast before comparing frames
            while(d.Transitioning)yield return null;
            Assert.AreEqual(stage,p.BrightnessStage);
            Guides(d);float sum=Sum(d);Assert.Less(sum,previous);previous=sum;
            Assert.That(d.wickLights.Count(l=>l.enabled),Is.EqualTo(d.extinguishAt.Count(off=>off>stage)),
                string.Join(";",d.wickLights.Select((l,i)=>l.name+":"+l.enabled+":"+l.intensity+":off@"+d.extinguishAt[i])));
            Assert.That(RenderSettings.ambientLight.b,Is.EqualTo(d.profile.stages[stage].ambient.b).Within(.001f));
            Time.timeScale=0;
            ArtTests.View(new Vector3(0,0,-2.3f),new Vector3(.1f,1.5f,3.2f));
            yield return Capture("stage4-"+new[]{"start","p1","p2","p3"}[stage]);
            Time.timeScale=1;
            if(stage==1){ArtTests.View(new Vector3(2.6f,0,-.65f),d.brewLiquid.transform.position);yield return Capture("stage4-p1-cauldron");}
            if(stage==2){ArtTests.View(new Vector3(2.1f,0,1f),new Vector3(2.1f,1.3f,2.65f));yield return Capture("stage4-p2-inscription");}
            if(stage==3)Assert.Greater(d.flames.Count(r=>r.enabled),0);
        }
        ArtTests.View(new Vector3(2.2f,0,.8f),new Vector3(2.1f,1.3f,2.65f));
        yield return Capture("stage4-final-sockets");
        Assert.False(p.dawn.enabled);Assert.False(p.dawnRays.activeSelf);
        foreach(var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {Assert.AreEqual(CameraClearFlags.SolidColor,camera.clearFlags);Assert.AreEqual(Color.black,camera.backgroundColor);}
    }
    [UnityTest] public IEnumerator PuzzleAndClearingInteractionsContinueDuringSmoothTransitions()
    {
        var d=Object.FindFirstObjectByType<WarehouseDarkening>();
        var p=Object.FindFirstObjectByType<RoomPresentation>();
        yield return Scenario.Rune();Scenario.Collect(ItemIds.CrystalSapphire);
        var lid=GameObject.Find("ChestLidHinge").GetComponent<XriClearable>();
        // Select through the same XRI desktop ray used by gameplay, while light blend is active.
        ArtTests.View(new Vector3(-2.1f,0,-2),new Vector3(-3.25f,.61f,-2.3f));
        yield return null;yield return null;
        d.SetStage(1);Assert.True(d.Transitioning);
        Object.FindFirstObjectByType<DesktopXriDriver>().Click();
        yield return new WaitForSeconds(.65f);Assert.True(lid.IsCleared);
        Guides(d);Assert.True(d.Transitioning);
        Scenario.Collect(ItemIds.Moonpetal);Scenario.Select(ItemIds.Moonpetal);
        Object.FindFirstObjectByType<PotionPuzzle>().Interact(Scenario.Player);
        Assert.False(InventoryManager.Instance.Contains(ItemIds.Moonpetal),"Ingredient consumed during transition");
        foreach(var id in PotionPuzzleLogic.DefaultRecipe.Skip(1))
        {Scenario.Collect(id);Scenario.Select(id);Object.FindFirstObjectByType<PotionPuzzle>().Interact(Scenario.Player);}
        yield return new WaitForSeconds(2);Assert.AreEqual(2,p.BrightnessStage);
        Scenario.Select(ItemIds.PotionRevelation);Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player);
        foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
        {Scenario.Select(socket.Side==SocketSide.Left?ItemIds.CrystalSapphire:ItemIds.CrystalRuby);socket.Interact(Scenario.Player);}
        Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);
        yield return new WaitForSeconds(.6f);Assert.AreEqual(3,p.BrightnessStage);Guides(d);
        yield return new WaitForSeconds(4);
        Assert.True(Object.FindFirstObjectByType<DoorController>().PassageReady);
        Assert.False(p.dawn.enabled);
        GameManager.Instance.NotifyPlayerEscaped();
        yield return new WaitForSecondsRealtime(.9f);
        Assert.Less(p.fade.color.r,.01f);Assert.Less(p.fade.color.g,.01f);Assert.Greater(p.fade.color.a,.5f);
        yield return new WaitForSecondsRealtime(.4f);yield return Capture("stage4-black-fade");
        yield return new WaitForSecondsRealtime(1.5f);
        Assert.AreEqual(GameFlowState.Cleared,GameManager.Instance.Progression.Flow);
        Assert.AreEqual(Color.black,p.clearGroup.GetComponent<Image>().color);
        yield return Capture("stage4-ending");
    }
    [UnityTest] public IEnumerator EveryPictureHasSeparateCloseHelp()
    {
        foreach(var item in Object.FindObjectsByType<InspectableObject>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            var field=typeof(InspectableObject).GetField("picture",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            var texture=field.GetValue(item) as Texture2D;if(texture==null)continue;
            UIManager.Instance.ShowPicture(texture);yield return null;Canvas.ForceUpdateCanvases();
            var picture=Object.FindObjectsByType<RawImage>(FindObjectsSortMode.None).Single(r=>r.name=="PictureClue").rectTransform;
            var help=picture.parent.Find("CloseHelp") as RectTransform;
            Assert.Less(help.anchoredPosition.y+help.rect.height/2,picture.anchoredPosition.y-picture.rect.height/2-10,item.name);
            UIManager.Instance.CloseRead();
        }
    }
}


