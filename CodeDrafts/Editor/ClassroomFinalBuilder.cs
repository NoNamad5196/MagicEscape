using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static void Lamp(string name,Vector3 pos,Color color,float intensity,float range,LightType type=LightType.Point,float angle=45)
        { var l=Component<Light>(name); l.transform.position=pos; l.type=type; l.color=color; l.intensity=intensity; l.range=range; l.spotAngle=angle; if(type==LightType.Spot) l.transform.rotation=Quaternion.Euler(90,0,0); }
        public static void S9()
        {
            S8();
            Lamp("Lamp_Desk",new Vector3(-3.5f,1.4f,-3),new Color(1,.706f,.353f),2,4);
            Lamp("Lamp_Shelf_1",new Vector3(5.4f,2.6f,0),new Color(1,.706f,.353f),1.5f,4);
            Lamp("Lamp_Shelf_2",new Vector3(5.4f,2.6f,2.5f),new Color(1,.706f,.353f),1.5f,4);
            Lamp("Fire_Cauldron",new Vector3(3,.3f,2.5f),new Color(1,.478f,.165f),1.5f,3);
            Lamp("Spot_Altar",new Vector3(0,4.2f,.5f),Color.white,3,6,LightType.Spot,45);
            Lamp("Spot_Seal",new Vector3(0,4.2f,3.6f),new Color(.784f,.706f,1),2,6,LightType.Spot,50);
            const string path="Assets/Settings/ClassroomVolume.asset"; var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if(profile==null)
            {
                profile=ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile,path);
                var bloom=profile.Add<Bloom>(true); bloom.threshold.value=.9f; bloom.intensity.value=1.2f; bloom.scatter.value=.6f;
                var vignette=profile.Add<Vignette>(true); vignette.intensity.value=.25f;
                var tone=profile.Add<Tonemapping>(true); tone.mode.value=TonemappingMode.ACES;
                var adjust=profile.Add<ColorAdjustments>(true); adjust.saturation.value=5;
                foreach(var c in profile.components) AssetDatabase.AddObjectToAsset(c,profile); EditorUtility.SetDirty(profile);
            }
            var volume=Component<Volume>("GlobalVolume"); volume.isGlobal=true; volume.sharedProfile=profile;
            Directory.CreateDirectory("Assets/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(Find("Player"),"Assets/Prefabs/PF_Player.prefab");
            PrefabUtility.SaveAsPrefabAsset(Find("RuneStone_Light"),"Assets/Prefabs/PF_RuneStone.prefab");
            PrefabUtility.SaveAsPrefabAsset(Find("Ingredient_Moonpetal"),"Assets/Prefabs/PF_ItemPickup.prefab");
            PrefabUtility.SaveAsPrefabAsset(Find("Painting_I"),"Assets/Prefabs/PF_Painting.prefab");
            PrefabUtility.SaveAsPrefabAsset(Find("Socket_Left"),"Assets/Prefabs/PF_CrystalSocket.prefab");
            PrefabUtility.SaveAsPrefabAsset(Find("SealLight_1"),"Assets/Prefabs/PF_SealLight.prefab");
            Save();
            var objects=SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Transform>(true)).Select(x=>Hierarchy(x)+"|"+x.gameObject.activeSelf+"|"+x.localPosition.ToString("F3")+"|"+x.localScale.ToString("F3")+"|"+string.Join(",",x.GetComponents<Component>().Select(c=>c.GetType().Name))).OrderBy(x=>x);
            File.WriteAllLines("../docs/validation/s9-scene-manifest.txt",objects);
        }
        static string Hierarchy(Transform t)=>t.parent==null?t.name:Hierarchy(t.parent)+"/"+t.name;
    }
}
