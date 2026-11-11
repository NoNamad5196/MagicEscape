using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace MagicEscape.EditorTools
{
    // Each entry point reconstructs its stage from source, never appends duplicates.
    public static partial class ClassroomBuilder
    {
        public const string ScenePath = "Assets/Scenes/Classroom.unity";
        public static void S1()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory("Assets/Settings");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            var tags = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            tags.FindProperty("layers").GetArrayElementAtIndex(8).stringValue = "Player";
            tags.FindProperty("layers").GetArrayElementAtIndex(9).stringValue = "Interactable";
            tags.ApplyModifiedPropertiesWithoutUndo();
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            settings.FindProperty("activeInputHandler").intValue = 2;
            settings.ApplyModifiedPropertiesWithoutUndo();
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/ClassroomRenderer.asset");
            if (renderer == null) { renderer = ScriptableObject.CreateInstance<UniversalRendererData>(); AssetDatabase.CreateAsset(renderer, "Assets/Settings/ClassroomRenderer.asset"); }
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/ClassroomURP.asset");
            if (pipeline == null) { pipeline = UniversalRenderPipelineAsset.Create(renderer); AssetDatabase.CreateAsset(pipeline, "Assets/Settings/ClassroomURP.asset"); }
            pipeline.supportsCameraDepthTexture = true;
            pipeline.supportsHDR = true;
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            EditorUtility.SetDirty(pipeline);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var room = new GameObject("Room").transform;
            Box("Floor", room, new Vector3(0,-.05f,0), new Vector3(12,.1f,10));
            Box("Ceiling", room, new Vector3(0,4.5f,0), new Vector3(12,.1f,10));
            Box("Wall_W", room, new Vector3(-6,2.25f,0), new Vector3(.2f,4.5f,10));
            Box("Wall_E", room, new Vector3(6,2.25f,0), new Vector3(.2f,4.5f,10));
            Box("Wall_S", room, new Vector3(0,2.25f,-5), new Vector3(12,4.5f,.2f));
            Box("Wall_N_Left", room, new Vector3(-3.825f,2.25f,5), new Vector3(4.35f,4.5f,.2f));
            Box("Wall_N_Right", room, new Vector3(3.825f,2.25f,5), new Vector3(4.35f,4.5f,.2f));
            Box("Wall_N_Top", room, new Vector3(0,3.55f,5), new Vector3(3.3f,1.9f,.2f));
            Box("CorridorFloor", room, new Vector3(0,-.05f,6.5f), new Vector3(3.3f,.1f,3));
            var light = new GameObject("Moonlight").AddComponent<Light>();
            light.type = LightType.Directional; light.color = new Color(.56f,.66f,1); light.intensity = .3f;
            light.transform.rotation = Quaternion.Euler(35,150,0); light.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = AmbientMode.Flat; RenderSettings.ambientLight = new Color(.165f,.145f,.25f);
            Save();
        }
        static GameObject Box(string name, Transform parent, Vector3 position, Vector3 scale)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name;
            go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            go.isStatic = true;
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Graybox.asset");
            if (mat == null) { mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); mat.color = new Color(.45f,.42f,.36f); AssetDatabase.CreateAsset(mat,"Assets/Settings/Graybox.asset"); }
            go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }
        static void Save()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath,true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Classroom builder saved " + ScenePath);
        }
    }
}
