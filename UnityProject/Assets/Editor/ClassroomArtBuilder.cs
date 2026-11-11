using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static Material ArtMaterial(string name, Color color, float emission = 0)
        {
            string path = "Assets/Settings/Art_" + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) { mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(mat, path); }
            mat.color = color; mat.SetFloat("_Smoothness", .18f);
            mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", color * emission);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            MaterialEditor.FixupEmissiveFlag(mat);
            EditorUtility.SetDirty(mat); return mat;
        }

        // Mesh-only copies: never import colliders, scripts, layers or references from a model.
        static bool ArtModel(string model, Transform parent, Vector3 bottom, Vector3 size, bool replace = false)
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/KayKitDungeon/" + model + ".fbx");
            if (source == null) return false; // Original graybox stays usable without optional packages.
            var root = new GameObject("Art_" + model);
            foreach (var filter in source.GetComponentsInChildren<MeshFilter>(true))
            {
                var child = new GameObject(filter.name, typeof(MeshFilter), typeof(MeshRenderer)); child.transform.SetParent(root.transform, false);
                child.transform.localPosition = filter.transform.position - source.transform.position;
                child.transform.localRotation = filter.transform.rotation;
                child.transform.localScale = filter.transform.lossyScale;
                child.GetComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
                var mat = ArtMaterial("KayKit", Color.white);
                mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ThirdParty/KayKitDungeon/dungeon_texture.png");
                child.GetComponent<MeshRenderer>().sharedMaterial = mat;
            }
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { UnityEngine.Object.DestroyImmediate(root); return false; }
            var bounds = renderers[0].bounds; foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            var scale = new Vector3(size.x / Mathf.Max(bounds.size.x, .001f), size.y / Mathf.Max(bounds.size.y, .001f), size.z / Mathf.Max(bounds.size.z, .001f));
            // Replacement furniture must support the unchanged pickup height. Decorations retain proportions.
            float fit = Mathf.Min(scale.x, scale.y, scale.z); root.transform.SetParent(parent, false); root.transform.localScale = replace ? scale : Vector3.one * fit;
            root.transform.localPosition = bottom - Vector3.Scale(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), root.transform.localScale);
            if (replace && parent.GetComponent<Renderer>() != null) parent.GetComponent<Renderer>().enabled = false;
            return true;
        }

        static string[] ColliderManifest() => UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Select(c => Hierarchy(c.transform) + "|" + EditorJsonUtility.ToJson(c) + "|" + c.transform.position.ToString("F4") + "|" + c.transform.lossyScale.ToString("F4") + "|" + c.gameObject.layer).OrderBy(x => x).ToArray();

        static Material ArtParticleMaterial(bool smoke)
        {
            string name = smoke ? "Smoke" : "Spark";
            string path = "Assets/Settings/Art_" + name + "Texture.asset";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
            {
                texture = new Texture2D(32, 32, TextureFormat.RGBA32, false); texture.wrapMode = TextureWrapMode.Clamp;
                for (int y = 0; y < 32; y++) for (int x = 0; x < 32; x++)
                {
                    var p = new Vector2((x - 15.5f) / 15.5f, (y - 15.5f) / 15.5f);
                    float radius = smoke ? p.magnitude : Mathf.Abs(p.x) + Mathf.Abs(p.y);
                    texture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Pow(Mathf.Clamp01(1 - radius), smoke ? 1.5f : .7f)));
                }
                texture.Apply(); AssetDatabase.CreateAsset(texture, path);
            }
            path = "Assets/Settings/Art_" + name + "Particles.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")); AssetDatabase.CreateAsset(material, path); }
            material.SetTexture("_BaseMap", texture); material.SetFloat("_Surface", 1); material.SetFloat("_Blend", 0);
            material.SetFloat("_SrcBlend", 5); material.SetFloat("_DstBlend", 10); material.SetFloat("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); material.renderQueue = 3000; EditorUtility.SetDirty(material);
            return material;
        }

        static Mesh CauldronMesh()
        {
            const string path = "Assets/Settings/Art_Cauldron.asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path); if (mesh != null) return mesh;
            var profile = new[] {new Vector2(.22f, -1), new Vector2(.43f, -.7f), new Vector2(.52f, 0), new Vector2(.46f, .8f), new Vector2(.5f, .9f), new Vector2(.5f, 1), new Vector2(.4f, 1), new Vector2(.4f, .7f)};
            const int sides = 32; var vertices = new Vector3[profile.Length * sides]; var triangles = new int[(profile.Length - 1) * sides * 6]; int t = 0;
            for (int i = 0; i < profile.Length; i++) for (int j = 0; j < sides; j++)
            {
                float a = j * Mathf.PI * 2 / sides; vertices[i * sides + j] = new Vector3(Mathf.Sin(a) * profile[i].x, profile[i].y, Mathf.Cos(a) * profile[i].x);
                if (i == profile.Length - 1) continue;
                int v = i * sides + j, next = i * sides + (j + 1) % sides;
                triangles[t++] = v; triangles[t++] = next; triangles[t++] = v + sides;
                triangles[t++] = next; triangles[t++] = next + sides; triangles[t++] = v + sides;
            }
            mesh = new Mesh { name = "MagicEscape authored cauldron", vertices = vertices, triangles = triangles }; mesh.RecalculateNormals(); mesh.RecalculateBounds(); AssetDatabase.CreateAsset(mesh, path); return mesh;
        }

        public static void S10()
        {
            S9(); var before = ColliderManifest();
            var wood = ArtMaterial("Wood", new Color(.18f, .095f, .045f));
            var stone = ArtMaterial("Stone", new Color(.23f, .25f, .30f));
            foreach (var r in Find("Room").GetComponentsInChildren<Renderer>()) r.sharedMaterial = r.name == "Floor" ? wood : stone;
            Find("DoorPanel").GetComponent<Renderer>().sharedMaterial = wood;
            Find("CauldronBody").GetComponent<MeshFilter>().sharedMesh = CauldronMesh();
            Find("CauldronBody").GetComponent<Renderer>().sharedMaterial = ArtMaterial("Iron", new Color(.055f, .065f, .085f));
            // URP's importer clears _EMISSION for a black serialized emission color.
            // Keep a tiny non-black material default; GlowFeedback's property block owns runtime emission.
            foreach (var glow in UnityEngine.Object.FindObjectsByType<GlowFeedback>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var renderer = glow.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var color = renderer.sharedMaterial.color; renderer.sharedMaterial = ArtMaterial("Reactive_" + ColorUtility.ToHtmlStringRGB(color), color, .01f);
                    // Keep the white inscription legible instead of clipping its whole slab to white.
                    Set(glow, "intensity", glow.name == "InscriptionSlab" ? .08f : .8f);
                }
            }
            foreach (var text in UnityEngine.Object.FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                text.enableAutoSizing = true; text.fontSizeMin = .1f; text.fontSizeMax = text.fontSize; text.overflowMode = TextOverflowModes.Ellipsis;
            }
            ArtModel("table_medium", Find("Drawer").transform, new Vector3(0, -.5f, 0), Vector3.one, true);
            ArtModel("shelf_small", Find("Shelf").transform, new Vector3(0, .35f, 0), new Vector3(1, .15f, 1), true);
            var decor = new GameObject("Art_Decor").transform;
            ArtModel("chair", decor, new Vector3(-4.9f, 0, -4.1f), new Vector3(.6f, 1, .6f));
            ArtModel("candle_triple", decor, new Vector3(-3.5f, .72f, -3.3f), new Vector3(.35f, .55f, .3f));
            ArtModel("candle_lit", decor, new Vector3(5.7f, 1.3f, 1.8f), new Vector3(.2f, .45f, .2f));
            ArtModel("candle_lit", decor, new Vector3(-3.7f, 1.01f, 3.7f), new Vector3(.15f, .32f, .15f));
            Lamp("Art_WindowBeam", new Vector3(5, 2.3f, -4.6f), new Color(.38f, .55f, 1), 4, 8, LightType.Spot, 55);
            Find("Art_WindowBeam").transform.rotation = Quaternion.Euler(25, -35, 0);
            Find("Window_SE").GetComponent<Renderer>().sharedMaterial = ArtMaterial("MoonGlass", new Color(.18f, .3f, .65f), 2);
            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None)) light.shadows = LightShadows.None;
            Find("Spot_Seal").transform.position = new Vector3(-3, 4.2f, 3.6f);
            foreach (var r in Find("ExitDoor").GetComponentsInChildren<Renderer>())
                if (r.name.StartsWith("SealLight_")) r.sharedMaterial = ArtMaterial("SealRed", new Color(1, .13f, .08f), 3);
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/ClassroomVolume.asset");
            if (profile.TryGet<Bloom>(out var bloom)) { bloom.intensity.Override(1.2f); bloom.threshold.Override(.9f); }
            if (profile.TryGet<Tonemapping>(out var tone)) tone.mode.Override(TonemappingMode.ACES);
            if (profile.TryGet<Vignette>(out var vignette)) vignette.intensity.Override(.18f);
            EditorUtility.SetDirty(profile);
            foreach (var ps in UnityEngine.Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
            {
                var main = ps.main; bool smoke = ps.name.Contains("Smoke");
                ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = ArtParticleMaterial(smoke);
                main.startSize = smoke ? .28f : .055f; main.startLifetime = smoke ? 1.8f : 1.1f;
                main.startSpeed = smoke ? .3f : .75f;
                var velocity = ps.velocityOverLifetime; velocity.enabled = true; velocity.space = ParticleSystemSimulationSpace.Local; velocity.y = smoke ? .6f : .2f;
                var size = ps.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, smoke ? .3f : 1, 1, smoke ? 1.7f : 0));
                var color = ps.colorOverLifetime; color.enabled = true;
                var gradient = new Gradient(); gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] {new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1)}); color.color = gradient;
            }
            string[] clips = {"click_001", "confirmation_001", "click_002", "error_001", "confirmation_002", "click_002", "error_002", "confirmation_003", "maximize_001", "error_003", "open_001", "confirmation_004", "glass_001", "confirmation_004", "click_001"};
            // Every SfxId is explicit; a missing optional CC0 clip safely remains null.
            Set(SceneComponent<AudioManager>(), "clips", Enum.GetValues(typeof(SfxId)).Cast<SfxId>().Select((id, i) => new AudioManager.SfxEntry {
                id = id, clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/KenneyInterface/" + clips[i] + ".ogg"), volume = .5f
            }).ToArray());
            if (!before.SequenceEqual(ColliderManifest())) throw new InvalidOperationException("S10 changed gameplay colliders/layers/transforms");
            Directory.CreateDirectory("../docs/validation/s10");
            File.WriteAllText("../docs/validation/s10/builder-audit.txt", "S10 reconstructed from S9. Existing Collider serialization/world transforms/layers unchanged. Mesh-only imports, no model scripts/colliders.\n");
            File.WriteAllLines("../docs/validation/s10/scene-manifest.txt", UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).Select(t => Hierarchy(t) + "|" + t.localPosition.ToString("F3") + "|" + t.localRotation.eulerAngles.ToString("F3") + "|" + t.localScale.ToString("F3") + "|" + string.Join(",", t.GetComponents<UnityEngine.Component>().Select(c => c.GetType().Name))).OrderBy(x => x));
            Save();
        }
    }
}

