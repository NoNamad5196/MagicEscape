using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ArtTests
{
    static string Output => Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/validation/s10f"));
    public static void View(Vector3 position, Vector3 target)
    {
        var player = Scenario.Player.GetComponent<PlayerController>();
        var cc = player.GetComponent<CharacterController>(); cc.enabled = false; player.transform.position = position; cc.enabled = true;
        Vector3 d = target - player.CameraPivot.position;
        player.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg, 0);
        float pitch = -Mathf.Atan2(d.y, new Vector2(d.x, d.z).magnitude) * Mathf.Rad2Deg;
        typeof(PlayerController).GetField("_pitch", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, pitch);
        player.CameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0); Physics.SyncTransforms();
    }
    public static IEnumerator Capture(string name)
    {
        string path = Path.Combine(Output, name + ".png");
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
    [UnityTest]
    public IEnumerator S10f_ArtAudioEffectsLayoutAndGameViewCaptures()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        Directory.CreateDirectory(Output);
        GameManager.Instance.StartGame();
        View(new Vector3(0,0,-4.65f),new Vector3(0,1.5f,4.8f));
        yield return new WaitForSecondsRealtime(2.5f);yield return Capture("15-opening-candles");
        yield return new WaitForSecondsRealtime(5.8f);yield return Capture("16-opening-seals");
        GameManager.Instance.SkipPresentation();yield return null;
        Assert.That(RenderSettings.ambientLight.maxColorComponent,Is.InRange(.14f,.17f),"Readable stage-zero ambient");
        Assert.AreEqual(FogMode.Exponential,RenderSettings.fogMode);
        Assert.AreEqual(2,Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Count(l=>l.enabled&&l.shadows!=LightShadows.None),"Only lantern and moon cast real-time shadows");
        Assert.True(GameObject.Find("Art_ThirdPass").GetComponentsInChildren<Collider>(true).Length==0);
        var ambience=Object.FindFirstObjectByType<RoomAmbience>();Assert.NotNull(ambience);Assert.NotNull(ambience.wind.clip);Assert.NotNull(ambience.drone.clip);Assert.NotNull(ambience.timber.clip);
        yield return null;Assert.True(ambience.wind.isPlaying);Assert.True(ambience.drone.isPlaying);
        foreach(var renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))foreach(var material in renderer.sharedMaterials){Assert.NotNull(material,renderer.name);Assert.True(material.shader.isSupported,material.name+" supported shader");}
        var orbit=GameObject.Find("ArtC_RuneRing_0").transform;var rotation=orbit.rotation;yield return new WaitForSeconds(.1f);Assert.Greater(Quaternion.Angle(rotation,orbit.rotation),.1f,"Seal ring rotates in play");
        foreach(string id in new[]{ItemIds.Moonpetal,ItemIds.SalamanderScale,ItemIds.CrystalDust,ItemIds.Toadstool,ItemIds.CrystalSapphire,ItemIds.CrystalRuby,ItemIds.PotionRevelation})Assert.Greater(InventoryManager.Instance.GetData(id).icon.texture.width,128,"Model-rendered icon: "+id);
        var audio = Object.FindFirstObjectByType<AudioManager>();
        var entries = (AudioManager.SfxEntry[])typeof(AudioManager).GetField("clips", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(audio);
        Assert.AreEqual(Enum.GetValues(typeof(SfxId)).Length, entries.Length);
        foreach (SfxId id in Enum.GetValues(typeof(SfxId))) { Assert.AreEqual(1, entries.Count(e => e.id == id && e.clip != null), id.ToString()); AudioManager.Play(id); }
        foreach (var ps in Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None)) Assert.NotNull(ps.GetComponent<ParticleSystemRenderer>().sharedMaterial, ps.name);
        foreach (var glow in Object.FindObjectsByType<GlowFeedback>(FindObjectsSortMode.None)) Assert.True(glow.GetComponent<Renderer>().sharedMaterial.IsKeywordEnabled("_EMISSION"), glow.name + " persisted emission keyword");
        Assert.True(GameObject.Find("Art_Decor").GetComponentsInChildren<Collider>().Length == 0);
        View(new Vector3(0, 0, -4.65f), new Vector3(0, 1.2f, 1)); yield return Capture("01-classroom");
        View(new Vector3(-1.35f,0,-.1f),new Vector3(-5.8f,1.7f,0));yield return Capture("02-painting-wall");
        View(new Vector3(-4.1f,0,-.8f),new Vector3(-5.8f,1.5f,-.8f));yield return Capture("10-painting-closeup");
        View(new Vector3(-4.2f,0,-4),new Vector3(-5.8f,1.5f,-4));yield return Capture("11-chalkboard-closeup");yield return Capture("17-picture-chalkboard");
        Assert.NotNull(GameObject.Find("ChalkSequence"));
        Assert.False(Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="ChalkboardLabel"&&t.gameObject.scene.IsValid()).gameObject.activeSelf);
        Assert.True(GameObject.Find("Art_SecondPass").GetComponentsInChildren<Collider>().Length==0);
        Assert.True(GameObject.Find("Art_FourthPass").GetComponentsInChildren<Collider>(true).Length==0);
        var uiFont=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="PromptText"&&t.gameObject.scene.IsValid()).font;
        var credits=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="ArtCredits"&&t.gameObject.scene.IsValid());
        foreach(char c in credits.text.Where(c=>!char.IsControl(c)))Assert.True(credits.font.HasCharacter(c,false,false),"Credit glyph U+"+((int)c).ToString("X4"));
        foreach(var field in typeof(GameText).GetFields(BindingFlags.Public|BindingFlags.Static).Where(f=>f.FieldType==typeof(string)))
            foreach(char c in ((string)field.GetValue(null)).Where(c=>!char.IsControl(c)))Assert.True(uiFont.HasCharacter(c,false,false),"Missing GameText glyph U+"+((int)c).ToString("X4"));
        foreach(string roman in new[]{"I","II","III","IV"}) {
            var plate=GameObject.Find("PaintingLabel_"+roman).GetComponent<TMPro.TMP_Text>();plate.ForceMeshUpdate();
            Assert.AreEqual(roman,plate.text);Assert.Greater(plate.textInfo.characterCount,0);Assert.Greater(plate.textInfo.meshInfo[0].vertexCount,0,"Roman plate has visible glyph mesh");
        }
        Assert.True(GameObject.Find("ArtB_Fit_Cauldron").GetComponentsInChildren<MeshRenderer>().Length>0);
        foreach(var r in GameObject.Find("Room").GetComponentsInChildren<MeshRenderer>()) Assert.NotNull(r.sharedMaterial.mainTexture,r.name);
        foreach(var r in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None)) {
            var icon=r.transform.Find("ArtB_RuneIcon_"+r.Rune);Assert.NotNull(icon);
            Assert.True(icon.GetComponent<Renderer>().sharedMaterial.IsKeywordEnabled("_EMISSION"),r.Rune+" icon persisted emission");
        }
        Assert.True(GameObject.Find("ArtB_AltarCircle").GetComponent<Renderer>().sharedMaterial.IsKeywordEnabled("_EMISSION"));
        View(new Vector3(-2.7f,0,1.9f),new Vector3(1,1.25f,-3.5f));yield return Capture("07-student-desks-window");
        View(new Vector3(3.2f,0,-2.2f),new Vector3(5,1.7f,-4.8f));yield return Capture("09-window-moonlight");
        int elementIndex=0;
        foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).OrderBy(s=>s.Rune))
        {
            var radial=(stone.transform.position-new Vector3(0,stone.transform.position.y,.5f)).normalized;
            View(new Vector3(0,0,.5f)+radial*2.35f,stone.transform.position+Vector3.up*.08f);
            yield return new WaitForSeconds(.25f);yield return Capture((19+elementIndex).ToString("00")+"-element-"+stone.Rune);
            View(new Vector3(0,0,.5f)+radial*1.3f,stone.transform.position+Vector3.up*.08f);
            yield return Capture((29+elementIndex).ToString("00")+"-element-closeup-"+stone.Rune);elementIndex++;
        }
        // Temporary mesh-only validation exhibit: six identical slabs at a measured 2m.
        var lineup=new GameObject("ValidationSixSymbols");
        int lineIndex=0;
        foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).OrderBy(s=>s.Rune))
        {
            float x=(lineIndex++-2.5f)*.46f;
            var slab=Object.Instantiate(stone.transform.Find("AlchemicalSlab").gameObject,lineup.transform);slab.transform.position=new Vector3(x,1.6f,.5f);slab.transform.rotation=Quaternion.identity;slab.transform.localScale=new Vector3(.42f,.42f,.05f);
            var icon=Object.Instantiate(stone.transform.Find("ArtB_RuneIcon_"+stone.Rune).gameObject,lineup.transform);icon.transform.position=new Vector3(x,1.6f,.465f);icon.transform.rotation=Quaternion.identity;icon.transform.localScale=new Vector3(.40f,.40f,1);
        }
        View(new Vector3(0,0,-1.535f),new Vector3(0,1.6f,.465f));yield return Capture("35-six-symbols-at-2m");Object.Destroy(lineup);yield return null;
        View(new Vector3(-1.5f, 0, -1), new Vector3(0, 1, .5f));
        foreach (var rune in RunePuzzleLogic.DefaultAnswer) Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(s => s.Rune == rune).Interact(Scenario.Player);
        yield return new WaitForSeconds(.65f); yield return Capture("03-altar");
        yield return new WaitForSeconds(2.2f); Scenario.Collect(ItemIds.CrystalSapphire);
        View(new Vector3(1.4f, 0, 1.4f), new Vector3(3, 1.1f, 2.5f));
        Scenario.Collect(ItemIds.Toadstool); Scenario.Select(ItemIds.Toadstool); Object.FindFirstObjectByType<PotionPuzzle>().Interact(Scenario.Player);
        yield return new WaitForSeconds(.25f); yield return Capture("04-cauldron");
        View(new Vector3(4.2f,0,2.35f),new Vector3(4.2f,1.15f,3.6f));
        GameObject.Find("RecipeBook").GetComponent<InspectableObject>().Interact(Scenario.Player);yield return Capture("18-picture-recipe");UIManager.Instance.CloseRead();
        int step=0;foreach(var id in PotionPuzzleLogic.DefaultRecipe)
        {
            Scenario.Collect(id);Scenario.Select(id);Object.FindFirstObjectByType<PotionPuzzle>().Interact(Scenario.Player);
            View(new Vector3(3,0,1.4f),new Vector3(3,1.03f,2.5f));yield return new WaitForSeconds(.25f);
            yield return Capture((25+step).ToString("00")+"-cauldron-step-"+(step+1));step++;
        }
        yield return new WaitForSeconds(1);

        View(new Vector3(-3, 0, 1.5f), new Vector3(-3, 1.4f, 3.7f));
        Scenario.Select(ItemIds.PotionRevelation); Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player); UIManager.Instance.CloseRead();
        var reveal=Object.FindFirstObjectByType<SealInscription>().transform.Find("RevealedText");
        Assert.False(reveal.Find("InscriptionText").gameObject.activeSelf);
        Assert.True(Object.FindFirstObjectByType<VisualClues>().socketElements.activeSelf);
        var prompt=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="PromptText" && t.gameObject.scene.IsValid());
        Assert.AreNotEqual(RenderMode.WorldSpace,prompt.GetComponentInParent<Canvas>().renderMode);
        Assert.LessOrEqual(prompt.rectTransform.anchoredPosition.y,-280);
        View(new Vector3(-3,0,1.4f),new Vector3(-3,1.65f,3.7f));
        yield return new WaitForSeconds(2);yield return Capture("05-inscription-revealed");
        View(new Vector3(-3,0,2.1f),new Vector3(-3,1.75f,3.7f));yield return Capture("12-inscription-closeup");
        foreach (var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None)) { Scenario.Select(socket.Side == SocketSide.Left ? ItemIds.CrystalSapphire : ItemIds.CrystalRuby); socket.Interact(Scenario.Player); }
        View(new Vector3(-3,0,1.4f),new Vector3(-3,1.55f,3.7f));yield return new WaitForSeconds(.2f);yield return Capture("13-seal-ready-crystal-colours");
        Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);
        yield return new WaitForSeconds(3.8f);
        View(new Vector3(0,0,2.5f),new Vector3(0,1.6f,6));yield return Capture("06-exit");yield return Capture("36-cold-corridor-before-escape");
        // Isolate the corridor lighting without entering the actual exit trigger / clear overlay.
        var exitTrigger=Object.FindFirstObjectByType<ExitTrigger>();exitTrigger.gameObject.SetActive(false);
        View(new Vector3(0,0,5.6f),new Vector3(0,1.65f,7.9f));
        var lights=Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(l=>l.enabled&&l.name!="ArtC_PlayerLantern").ToArray();foreach(var l in lights)l.enabled=false;
        var ambient=RenderSettings.ambientLight;RenderSettings.ambientLight=Color.black;yield return Capture("08-corridor-lantern-only");
        foreach(var l in lights)l.enabled=true;RenderSettings.ambientLight=ambient;
        foreach(string id in new[]{ItemIds.Moonpetal,ItemIds.SalamanderScale,ItemIds.CrystalDust,ItemIds.Toadstool,ItemIds.CrystalSapphire,ItemIds.CrystalRuby,ItemIds.PotionRevelation})InventoryManager.Instance.TryAdd(id);
        View(new Vector3(0,0,-4.65f),new Vector3(0,1.2f,1));yield return Capture("14-hotbar-model-icons");
        Assert.True(GameManager.Instance.IsPlaying,"No clear overlay hides validation views");exitTrigger.gameObject.SetActive(true);
        File.WriteAllText(Path.Combine(Output,"effect-budget.txt"),"Emitters="+Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).Length+"\nActive lights="+Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Count(l=>l.enabled)+"\nRealtime shadow lights="+Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Count(l=>l.enabled&&l.shadows!=LightShadows.None)+"\nLaptop FPS not measured; batchmode is not a performance acceptance test.\n");
        GameManager.Instance.NotifyPlayerEscaped();yield return new WaitForSecondsRealtime(.65f);yield return Capture("37-black-fade-midpoint");
        Assert.Greater(Object.FindFirstObjectByType<RoomPresentation>().fade.color.a,.3f);
        Assert.AreEqual(0,Object.FindFirstObjectByType<RoomPresentation>().fade.color.r);
        yield return new WaitForSecondsRealtime(2);yield return Capture("28-ending");
    }
}


