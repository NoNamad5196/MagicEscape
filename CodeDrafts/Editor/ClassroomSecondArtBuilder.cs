using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TMPro;
using MagicEscape.Core;
using Object = UnityEngine.Object;

namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static Material Surface(string name, string file, Color tint)
        {
            var m = ArtMaterial("S10b_" + name, tint);
            m.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
            m.mainTextureScale = Vector2.one;
            EditorUtility.SetDirty(m); return m;
        }
        // Cube UVs in metres. Every face keeps the same texel density despite nonuniform scale.
        static void Tiled(GameObject go, Material mat, float metres = 2)
        {
            var f = go.GetComponent<MeshFilter>();
            string path = "Assets/Settings/S10b_UV_" + go.name + ".asset";
            var mesh = Object.Instantiate(f.sharedMesh); mesh.name = "Metric UV " + go.name;
            var v = mesh.vertices; var n = mesh.normals; var uv = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                var w = Vector3.Scale(v[i], go.transform.lossyScale);
                uv[i] = Mathf.Abs(n[i].y) > .5f ? new Vector2(w.x,w.z)/metres : Mathf.Abs(n[i].x) > .5f ? new Vector2(w.z,w.y)/metres : new Vector2(w.x,w.y)/metres;
            }
            mesh.uv = uv;
            var old = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (old == null) { AssetDatabase.CreateAsset(mesh,path); old=mesh; }
            else { EditorUtility.CopySerialized(mesh,old); Object.DestroyImmediate(mesh); }
            f.sharedMesh=old; go.GetComponent<Renderer>().sharedMaterial=mat;
        }
        static GameObject Visual(string name, Transform parent, Vector3 position, Vector3 size, Material mat, PrimitiveType primitive=PrimitiveType.Cube)
        {
            var g=GameObject.CreatePrimitive(primitive); g.name="ArtB_"+name;
            Object.DestroyImmediate(g.GetComponent<Collider>()); g.transform.SetParent(parent,false);
            g.transform.localPosition=position; g.transform.localScale=size; g.GetComponent<Renderer>().sharedMaterial=mat; return g;
        }
        static void FinishRoom(Transform decor)
        {
            var stone=Surface("Masonry","Assets/ThirdParty/PolyHaven/stone_wall_02.jpg",new Color(.7f,.75f,.85f));
            var timber=Surface("Timber","Assets/ThirdParty/PolyHaven/wood_floor.jpg",new Color(.42f,.3f,.19f));
            foreach(var r in Find("Room").GetComponentsInChildren<MeshRenderer>())
                Tiled(r.gameObject,r.name.Contains("Floor")?timber:r.name=="Ceiling"?timber:stone);
            for(int z=-4;z<=4;z+=2)
            {
                var beam=Visual("CeilingBeam_"+z,decor,new Vector3(0,4.28f,z),new Vector3(12,.3f,.23f),timber); Tiled(beam,timber);
            }
            for(int x=-1;x<=1;x+=2) { var b=Visual("Cornice_"+x,decor,new Vector3(x*5.82f,3.8f,0),new Vector3(.22f,.2f,10),timber);Tiled(b,timber); }
            RenderSettings.fog=true; RenderSettings.fogMode=FogMode.ExponentialSquared; RenderSettings.fogDensity=.018f;
            RenderSettings.fogColor=new Color(.055f,.07f,.12f);
            RenderSettings.ambientLight=new Color(.23f,.24f,.3f);
        }
        static Transform ModelAt(string pack,string model,Transform parent,Vector3 bottom,Vector3 size,float yaw=0)
        {
            var src=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/"+pack+"/"+model+".fbx");
            if(src==null) throw new InvalidOperationException("Missing selected CC0 model: "+model);
            var root=new GameObject("ArtB_"+model).transform;
            Action<Transform,Transform> copy=null;
            copy=(a,b)=> {
                var c=new GameObject(a.name).transform; c.SetParent(b,false); c.localPosition=a.localPosition; c.localRotation=a.localRotation; c.localScale=a.localScale;
                var f=a.GetComponent<MeshFilter>(); var r=a.GetComponent<MeshRenderer>();
                if(f!=null && r!=null) {
                    c.gameObject.AddComponent<MeshFilter>().sharedMesh=f.sharedMesh;
                    c.gameObject.AddComponent<MeshRenderer>().sharedMaterials=r.sharedMaterials.Select(m=> {
                        if(pack=="KayKitDungeon") return Surface("KayKitB","Assets/ThirdParty/KayKitDungeon/dungeon_texture.png",Color.white);
                        string category=new[]{"Furniture","Metal","Cloth","Props"}.FirstOrDefault(k=>m!=null && m.name.IndexOf(k,StringComparison.OrdinalIgnoreCase)>=0)??"Props";
                        return Surface("Quat_"+category,"Assets/ThirdParty/Quaternius/T_Trim_"+category+"_BaseColor.png",Color.white);
                    }).ToArray();
                }
                foreach(Transform t in a) copy(t,c);
            }; copy(src.transform,root); root.rotation=Quaternion.Euler(0,yaw,0);
            var rs=root.GetComponentsInChildren<Renderer>(); var bounds=rs[0].bounds; foreach(var r in rs) bounds.Encapsulate(r.bounds);
            // Scale in world axes BEFORE parenting, so legacy nonuniform collider transforms do not distort props.
            var scale=new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z);
            var wrapper=new GameObject("ArtB_Fit_"+model).transform; root.SetParent(wrapper,true); wrapper.localScale=scale;
            wrapper.position=bottom-Vector3.Scale(new Vector3(bounds.center.x,bounds.min.y,bounds.center.z),scale);
            wrapper.SetParent(parent,true); return wrapper;
        }
        static void SchoolProps(Transform decor)
        {
            ModelAt("Quaternius","Bookcase_2",decor,new Vector3(5.6f,0,-.7f),new Vector3(.55f,2.7f,1.65f),-90);
            ModelAt("Quaternius","Bookcase_2",decor,new Vector3(5.6f,0,3.5f),new Vector3(.55f,2.7f,1.65f),-90);
            foreach(var r in Find("Shelf").GetComponentsInChildren<Renderer>()) r.enabled=false;
            ModelAt("Quaternius","Shelf_Small_Bottles",Find("Shelf").transform,new Vector3(5.65f,.75f,1.5f),new Vector3(.5f,1.2f,1.2f),-90);
            foreach(var r in Find("CauldronBody").GetComponentsInChildren<Renderer>()) r.enabled=false;
            ModelAt("Quaternius","Cauldron",Find("CauldronBody").transform,new Vector3(3,.02f,2.5f),new Vector3(1.22f,1.08f,1.22f));
            ModelAt("Quaternius","Book_5",Find("RecipeBook").transform,new Vector3(4.2f,1.08f,3.6f),new Vector3(.6f,.13f,.4f)); Find("RecipeBook").GetComponent<Renderer>().enabled=false;
            ModelAt("Quaternius","BookStand",decor,new Vector3(4.2f,0,3.6f),new Vector3(.7f,1.06f,.5f));
            ModelAt("Quaternius","Book_Stack_1",decor,new Vector3(-3.75f,.73f,-3.35f),new Vector3(.38f,.25f,.28f));
            ModelAt("Quaternius","Scroll_1",decor,new Vector3(-3.2f,.72f,-3.4f),new Vector3(.32f,.08f,.22f));
            for(int i=0;i<3;i++) {
                float x=-1.7f+i*1.8f;
                ModelAt("KayKitDungeon","table_small",decor,new Vector3(x,0,-3.7f),new Vector3(1.1f,.72f,.65f));
                ModelAt("KayKitDungeon","chair",decor,new Vector3(x,0,-4.4f),new Vector3(.5f,.85f,.5f));
                ModelAt("Quaternius","BookGroup_Medium_1",decor,new Vector3(x,.73f,-3.7f),new Vector3(.35f,.17f,.25f));
            }
            for(int x=-1;x<=1;x+=2) {
                ModelAt("KayKitDungeon","pillar_decorated",decor,new Vector3(x*5.65f,0,4.65f),new Vector3(.45f,3.9f,.45f));
                ModelAt("KayKitDungeon","banner_patternA_blue",decor,new Vector3(x*2.5f,2.3f,4.74f),new Vector3(.75f,1.45f,.12f));
                ModelAt("Quaternius","Lantern_Wall",decor,new Vector3(x*4.5f,2.5f,4.6f),new Vector3(.3f,.6f,.35f));
            }
            var wood=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            foreach(string name in new[]{"Windowsill","PlantPot"}) Tiled(Find(name),wood,.5f);
            for(int i=-1;i<=1;i++) Visual("WindowMullion_"+i,decor,new Vector3(5+i*.68f,1.7f,-4.8f),new Vector3(.055f,1.5f,.08f),wood);
            for(int i=-1;i<=1;i+=2) Visual("WindowRail_"+i,decor,new Vector3(5,1.7f+i*.72f,-4.8f),new Vector3(1.5f,.065f,.08f),wood);
        }
        public static void S10b()
        {
            S10(); var before=ColliderManifest(); var decor=new GameObject("Art_SecondPass").transform;
            FinishRoom(decor); SchoolProps(decor);
            if(!before.SequenceEqual(ColliderManifest())) throw new InvalidOperationException("S10b changed collider/layer/world transforms");
            Directory.CreateDirectory("../docs/validation/s10b");
            File.WriteAllText("../docs/validation/s10b/builder-audit.txt","S10b reconstructed from S10. Collider JSON, world transforms/scales/layers unchanged. Added visuals have no colliders.\n");
            Save();
        }
    }
}
