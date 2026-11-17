using System.Collections.Generic;
using System.Linq;
using System.IO;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
 public static partial class ClassroomBuilder
 {
  // Only visual meshes/materials. The original interaction colliders and homes stay authoritative.
  static Mesh WarehouseBevelMesh()
  {
   const string path="Assets/Art/Warehouse/WornSlab.asset";
   var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);bool fresh=mesh==null;
   if(fresh)mesh=new Mesh{name="Hand dressed chamfered stone"};else mesh.Clear();
   var v=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
   var outline=new[]{new Vector2(-.42f,-.5f),new Vector2(.42f,-.5f),new Vector2(.5f,-.42f),new Vector2(.5f,.42f),new Vector2(.42f,.5f),new Vector2(-.42f,.5f),new Vector2(-.5f,.42f),new Vector2(-.5f,-.42f)};
   // Clockwise winding viewed from above; separate faces retain honest bevel normals.
   void Tri(Vector3 a,Vector3 b,Vector3 c){int k=v.Count;v.AddRange(new[]{a,b,c});var normal=Vector3.Cross(b-a,c-a).normalized;
    Vector2 U(Vector3 p){return Mathf.Abs(normal.y)>.6f?new Vector2(p.x+.5f,p.z+.5f):Mathf.Abs(normal.x)>.6f?new Vector2(p.z+.5f,p.y+.5f):new Vector2(p.x+.5f,p.y+.5f);}
    uv.AddRange(new[]{U(a),U(b),U(c)});indices.AddRange(new[]{k,k+1,k+2});}
   Vector3 P(int n,float y,float scale){var p=outline[n%8]*scale;return new Vector3(p.x,y,p.y);}
   for(int n=0;n<8;n++)
   {
    Tri(new Vector3(0,.5f,0),P(n+1,.5f,.94f),P(n,.5f,.94f));
    foreach(int ring in new[]{0,1})
    {
     float ya=ring==0?-.5f:.15f,yb=ring==0?.15f:.5f,sa=1,sb=ring==0?1:.94f;
     var a=P(n,ya,sa);var b=P(n+1,ya,sa);var c=P(n+1,yb,sb);var d=P(n,yb,sb);
     Tri(a,c,b);Tri(a,d,c);
    }
    Tri(new Vector3(0,-.5f,0),P(n,-.5f,1),P(n+1,-.5f,1));
   }
   mesh.SetVertices(v);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
   if(fresh)AssetDatabase.CreateAsset(mesh,path);else EditorUtility.SetDirty(mesh);return mesh;
  }
  static Material WarehouseStone()
  {
   const string path="Assets/Art/Warehouse/DressedStone.png";
   var tex=new Texture2D(256,256,TextureFormat.RGB24,false);var colors=new Color[256*256];
   for(int y=0;y<256;y++)for(int x=0;x<256;x++)
   {
    float cloud=Mathf.PerlinNoise(x*.036f,y*.036f),grain=Mathf.PerlinNoise(x*.81f+11,y*.81f+7);
    float vein=Mathf.SmoothStep(.52f,.55f,Mathf.PerlinNoise(x*.014f,y*.018f))* .025f;
    colors[y*256+x]=new Color(.27f,.31f,.29f)*( .80f+cloud*.35f+grain*.10f)-new Color(vein,vein,vein);
   }
   tex.SetPixels(colors);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());Object.DestroyImmediate(tex);
   AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
   var mat=ArtMaterial("WarehouseDressedStone",Color.white);mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
   mat.SetFloat("_Metallic",0);mat.SetFloat("_Smoothness",.12f);EditorUtility.SetDirty(mat);return mat;
  }
  static Material WarehouseCarving(Material source,string key)
  {
   var mat=ArtMaterial("WarehouseCarving_"+key,Color.white);
   mat.shader=Shader.Find("MagicEscape/WarehouseIncision");mat.SetTexture("_BaseMap",source.GetTexture("_BaseMap"));
   mat.SetColor("_BaseColor",new Color(.68f,.70f,.60f));mat.SetColor("_EmissionColor",source.GetColor("_EmissionColor"));
   EditorUtility.SetDirty(mat);return mat;
  }
  static void ConfigureWarehouseStage12()
  {
   var stoneMaterial=WarehouseStone();var bevel=WarehouseBevelMesh();
   foreach(var r in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
   {
    if(r.sharedMaterial!=null&&r.sharedMaterial.shader.name=="MagicEscape/AlchemyEngraving")
     r.sharedMaterial=WarehouseCarving(r.sharedMaterial,r.sharedMaterial.name);
    if(r.name.StartsWith("RuneName_"))
    {
     var id=r.name.Substring("RuneName_".Length);
     r.sharedMaterial.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Warehouse/RuneName_"+id+".png"));
     EditorUtility.SetDirty(r.sharedMaterial);
    }
   }
   foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
   {
    var slab=stone.transform.Find("AlchemicalSlab");slab.GetComponent<MeshFilter>().sharedMesh=bevel;slab.GetComponent<Renderer>().sharedMaterial=stoneMaterial;
    foreach(var mark in new[]{stone.transform.Find("ArtB_RuneIcon_"+stone.Rune),stone.transform.Find("RuneName_"+stone.Rune)})
    {var p=mark.position;p.y=slab.position.y+.0288f;mark.position=p;}
   }
   var chalk=Find("ChalkSequence").GetComponent<Renderer>().sharedMaterial;
   chalk.shader=Shader.Find("MagicEscape/WarehouseChalk");chalk.SetColor("_BaseColor",new Color(.84f,.87f,.81f));EditorUtility.SetDirty(chalk);
   // Neutral dedicated frame light keeps cool paper pigments from becoming orange under the candles.
   foreach(string roman in new[]{"I","II","III","IV"})
   {
    var frame=Find("ArtB_PictureFrame_"+roman);
    foreach(var lamp in frame.GetComponentsInChildren<Light>(true)){lamp.color=new Color(.64f,.75f,.84f);lamp.intensity*=.55f;}
    var picture=frame.transform.Find("ArtB_StoryIllustration").GetComponent<Renderer>().sharedMaterial;
    picture.shader=Shader.Find("MagicEscape/WarehouseIllustration");picture.SetColor("_BaseColor",Color.white);EditorUtility.SetDirty(picture);
   }
   // No normal-map weave. Only long analytic surface motion and bounded puzzle glow.
   var liquid=Find("Liquid");var brew=ArtMaterial("WarehouseBrew",new Color(.018f,.046f,.04f));
   brew.shader=Shader.Find("MagicEscape/WarehouseBrew");brew.SetColor("_BaseColor",new Color(.018f,.046f,.04f));brew.SetColor("_EmissionColor",Color.black);
   liquid.GetComponent<Renderer>().sharedMaterial=brew;EditorUtility.SetDirty(brew);
   Set(liquid.GetComponent<GlowFeedback>(),"affectBaseColor",true);Set(liquid.GetComponent<GlowFeedback>(),"intensity",.22f);
   // These short colours remain subordinate to physical reward appearance, then darkening attenuates them.
   Set(SceneComponent<PotionPuzzle>(),"solvedColor",new Color(.22f,.18f,.07f));
   // Moonlight is a background contributor, not a permanent beacon or a second darkening writer.
   var dark=SceneComponent<WarehouseDarkening>();var moon=new GameObject("WarehouseWindowMoonlight").AddComponent<Light>();
   moon.type=LightType.Spot;moon.color=new Color(.30f,.51f,.72f);moon.intensity=18f;moon.range=7;moon.spotAngle=84;moon.innerSpotAngle=35;
   moon.transform.position=new Vector3(4.15f,2.30f,-2.90f);moon.transform.LookAt(new Vector3(.3f,.4f,.2f));moon.shadows=LightShadows.None;
   // A low indirect bounce puts the window's cool light on the aisle, not only on the far wall.
   // Nearer source also avoids losing the distant spot to forward per-object light culling.
   var bounce=new GameObject("WarehouseMoonBounce").AddComponent<Light>();bounce.type=LightType.Point;
   bounce.transform.position=new Vector3(1.55f,1.15f,-1.65f);bounce.color=new Color(.24f,.46f,.70f);
   bounce.intensity=1.8f;bounce.range=3.8f;bounce.shadows=LightShadows.None;bounce.renderMode=LightRenderMode.ForcePixel;
   dark.backgroundLights=dark.backgroundLights.Concat(new[]{moon,bounce}).ToArray();
   // Reuse existing shapes; slate-green ceramics are dressing, never a puzzle colour code.
   var ceramic=ArtMaterial("WarehouseGlazedSage",new Color(.16f,.29f,.27f));ceramic.SetFloat("_Smoothness",.33f);EditorUtility.SetDirty(ceramic);
   foreach(var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(r=>r.name=="ArtB_WorkbenchDecorJar"||r.name=="ArtB_WorkbenchDecorJarNeck"))r.sharedMaterial=ceramic;
   // The seal tablet and pedestal are dressed stone, not raw primitive boxes.
   foreach(string name in new[]{"Inscription","Pedestal"})
   {
    var go=Find(name);var filter=go.GetComponent<MeshFilter>();if(filter!=null){filter.sharedMesh=bevel;go.GetComponent<Renderer>().sharedMaterial=stoneMaterial;}
   }
   foreach(var r in Find("AltarBase").GetComponentsInChildren<Renderer>().Where(r=>r.name.StartsWith("ArtB_AltarStone_")))
   {r.GetComponent<MeshFilter>().sharedMesh=bevel;r.sharedMaterial=stoneMaterial;}
   // Add a restrained fitted timber skirt and iron corner straps to the conspicuous chest support.
   var plinth=Find("WarehouseChestPlinth");var bounds=plinth.GetComponent<Renderer>().bounds;
   var timber=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
   var iron=ArtMaterial("WarehouseWornStrap",new Color(.11f,.14f,.14f));iron.SetFloat("_Metallic",.45f);iron.SetFloat("_Smoothness",.18f);EditorUtility.SetDirty(iron);
   foreach(int side in new[]{-1,1})
   {
    WorldVisual("ChestPlinthSkirt",plinth.transform,new Vector3(bounds.center.x,bounds.min.y+.055f,bounds.center.z+side*(bounds.extents.z+.008f)),new Vector3(bounds.size.x,.10f,.028f),timber);
    foreach(int edge in new[]{-1,1})WorldVisual("ChestPlinthStrap",plinth.transform,new Vector3(bounds.center.x+edge*(bounds.extents.x-.08f),bounds.center.y,bounds.center.z+side*(bounds.extents.z+.003f)),new Vector3(.045f,bounds.size.y,.012f),iron);
   }
  }
 }
}
