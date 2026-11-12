using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using Object=UnityEngine.Object;

namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static readonly Color Candle=new Color(1,.64f,.30f);
        static readonly Color Lunar=new Color(.32f,.61f,.85f);
        static Material CMat(string name,Color color,float glow=0) => ArtMaterial("S10c_"+name,color,glow);
        static GameObject CVisual(string name,Transform parent,Vector3 pos,Vector3 size,Material mat,PrimitiveType type=PrimitiveType.Cube)
            => WorldVisual("C_"+name,parent,pos,size,mat,type);
        static Light CLight(string name,Vector3 pos,Color color,float intensity,float range,Vector3? aim=null,float angle=70)
        {
            Lamp("ArtC_"+name,pos,color,intensity,range,aim.HasValue?LightType.Spot:LightType.Point,angle);
            var l=Find("ArtC_"+name).GetComponent<Light>();l.shadows=LightShadows.None;
            if(aim.HasValue){l.transform.LookAt(aim.Value);l.innerSpotAngle=angle*.5f;}
            else if(color.r>color.b){var motion=l.gameObject.AddComponent<AtmosphereMotion>();motion.kind=AtmosphereMotion.Kind.Flame;motion.source=l;motion.seed=pos.sqrMagnitude;}
            return l;
        }
        static T VolumeEffect<T>(VolumeProfile p) where T:VolumeComponent
        {
            if(p.TryGet<T>(out var c))return c;
            c=p.Add<T>(true);AssetDatabase.AddObjectToAsset(c,p);return c;
        }
        static void ThirdLighting(Transform decor)
        {
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.032f,.05f,.075f);
            RenderSettings.reflectionIntensity=.12f;
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Exponential;RenderSettings.fogDensity=.023f;RenderSettings.fogColor=new Color(.017f,.036f,.045f);
            foreach(var l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                l.shadows=LightShadows.None;
                if(l.type==LightType.Directional)l.intensity=.045f;
                if(l.name=="ArtB_ClueWash"||l.name=="ArtB_NorthFill")l.enabled=false;
                if(l.name=="ArtB_ExitBlue")l.intensity=.26f;
                if(l.name=="Spot_Altar"){l.intensity=1.8f;l.range=4.2f;l.spotAngle=38;}
                if(l.name=="Spot_Seal"){l.intensity=1.2f;l.range=4;l.spotAngle=48;}
                if(l.color.r>l.color.b&&l.type==LightType.Point)
                {
                    l.range=Mathf.Min(l.range,3.2f);l.intensity*=.85f;
                    var m=l.gameObject.AddComponent<AtmosphereMotion>();m.kind=AtmosphereMotion.Kind.Flame;m.source=l;m.seed=l.transform.position.sqrMagnitude;
                }
            }
            for(int i=0;i<4;i++)
            {
                float z=-2.4f+i*1.6f;
                var l=CLight("Picture_"+i,new Vector3(-5.18f,2.55f,z),Candle,1.6f,2.1f,new Vector3(-5.8f,1.48f,z),64);
                CVisual("PictureLamp_"+i,decor,new Vector3(-5.56f,2.42f,z),new Vector3(.25f,.055f,.34f),CMat("Brass",new Color(.4f,.25f,.08f)));
                CVisual("PictureBulb_"+i,decor,new Vector3(-5.43f,2.39f,z),new Vector3(.1f,.025f,.24f),CMat("Bulb",Candle,1.5f));
            }
            CLight("Chalkboard",new Vector3(-4.9f,2.1f,-3.8f),Candle,1.1f,2,new Vector3(-5.8f,1.5f,-4),65);
            CLight("Sockets",new Vector3(-3,2.75f,3.25f),new Color(.5f,.65f,.8f),1,2.5f,new Vector3(-3,1,3.65f),75);
            CLight("BrewRim",new Vector3(3,1.85f,2.5f),new Color(.32f,.48f,.66f),.7f,2);
            var cam=SceneComponent<Camera>();
            var lantern=CLight("PlayerLantern",Vector3.zero,Candle,3.8f,7,Vector3.forward,92);
            lantern.transform.SetParent(cam.transform,false);lantern.transform.localPosition=new Vector3(.16f,-.12f,.1f);lantern.transform.localRotation=Quaternion.Euler(4,-2,0);
            lantern.innerSpotAngle=48;lantern.shadows=LightShadows.Soft;lantern.shadowStrength=.75f;lantern.shadowBias=.035f;lantern.shadowNormalBias=.16f;
            var sway=lantern.gameObject.AddComponent<AtmosphereMotion>();sway.kind=AtmosphereMotion.Kind.Lantern;sway.source=lantern;
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/ClassroomURP.asset");
            pipeline.supportsCameraDepthTexture=true;pipeline.supportsHDR=true;pipeline.maxAdditionalLightsCount=8;pipeline.shadowDistance=16;
            var so=new SerializedObject(pipeline);var shadows=so.FindProperty("m_AdditionalLightShadowsSupported");if(shadows!=null)shadows.boolValue=true;
            var map=so.FindProperty("m_AdditionalLightsShadowmapResolution");if(map!=null)map.intValue=1024;
            var soft=so.FindProperty("m_SoftShadowsSupported");if(soft!=null)soft.boolValue=true;
            so.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(pipeline);
            var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/ClassroomVolume.asset");
            VolumeEffect<Tonemapping>(profile).mode.Override(TonemappingMode.ACES);
            var bloom=VolumeEffect<Bloom>(profile);bloom.intensity.Override(.55f);bloom.threshold.Override(1.05f);bloom.scatter.Override(.55f);
            var vignette=VolumeEffect<Vignette>(profile);vignette.intensity.Override(.33f);vignette.smoothness.Override(.36f);
            var grain=VolumeEffect<FilmGrain>(profile);grain.type.Override(FilmGrainLookup.Thin1);grain.intensity.Override(.075f);grain.response.Override(.75f);
            VolumeEffect<ChromaticAberration>(profile).intensity.Override(.012f);
            var split=VolumeEffect<SplitToning>(profile);split.shadows.Override(new Color(.43f,.51f,.53f));split.highlights.Override(new Color(.56f,.51f,.44f));split.balance.Override(5);
            var adjust=VolumeEffect<ColorAdjustments>(profile);adjust.saturation.Override(-12);adjust.contrast.Override(12);adjust.postExposure.Override(.2f);adjust.colorFilter.Override(Color.white);
            EditorUtility.SetDirty(profile);
        }
        static void NightWindows(Transform decor)
        {
            var glass=CMat("NightGlass",new Color(.65f,.8f,1),.55f);glass.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/S10c/NightGlass.png");glass.SetTexture("_EmissionMap",glass.mainTexture);EditorUtility.SetDirty(glass);
            Find("Window_SE").GetComponent<Renderer>().sharedMaterial=glass;
            Find("ArtB_ExitMoonWindow").GetComponent<Renderer>().enabled=false;
            CVisual("HallGlass",decor,new Vector3(0,1.65f,7.92f),new Vector3(1.25f,1.8f,1),glass,PrimitiveType.Quad);
            var iron=CMat("WindowLead",new Color(.045f,.06f,.075f));
            for(int i=-2;i<=2;i++) CVisual("WindowLead_"+i,decor,new Vector3(5+i*.29f,1.7f,-4.81f),new Vector3(.025f,1.4f,.035f),iron);
            for(int i=-1;i<=1;i++) CVisual("WindowCross_"+i,decor,new Vector3(5,1.7f+i*.43f,-4.80f),new Vector3(1.48f,.024f,.035f),iron);
            for(int i=-1;i<=1;i++) CVisual("HallLead_"+i,decor,new Vector3(i*.4f,1.65f,7.91f),new Vector3(.035f,1.8f,.04f),iron);
            var moon=Find("Art_WindowBeam").GetComponent<Light>();moon.transform.position=new Vector3(5,2.2f,-4.65f);moon.transform.LookAt(new Vector3(2,.2f,-.9f));
            moon.color=Lunar;moon.intensity=5;moon.range=7;moon.spotAngle=48;moon.innerSpotAngle=25;moon.shadows=LightShadows.Soft;moon.shadowStrength=.65f;
            foreach(var l in new[]{moon,Find("ArtC_PlayerLantern").GetComponent<Light>()})
            {var data=l.GetComponent<UniversalAdditionalLightData>();if(data==null)data=l.gameObject.AddComponent<UniversalAdditionalLightData>();var settings=new SerializedObject(data);settings.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierMedium;settings.ApplyModifiedPropertiesWithoutUndo();}
            var beamMat=CMat("Shaft",Color.white);beamMat.shader=Shader.Find("MagicEscape/MoonShaft");beamMat.SetColor("_Color",new Color(.12f,.23f,.35f,.14f));EditorUtility.SetDirty(beamMat);
            // Two soft crossed ribbons, not an opaque cone. Confined to window's pool.
            var start=moon.transform.position;var end=new Vector3(2,.25f,-.9f);var axis=(end-start).normalized;
            for(int j=0;j<2;j++)
            {
                var g=new GameObject("ArtC_MoonShaft_"+j,typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(decor);
                var side=j==0?Vector3.Cross(axis,Vector3.up).normalized:Vector3.Cross(axis,Vector3.right).normalized;
                var mesh=new Mesh{name="Soft moon ribbon",vertices=new[]{start-side*.16f,start+side*.16f,end-side*.9f,end+side*.9f},uv=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1),new Vector2(1,1)},triangles=new[]{0,2,1,1,2,3}};mesh.RecalculateNormals();mesh.RecalculateBounds();
                var path="Assets/Settings/S10c_Shaft_"+j+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old==null){AssetDatabase.CreateAsset(mesh,path);old=mesh;}else{EditorUtility.CopySerialized(mesh,old);Object.DestroyImmediate(mesh);}
                g.GetComponent<MeshFilter>().sharedMesh=old;g.GetComponent<Renderer>().sharedMaterial=beamMat;g.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
            }
            var dust=Particles("ArtC_MoonDust",decor,(start+end)/2,Lunar,true);dust.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            dust.transform.rotation=Quaternion.FromToRotation(Vector3.forward,axis);
            var main=dust.main;main.playOnAwake=true;main.startLifetime=8;main.startSize=new ParticleSystem.MinMaxCurve(.008f,.023f);main.startSpeed=.025f;main.maxParticles=55;main.simulationSpace=ParticleSystemSimulationSpace.World;main.startColor=new Color(.34f,.55f,.7f,.32f);
            var shape=dust.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(1,1,4);var em=dust.emission;em.rateOverTime=6;
            dust.GetComponent<ParticleSystemRenderer>().sharedMaterial=ArtParticleMaterial(false);
        }
        static Mesh GemMesh()
        {
            string path="Assets/Settings/S10c_Gem.asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(mesh!=null)return mesh;
            var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
            for(int i=0;i<6;i++)
            {
                float a=i*Mathf.PI/3,b=(i+1)*Mathf.PI/3;
                var p=new Vector3(Mathf.Cos(a)*.38f,0,Mathf.Sin(a)*.38f);var q=new Vector3(Mathf.Cos(b)*.38f,0,Mathf.Sin(b)*.38f);
                foreach(var v in new[]{Vector3.up*.85f,q,p,Vector3.down*.6f,p,q}){triangles.Add(vertices.Count);vertices.Add(v);}
            }
            mesh=new Mesh{name="Authored six-sided cut crystal",vertices=vertices.ToArray(),triangles=triangles.ToArray()};mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);return mesh;
        }
        static Transform ItemArt(int index,Transform parent,Vector3 position,float size)
        {
            var root=new GameObject("ArtC_Item_"+ItemKeys[index]).transform;root.position=position;root.SetParent(parent,true);
            var blue=CMat("Sapphire",new Color(.04f,.26f,.82f),.75f);var red=CMat("Ruby",new Color(.85f,.045f,.09f),.7f);
            if(index==4||index==5)
            {
                var g=CVisual("Crystal",root,position,Vector3.one*size,(index==4?blue:red));g.GetComponent<MeshFilter>().sharedMesh=GemMesh();
                for(int i=-1;i<=1;i+=2){var chip=CVisual("CrystalChip",root,position+new Vector3(i*size*.22f,-size*.15f,0),Vector3.one*size*.48f,index==4?blue:red);chip.GetComponent<MeshFilter>().sharedMesh=GemMesh();chip.transform.rotation=Quaternion.Euler(0,0,i*25);}
            }
            else if(index==1||index==2||index==6)
            {
                var bottle=ModelAt("Quaternius",index==1?"Potion_1":"Potion_2",root,position-Vector3.up*size*.45f,new Vector3(size*.6f,size,size*.6f));
                foreach(var r in bottle.GetComponentsInChildren<Renderer>()){var m=CMat("Bottle_"+index,index==1?new Color(.9f,.6f,.4f):index==2?new Color(.4f,.78f,.9f):new Color(.45f,.85f,.48f),.025f);m.mainTexture=r.sharedMaterial.mainTexture;EditorUtility.SetDirty(m);r.sharedMaterial=m;}
                var content=CMat("Content_"+index,index==1?new Color(.9f,.18f,.04f):index==2?new Color(.16f,.7f,.85f):new Color(.36f,.8f,.3f),.6f);
                CVisual("BottleContents",root,position+new Vector3(0,-size*.08f,-size*.24f),new Vector3(size*.31f,size*.36f,size*.14f),content,PrimitiveType.Sphere);
                if(index==1)for(int k=0;k<3;k++){var s=CVisual("Scale",root,position+new Vector3((k-1)*size*.15f,0,-size*.31f),Vector3.one*size*.2f,red);s.GetComponent<MeshFilter>().sharedMesh=GemMesh();}
            }
            else if(index==0)
            {
                for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;var p=CVisual("MoonPetal",root,position+new Vector3(Mathf.Sin(a)*size*.22f,.02f,Mathf.Cos(a)*size*.22f),new Vector3(size*.22f,size*.10f,size*.5f),CMat("Petal",new Color(.48f,.7f,.93f),.45f),PrimitiveType.Sphere);p.transform.rotation=Quaternion.Euler(12, i*72,0);}
                CVisual("FlowerHeart",root,position+Vector3.up*.04f,Vector3.one*size*.18f,CMat("FlowerGold",new Color(.85f,.61f,.2f),.7f),PrimitiveType.Sphere);
            }
            else
            {
                CVisual("Stalk",root,position-Vector3.up*size*.20f,new Vector3(size*.2f,size*.22f,size*.2f),CMat("MushroomStalk",new Color(.5f,.42f,.3f)),PrimitiveType.Cylinder);
                CVisual("Cap",root,position+Vector3.up*size*.12f,new Vector3(size*.85f,size*.4f,size*.85f),CMat("MushroomCap",new Color(.35f,.04f,.16f)),PrimitiveType.Sphere);
                for(int i=0;i<5;i++){float a=i*1.25f;CVisual("CapSpot",root,position+new Vector3(Mathf.Sin(a)*size*.23f,size*.27f,Mathf.Cos(a)*size*.23f),Vector3.one*size*.085f,CMat("MushroomSpot",new Color(.75f,.6f,.43f)),PrimitiveType.Sphere);}
            }
            return root;
        }
        static void DressProps(Transform decor)
        {
            var stone=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Masonry.mat");
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if(t.name=="ArtB_Fit_pillar_decorated")foreach(var r in t.GetComponentsInChildren<Renderer>())r.sharedMaterial=stone;
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="ArtB_Fit_banner_patternA_blue"))foreach(var r in t.GetComponentsInChildren<Renderer>())r.enabled=false;
            var cloth=CMat("AcademyCloth",Color.white,.08f);cloth.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/S10c/AcademyCloth.png");cloth.SetTexture("_EmissionMap",cloth.mainTexture);EditorUtility.SetDirty(cloth);
            for(int side=-1;side<=1;side+=2)
            {
                var banner=CVisual("AcademyBanner_"+side,decor,new Vector3(side*2.5f,3.0f,4.69f),new Vector3(.75f,1.45f,1),cloth,PrimitiveType.Quad);
                CVisual("BannerRod_"+side,decor,new Vector3(side*2.5f,3.75f,4.65f),new Vector3(.9f,.05f,.06f),CMat("Brass",new Color(.4f,.25f,.08f)));
                CVisual("PillarFoot_"+side,decor,new Vector3(side*5.65f,.11f,4.65f),new Vector3(.68f,.22f,.68f),stone);
            }
            // Lantern trim atlas did not include the original metal sheet. Use purposeful dark brass instead.
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="ArtB_Fit_Lantern_Wall"))foreach(var r in t.GetComponentsInChildren<Renderer>())r.sharedMaterial=CMat("LanternMetal",new Color(.32f,.23f,.12f),.03f);
            for(int s=-1;s<=1;s+=2)CLight("NorthLantern_"+s,new Vector3(s*4.5f,2.78f,4.35f),Candle,1.2f,2.7f);
            // Shelf is physically attached to the wall, and the old candle no longer intersects a socket gem.
            var timber=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            for(int s=-1;s<=1;s+=2){var bracket=CVisual("WindowBracket_"+s,decor,new Vector3(5+s*.34f,.56f,-4.54f),new Vector3(.08f,.52f,.5f),timber);Tiled(bracket,timber,.5f);}
            Find("PlantPot").GetComponent<Renderer>().sharedMaterial=CMat("Terracotta",new Color(.28f,.12f,.08f));
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="Art_candle_lit"&&t.position.x<0))t.position+=new Vector3(-.14f,0,.1f);
            foreach(var pickup in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                var data=(ItemData)typeof(ItemPickup).GetField("item",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(pickup);
                int index=Array.IndexOf(ItemKeys,data.itemId);foreach(var r in pickup.GetComponentsInChildren<Renderer>(true))r.enabled=false;
                ItemArt(index,pickup.transform,pickup.transform.position,index==4?.27f:index==0?.27f:.22f);
            }
            foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
            {
                var placed=socket.transform.Find("PlacedCrystal");placed.GetComponent<MeshFilter>().sharedMesh=GemMesh();placed.localScale=new Vector3(.25f,.25f,.25f);
                placed.GetComponent<Renderer>().sharedMaterial=CMat(socket.Side==MagicEscape.Core.SocketSide.Left?"Sapphire":"Ruby",socket.Side==MagicEscape.Core.SocketSide.Left?new Color(.04f,.26f,.82f):new Color(.85f,.045f,.09f),.75f);
            }
            // Move only the HUD, not the collider-backed seal device. Keep the pedestal silhouette clear.
            foreach(var r in Resources.FindObjectsOfTypeAll<RectTransform>().Where(t=>t.gameObject.scene.IsValid()))
            {if(r.name=="Hotbar"){r.anchoredPosition=new Vector2(570,-482);r.localScale=Vector3.one*.78f;}if(r.name=="SelectedName"){r.anchoredPosition=new Vector2(570,-424);r.sizeDelta=new Vector2(600,40);r.GetComponent<TMP_Text>().fontSize=23;}}
            var board=Find("ChalkboardLabel").GetComponent<TMP_Text>();board.fontStyle=FontStyles.Normal;board.fontSize=1.55f;board.enableAutoSizing=false;board.textWrappingMode=TextWrappingModes.NoWrap;board.overflowMode=TextOverflowModes.Overflow;
            board.text=MagicEscape.Core.GameText.Chalkboard.Replace(" 순서로","\n순서로").Replace(" 액자의","\n액자의").Replace(" 따르라.","\n따르라.");board.rectTransform.sizeDelta=new Vector2(1.14f,.88f);board.color=new Color(.86f,.93f,.86f);
            var inscription=Resources.FindObjectsOfTypeAll<TMP_Text>().First(t=>t.name=="InscriptionText"&&t.gameObject.scene.IsValid());inscription.fontSize=1.08f;
            // Keep words intact on the two-line reveal without changing its text string.
            inscription.rectTransform.sizeDelta=new Vector2(1.78f,.38f);inscription.wordWrappingRatios=1;
            var title=Resources.FindObjectsOfTypeAll<RectTransform>().First(t=>t.name=="TitlePanel"&&t.gameObject.scene.IsValid());
            Label("ArtCredits",title,"사운드: Luke.RUSTLTD · DaGameKnower · Rudmer_Rotteveel (CC0)\n모델: Kay Lousberg · Quaternius (CC0) | 텍스처: Poly Haven (CC0) | 폰트: Noto Sans KR (OFL)",new Vector2(1700,70),new Vector2(0,-420),18);
        }
        static void CoreArt(Transform decor)
        {
            var core=Find("SealCore");core.GetComponent<MeshFilter>().sharedMesh=GemMesh();core.GetComponent<Renderer>().sharedMaterial=CMat("CoreObsidian",new Color(.08f,.06f,.17f),.02f);
            var light=CLight("CorePulse",core.transform.position,new Color(.4f,.3f,.8f),.4f,1.4f);light.transform.SetParent(core.transform,true);
            for(int j=0;j<2;j++)
            {
                var ring=new GameObject("ArtC_RuneRing_"+j).transform;ring.position=core.transform.position;ring.rotation=Quaternion.Euler(15+j*65,0,j*30);ring.SetParent(core.transform,true);
                for(int i=0;i<16;i++)
                {
                    float a=i*Mathf.PI/8;var p=ring.position+ring.rotation*new Vector3(Mathf.Cos(a)*(.23f+j*.045f),Mathf.Sin(a)*(.23f+j*.045f),0);
                    var rune=CVisual("OrbitRune",ring,p,new Vector3(.022f,.055f,.016f),CMat("OrbitRune",new Color(.35f,.27f,.6f),1.5f));rune.transform.rotation=ring.rotation*Quaternion.Euler(0,0,i*22.5f);
                    for(int k=0;k<2;k++){var arm=CVisual("RuneArm",ring,p+rune.transform.rotation*new Vector3(.014f,.018f-k*.025f,0),new Vector3(.035f,.009f,.016f),rune.GetComponent<Renderer>().sharedMaterial);arm.transform.rotation=rune.transform.rotation*Quaternion.Euler(0,0,25);}
                }
                // Batch all 48 rune pieces into one rotating mesh (two draw calls for both rings).
                var pieces=ring.GetComponentsInChildren<MeshFilter>();var combined=new Mesh{name="Combined orbit runes"};combined.CombineMeshes(pieces.Select(f=>new CombineInstance{mesh=f.sharedMesh,transform=ring.worldToLocalMatrix*f.transform.localToWorldMatrix}).ToArray());
                var path="Assets/Settings/S10c_Orbit_"+j+".asset";var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(saved==null){AssetDatabase.CreateAsset(combined,path);saved=combined;}else{EditorUtility.CopySerialized(combined,saved);Object.DestroyImmediate(combined);}
                foreach(var f in pieces)Object.DestroyImmediate(f.gameObject);
                ring.gameObject.AddComponent<MeshFilter>().sharedMesh=saved;ring.gameObject.AddComponent<MeshRenderer>().sharedMaterial=CMat("OrbitRune",new Color(.35f,.27f,.6f),1.5f);
                var motion=ring.gameObject.AddComponent<AtmosphereMotion>();motion.kind=AtmosphereMotion.Kind.SealRing;motion.seal=SceneComponent<SealPuzzle>();motion.emissives=ring.GetComponentsInChildren<Renderer>();motion.seed=j*90;motion.source=j==0?light:null;
            }
            var motes=Particles("ArtC_CoreMotes",decor,core.transform.position,new Color(.5f,.32f,.8f,.6f),true);motes.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=motes.main;main.playOnAwake=true;main.startLifetime=2.5f;main.startSize=.018f;main.startSpeed=.08f;main.maxParticles=18;
            var em=motes.emission;em.rateOverTime=5;var shape=motes.shape;shape.radius=.22f;motes.GetComponent<ParticleSystemRenderer>().sharedMaterial=ArtParticleMaterial(false);
        }
        static void NearRims()
        {
            var rim=CMat("NearRim",Color.white);rim.shader=Shader.Find("MagicEscape/NearRim");rim.SetColor("_Color",new Color(.22f,.36f,.38f,1));EditorUtility.SetDirty(rim);
            var targets=Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c is IInteractable).ToArray();
            foreach(var target in targets)foreach(var f in target.GetComponentsInChildren<MeshFilter>(true).ToArray())
            {
                var r=f.GetComponent<MeshRenderer>();if(r==null||!r.enabled||f.sharedMesh==null||f.GetComponent<TMP_Text>()!=null)continue;
                // Never outline the large clue wall, flat icon quads or invisible colliders.
                if(r.bounds.size.magnitude>1.7f||f.sharedMesh.vertexCount<8||f.name.StartsWith("ArtC_RuneRing"))continue;
                var g=new GameObject("ArtC_SubtleRim",typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(f.transform,false);g.GetComponent<MeshFilter>().sharedMesh=f.sharedMesh;var rr=g.GetComponent<MeshRenderer>();rr.sharedMaterial=rim;rr.shadowCastingMode=ShadowCastingMode.Off;rr.receiveShadows=false;
            }
        }
        static void AmbientAudio(Transform decor)
        {
            var amb=Component<RoomAmbience>("ArtC_Ambience",decor);
            Func<string,bool,AudioSource> source=(key,loop)=>{var a=Component<AudioSource>("ArtC_Audio_"+key,amb.transform);a.clip=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/AtmosphereCC0/"+key+".wav");if(a.clip==null)throw new InvalidOperationException("Missing CC0 ambience: "+key);a.loop=loop;a.playOnAwake=false;a.volume=loop?0:.095f;a.spatialBlend=loop?0:1;a.minDistance=2;a.maxDistance=12;return a;};
            amb.drone=source("drone",true);amb.wind=source("wind",true);amb.timber=source("creak",false);
        }
        static void BakeItemIcons()
        {
            Directory.CreateDirectory("Assets/Art/S10c/Icons");
            var oldAmbient=RenderSettings.ambientLight;var oldFog=RenderSettings.fog;RenderSettings.ambientLight=new Color(.65f,.65f,.65f);RenderSettings.fog=false;
            var camera=Component<Camera>("IconCamera");camera.transform.position=new Vector3(100,101,98);camera.transform.LookAt(new Vector3(100,100.1f,100));camera.orthographic=true;camera.orthographicSize=.82f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.cullingMask=1<<30;camera.enabled=false;
            var light=CLight("IconKey",new Vector3(98,103,97),Color.white,4,10);light.cullingMask=1<<30;
            var rt=RenderTexture.GetTemporary(256,256,24,RenderTextureFormat.ARGB32);var old=RenderTexture.active;
            try
            {
                for(int i=0;i<7;i++)
                {
                    var model=ItemArt(i,null,new Vector3(100,100,100),1);foreach(var t in model.GetComponentsInChildren<Transform>())t.gameObject.layer=30;
                    if(i==0)model.rotation=Quaternion.Euler(-30,0,0);
                    camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;var tex=new Texture2D(256,256,TextureFormat.RGBA32,false);tex.ReadPixels(new Rect(0,0,256,256),0,0);tex.Apply();
                    var path="Assets/Art/S10c/Icons/"+ItemKeys[i]+".png";File.WriteAllBytes(path,tex.EncodeToPNG());Object.DestroyImmediate(tex);Object.DestroyImmediate(model.gameObject);
                    AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.SaveAndReimport();
                    var item=Item(i);item.icon=AssetDatabase.LoadAssetAtPath<Sprite>(path);EditorUtility.SetDirty(item);
                }
            }
            finally{RenderSettings.ambientLight=oldAmbient;RenderSettings.fog=oldFog;camera.targetTexture=null;RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Object.DestroyImmediate(camera.gameObject);Object.DestroyImmediate(light.gameObject);}
        }
        public static void S10c()
        {
            S10b();var before=ColliderManifest();
            var gameplay=Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.GetType().Namespace=="MagicEscape").ToDictionary(c=>c,c=>EditorJsonUtility.ToJson(c));
            var decor=new GameObject("Art_ThirdPass").transform;
            ThirdLighting(decor);NightWindows(decor);DressProps(decor);CoreArt(decor);NearRims();AmbientAudio(decor);BakeItemIcons();
            if(!before.SequenceEqual(ColliderManifest()))throw new InvalidOperationException("S10c changed gameplay colliders, layers or world transforms");
            if(gameplay.Any(pair=>pair.Value!=EditorJsonUtility.ToJson(pair.Key)))throw new InvalidOperationException("S10c changed an existing gameplay component's configuration or references");
            Directory.CreateDirectory("../docs/validation/s10c");
            File.WriteAllText("../docs/validation/s10c/builder-audit.txt","S10c reconstructed from S10b via Editor API. Existing collider serialization, world positions/scales/layers AND gameplay MonoBehaviour serialization/references unchanged. No added collider/input/interaction. GameText and Core untouched. Two shadowed spotlights only (moon / player lantern).\n");
            File.WriteAllLines("../docs/validation/s10c/scene-manifest.txt",Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(t=>Hierarchy(t)+"|"+t.position.ToString("F4")+"|"+string.Join(",",t.GetComponents<Component>().Select(c=>c.GetType().Name))).OrderBy(x=>x));
            File.WriteAllLines("../docs/validation/s10c/lighting-audit.txt",Object.FindObjectsByType<Light>(FindObjectsSortMode.None).OrderBy(l=>l.name).Select(l=>l.name+"|enabled="+l.enabled+"|"+l.type+"|intensity="+l.intensity+"|range="+l.range+"|shadows="+l.shadows));
            Save();
        }
    }
}
