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
        public static void S10b()
        {
            S10(); var before=ColliderManifest(); var decor=new GameObject("Art_SecondPass").transform;
            FinishRoom(decor);
            if(!before.SequenceEqual(ColliderManifest())) throw new InvalidOperationException("S10b changed collider/layer/world transforms");
            Directory.CreateDirectory("../docs/validation/s10b");
            File.WriteAllText("../docs/validation/s10b/builder-audit.txt","S10b reconstructed from S10. Collider JSON, world transforms/scales/layers unchanged. Added visuals have no colliders.\n");
            Save();
        }
    }
}
