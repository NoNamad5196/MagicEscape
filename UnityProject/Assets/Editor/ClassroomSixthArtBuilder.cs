using System;
using System.IO;
using System.Linq;
using MagicEscape.Core;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
 public static partial class ClassroomBuilder
 {
  const string FArt="Assets/Art/S10f/";
  static Material FWater(string name,Color color)
  {
   var m=EMat("F_"+name,color);m.shader=Shader.Find("MagicEscape/ElementWater");m.SetColor("_BaseColor",color);m.SetColor("_EmissionColor",new Color(.18f,.32f,.42f));m.SetTexture("_NormalMap",AssetDatabase.LoadAssetAtPath<Texture2D>(FArt+"WaterNormal.png"));m.SetFloat("_Flow",1);EditorUtility.SetDirty(m);return m;
  }
  static Material FGlyph(RuneId id)
  {
   var m=EMat("F_Incision_"+id,new Color(.64f,.55f,.37f));m.shader=Shader.Find("MagicEscape/AlchemyEngraving");m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/S10b/"+id+".png"));m.SetColor("_BaseColor",new Color(.64f,.55f,.37f));m.SetColor("_EmissionColor",Color.black);m.EnableKeyword("_EMISSION");EditorUtility.SetDirty(m);return m;
  }
  static void FElement(Transform parent,Vector3 p,RuneId id,float size,bool lights=false)
  {
   if(id==RuneId.Fire)
   {
    var fire=EParticles("MaterialFire",parent,p+Vector3.up*size*.55f,Color.white,"Soft",1,size*2.4f,0,0);
    var main=fire.main;main.startLifetime=1;main.maxParticles=2;main.startSize=new ParticleSystem.MinMaxCurve(size*2.1f,size*2.4f);
    var shape=fire.shape;shape.enabled=false;var v=fire.velocityOverLifetime;v.enabled=false;
    var renderer=fire.GetComponent<ParticleSystemRenderer>();var mat=EMat("F_Fire",Color.white);mat.shader=Shader.Find("MagicEscape/AlchemyFire");mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(FArt+"Flame02.png"));EditorUtility.SetDirty(mat);renderer.sharedMaterial=mat;
    EParticles("FineEmbers",parent,p,new Color(.8f,.22f,.04f),"Soft",4,size*.045f,size*.3f,size*.6f);
    if(lights){var l=new GameObject("FireLight").AddComponent<Light>();l.transform.SetParent(parent,true);l.transform.position=p+Vector3.up*size;l.range=1.3f;l.color=new Color(1,.29f,.07f);l.shadows=LightShadows.None;var flick=l.gameObject.AddComponent<FireFlicker>();flick.source=l;flick.brightness=.3f;}
   }
   else if(id==RuneId.Water)
   {
    var m=FWater("Water",new Color(.015f,.055f,.13f));
    EShape("MaterialWaterPool",parent,p,new Vector3(size*1.7f,.008f,size*1.7f),m,PrimitiveType.Cylinder);
    // Thin UV-scrolling stream, elongated drops and impact rings share the same liquid vocabulary.
    EShape("MaterialWaterStream",parent,p+Vector3.up*size*.6f,new Vector3(size*.10f,size*.6f,size*.10f),m,PrimitiveType.Cylinder);
    var drops=EParticles("WaterDrops",parent,p+Vector3.up*size*1.2f,new Color(.24f,.45f,.65f),"Drop",5,size*.10f,size*.18f,-size*1.4f);
    var r=drops.GetComponent<ParticleSystemRenderer>();r.renderMode=ParticleSystemRenderMode.Stretch;r.lengthScale=2;r.velocityScale=.07f;
    ERing("ImpactRipple",parent,p+Vector3.up*.012f,size*.62f,new Color(.16f,.29f,.37f));
   }
   else
   {
    var c=new Color(.52f,.47f,.34f,.35f);
    var ps=EParticles("IncisionDust_"+id,parent,p,c,"Soft",3,size*.07f,size*.4f,.035f);
    var main=ps.main;main.maxParticles=6;main.startLifetime=.6f;
   }
  }
  public static void S10fApply()
  {
   EditorSceneManager.OpenScene(ScenePath);AssetDatabase.Refresh();Directory.CreateDirectory("../docs/validation/s10f");var before=ColliderManifest();var root=ERoot("Art_SixthPass");
   foreach(string file in new[]{"Flame02","WaterNormal"})
   {
    var imp=(TextureImporter)AssetImporter.GetAtPath(FArt+file+".png");imp.wrapMode=file=="WaterNormal"?TextureWrapMode.Repeat:TextureWrapMode.Clamp;imp.mipmapEnabled=true;imp.maxTextureSize=2048;
    if(file=="WaterNormal")imp.textureType=TextureImporterType.NormalMap;else imp.sRGBTexture=true;
    imp.SaveAndReimport();
   }
   // One incision material for every occurrence: frame, stone, inscription, lamps.
   foreach(RuneId id in Enum.GetValues(typeof(RuneId)))
   {
    var m=FGlyph(id);
    foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(r=>r.name=="ArtB_RuneIcon_"+id||r.name=="ArtB_CornerRune_"+id||r.name=="ArtB_InscriptionIcon_"+id))r.sharedMaterial=m;
   }
   var slate=EMat("F_Slate",new Color(.055f,.052f,.048f));slate.SetFloat("_Metallic",.15f);slate.SetFloat("_Smoothness",.24f);
   foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
   {
    // Do not change the collider, transform or hit distance. Replace visual children only.
    foreach(var t in stone.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("E_Element_")).ToArray())Object.DestroyImmediate(t.gameObject);
    foreach(var r in stone.GetComponentsInChildren<Renderer>(true))r.enabled=false;
    var slab=EShape("AlchemicalSlab",stone.transform,stone.transform.position,new Vector3(.30f,.055f,.30f),slate,PrimitiveType.Cube);
    var icon=stone.transform.Find("ArtB_RuneIcon_"+stone.Rune);icon.SetParent(null,true);icon.position=stone.transform.position+Vector3.up*.039f;icon.rotation=Quaternion.Euler(90,0,0);icon.localScale=new Vector3(.28f,.28f,1);icon.SetParent(stone.transform,true);icon.GetComponent<Renderer>().enabled=true;
    Set(stone.GetComponent<GlowFeedback>(),"targetRenderer",icon.GetComponent<Renderer>());Set(stone.GetComponent<GlowFeedback>(),"intensity",1.4f);
    var fx=new GameObject("BriefElement_"+stone.Rune).transform;fx.SetParent(stone.transform,false);FElement(fx,stone.transform.position+Vector3.up*.05f,stone.Rune,.11f);fx.gameObject.SetActive(false);
    var pulse=stone.GetComponent<AlchemicalPulse>()??stone.gameObject.AddComponent<AlchemicalPulse>();pulse.effect=fx.gameObject;pulse.engraving=icon.GetComponent<Renderer>();
   }
   Set(SceneComponent<RunePuzzle>(),"litColor",new Color(.55f,.44f,.24f));Set(SceneComponent<RunePuzzle>(),"solvedColor",new Color(.65f,.52f,.28f));
   var clues=SceneComponent<VisualClues>();
   foreach(var light in clues.frameLights)light.color=new Color(.72f,.61f,.43f);
   var effects=clues.socketElements.transform;foreach(Transform child in effects.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);
   foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))FElement(effects,socket.transform.position+Vector3.up*.12f,socket.Side==SocketSide.Left?RuneId.Water:RuneId.Fire,.19f,true);
   foreach(var crystal in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(t=>t.name=="PlacedCrystal"||t.name=="Reward_Sapphire").ToArray())
   {
    foreach(var t in crystal.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("E_Element_")||t.name=="InnerWaterDrop"||t.name=="InnerEmber").ToArray())if(t!=null)Object.DestroyImmediate(t.gameObject);
    bool water=crystal.name=="Reward_Sapphire"||crystal.parent.name=="Socket_Left";
    var core=new GameObject("MaterialInsideCrystal").transform;core.SetParent(crystal,true);core.position=crystal.position;
    if(water)EShape("InternalWater",core,crystal.position,new Vector3(.065f,.14f,.065f),FWater("CrystalWater",new Color(.015f,.07f,.19f)));
    else FElement(core,crystal.position,RuneId.Fire,.055f);
   }
   var liquid=Find("Liquid");liquid.GetComponent<Renderer>().sharedMaterial=FWater("Brew",new Color(.035f,.055f,.06f));Set(liquid.GetComponent<GlowFeedback>(),"intensity",.22f);
   Set(SceneComponent<PotionPuzzle>(),"stepColors",new[]{new Color(.055f,.10f,.15f),new Color(.18f,.055f,.025f),new Color(.19f,.14f,.045f)});
   foreach(var r in Find("ArtB_Fit_Cauldron").GetComponentsInChildren<Renderer>(true))r.sharedMaterial=EMat("F_CastIron",new Color(.035f,.038f,.04f));
   var iron=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10e_F_CastIron.mat");iron.SetFloat("_Metallic",.8f);iron.SetFloat("_Smoothness",.25f);EditorUtility.SetDirty(iron);
   // S10d's orange square was the third seal lamp's rectangular emissive lens, NOT a HUD marker.
   for(int i=1;i<=3;i++)
   {
    var lamp=Find("SealLight_"+i);var lens=lamp.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.EndsWith("SealLampLens"));lens.enabled=false;
    var mark=EShape("SealLampEngraving",lamp.transform,lens.transform.position-new Vector3(0,0,.018f),new Vector3(.22f,.22f,1),FGlyph(i==1?RuneId.Earth:i==2?RuneId.Water:RuneId.Fire),PrimitiveType.Quad);
    var glow=mark.GetComponent<Renderer>().sharedMaterial;glow.SetColor("_EmissionColor",new Color(.24f,.19f,.10f));EditorUtility.SetDirty(glow);
    var light=lamp.GetComponentInChildren<Light>(true);light.color=new Color(.55f,.43f,.25f);light.intensity=.35f;
   }
   var p=SceneComponent<RoomPresentation>();p.dawn.color=new Color(.24f,.34f,.52f);p.dawn.intensity=.35f;p.dawn.range=4;p.dawn.spotAngle=40;p.dawn.innerSpotAngle=12;
   p.dawnWindowMaterial=EMat("F_NightWindow",new Color(.012f,.022f,.04f),.12f);p.dawnWindow.sharedMaterial=p.dawnWindowMaterial;
   foreach(var r in p.dawnRays.GetComponentsInChildren<Renderer>(true)){var mat=EMat("F_MoonRibbon",Color.white);mat.shader=Shader.Find("MagicEscape/MoonShaft");mat.SetColor("_Color",new Color(.20f,.30f,.48f,.025f));r.sharedMaterial=mat;EditorUtility.SetDirty(mat);}
   // A black full-screen root, with a modest gold-edged dark card. Existing text/buttons retained.
   var clear=p.clearGroup.GetComponent<Image>();clear.sprite=null;clear.color=Color.black;var rt=clear.rectTransform;rt.sizeDelta=new Vector2(1920,1080);
   var card=Rect("AlchemyEndingCard",clear.transform,new Vector2(920,630),Vector2.zero);var ci=card.gameObject.AddComponent<Image>();ci.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/S10d/ClothPanel.png");ci.type=Image.Type.Sliced;ci.color=new Color(.50f,.47f,.40f);ci.raycastTarget=false;card.SetAsFirstSibling();
   var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/GowunBatang-Regular-SDF.asset");
   foreach(var t in clear.GetComponentsInChildren<TMP_Text>(true)){t.font=font;t.color=new Color(.78f,.70f,.52f);if(t.name=="ClearMessage"){t.rectTransform.sizeDelta=new Vector2(820,130);t.fontSize=27;}}
   if(!before.SequenceEqual(ColliderManifest()))throw new InvalidOperationException("S10f changed gameplay collider manifest");
   File.WriteAllText("../docs/validation/s10f/builder-audit.txt","Collider manifest unchanged. Six equal slate visuals; original collider transforms retained. Orange rectangle = SealLight_3/ArtB_C_SealLampLens, replaced by brass fire incision. External fire = Unity Labs Flame02 CC0. Water normals original. No heat distortion (overdraw budget).\n");
   File.WriteAllLines("../docs/validation/s10f/scene-manifest.txt",Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(t=>Hierarchy(t)+"|"+t.position.ToString("F4")).OrderBy(x=>x));Save();
  }
  public static void S10f(){S10e();S10fApply();}
 }
}
