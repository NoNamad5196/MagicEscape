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
    static string Output => Path.GetFullPath(Path.Combine(Application.dataPath, "../../docs/validation/s10b"));
    static void View(Vector3 position, Vector3 target)
    {
        var player = Scenario.Player.GetComponent<PlayerController>();
        var cc = player.GetComponent<CharacterController>(); cc.enabled = false; player.transform.position = position; cc.enabled = true;
        Vector3 d = target - player.CameraPivot.position;
        player.transform.rotation = Quaternion.Euler(0, Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg, 0);
        float pitch = -Mathf.Atan2(d.y, new Vector2(d.x, d.z).magnitude) * Mathf.Rad2Deg;
        typeof(PlayerController).GetField("_pitch", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, pitch);
        player.CameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0); Physics.SyncTransforms();
    }
    static IEnumerator Capture(string name)
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
    public IEnumerator S10b_ArtAudioEffectsLayoutAndGameViewCaptures()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        Directory.CreateDirectory(Output);
        GameManager.Instance.StartGame();
        var audio = Object.FindFirstObjectByType<AudioManager>();
        var entries = (AudioManager.SfxEntry[])typeof(AudioManager).GetField("clips", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(audio);
        Assert.AreEqual(Enum.GetValues(typeof(SfxId)).Length, entries.Length);
        foreach (SfxId id in Enum.GetValues(typeof(SfxId))) { Assert.AreEqual(1, entries.Count(e => e.id == id && e.clip != null), id.ToString()); AudioManager.Play(id); }
        foreach (var ps in Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None)) Assert.NotNull(ps.GetComponent<ParticleSystemRenderer>().sharedMaterial, ps.name);
        foreach (var glow in Object.FindObjectsByType<GlowFeedback>(FindObjectsSortMode.None)) Assert.True(glow.GetComponent<Renderer>().sharedMaterial.IsKeywordEnabled("_EMISSION"), glow.name + " persisted emission keyword");
        Assert.True(GameObject.Find("Art_Decor").GetComponentsInChildren<Collider>().Length == 0);
        View(new Vector3(0, 0, -4.65f), new Vector3(0, 1.2f, 1)); yield return Capture("01-classroom");
        View(new Vector3(-1.35f,0,-.1f),new Vector3(-5.8f,1.7f,0));yield return Capture("02-painting-wall");
        Assert.True(GameObject.Find("Art_SecondPass").GetComponentsInChildren<Collider>().Length==0);
        foreach(string roman in new[]{"I","II","III","IV"}) {
            var plate=GameObject.Find("PaintingLabel_"+roman).GetComponent<TMPro.TMP_Text>();plate.ForceMeshUpdate();
            Assert.AreEqual(roman,plate.text);Assert.Greater(plate.textInfo.characterCount,0);Assert.Greater(plate.textInfo.meshInfo[0].vertexCount,0,"Roman plate has visible glyph mesh");
        }
        Assert.True(GameObject.Find("ArtB_Fit_Cauldron").GetComponentsInChildren<MeshRenderer>().Length>0);
        foreach(var r in GameObject.Find("Room").GetComponentsInChildren<MeshRenderer>()) Assert.NotNull(r.sharedMaterial.mainTexture,r.name);
        foreach(var r in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None)) Assert.NotNull(r.transform.Find("ArtB_RuneIcon_"+r.Rune));
        View(new Vector3(-1.5f, 0, -1), new Vector3(0, 1, .5f));
        foreach (var rune in RunePuzzleLogic.DefaultAnswer) Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).First(s => s.Rune == rune).Interact(Scenario.Player);
        yield return new WaitForSeconds(.65f); yield return Capture("03-altar");
        yield return new WaitForSeconds(2.2f); Scenario.Collect(ItemIds.CrystalSapphire);
        View(new Vector3(1.4f, 0, 1.4f), new Vector3(3, 1.1f, 2.5f));
        Scenario.Collect(ItemIds.Toadstool); Scenario.Select(ItemIds.Toadstool); Object.FindFirstObjectByType<PotionPuzzle>().Interact(Scenario.Player);
        yield return new WaitForSeconds(.25f); yield return Capture("04-cauldron");
        yield return Scenario.Potion();
        View(new Vector3(-3, 0, 1.5f), new Vector3(-3, 1.4f, 3.7f));
        Scenario.Select(ItemIds.PotionRevelation); Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player); UIManager.Instance.CloseRead();
        var reveal=Object.FindFirstObjectByType<SealInscription>().transform.Find("RevealedText");
        var sentence=reveal.Find("InscriptionText").GetComponent<TMPro.TMP_Text>();sentence.ForceMeshUpdate();
        Assert.AreEqual(GameText.InscriptionRevealed,sentence.text);
        Assert.LessOrEqual(sentence.textBounds.size.x,sentence.rectTransform.rect.width+.01f,"Inscription glyphs fit plaque width");
        Assert.LessOrEqual(sentence.textBounds.size.y,sentence.rectTransform.rect.height+.01f,"Inscription glyphs fit plaque height");
        Assert.LessOrEqual(sentence.preferredHeight,sentence.rectTransform.rect.height+.01f,"Inscription fits its box without ellipsis");
        var sentenceBottom=sentence.transform.position.y-sentence.rectTransform.rect.height/2;
        foreach(string name in new[]{"Water","Fire"}) {
            var icon=reveal.Find("ArtB_InscriptionIcon_"+name).GetComponent<Renderer>();
            Assert.Less(icon.bounds.max.y,sentenceBottom-.05f,"Runes are below the sentence");
        }
        var prompt=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="PromptText" && t.gameObject.scene.IsValid());
        Assert.AreNotEqual(RenderMode.WorldSpace,prompt.GetComponentInParent<Canvas>().renderMode);
        Assert.LessOrEqual(prompt.rectTransform.anchoredPosition.y,-280);
        View(new Vector3(-3,0,1.4f),new Vector3(-3,1.65f,3.7f));
        yield return new WaitForSeconds(2);yield return Capture("05-inscription-revealed");
        foreach (var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None)) { Scenario.Select(socket.Side == SocketSide.Left ? ItemIds.CrystalSapphire : ItemIds.CrystalRuby); socket.Interact(Scenario.Player); }
        Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);
        yield return new WaitForSeconds(3.8f);
        View(new Vector3(0,0,2.5f),new Vector3(0,1.6f,6));yield return Capture("06-exit");
    }
}


