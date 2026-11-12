using System;
using System.IO;
using System.Linq;
using MagicEscape.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        const string EArt = "Assets/Art/S10e/";
        static readonly Color[] Elements = {new Color(.28f,.8f,.13f),new Color(.07f,.48f,1),new Color(1,.25f,.025f),new Color(.48f,.7f,.53f),new Color(1,.76f,.22f),new Color(.35f,.06f,.55f)};
        static void EOpen() { EditorSceneManager.OpenScene(ScenePath); Directory.CreateDirectory("../docs/validation/s10e"); }
        static Transform ERoot(string name) { var old=GameObject.Find(name); if(old!=null)Object.DestroyImmediate(old); return new GameObject(name).transform; }
        static Material EMat(string name, Color color, float glow=0)
        {
            var m=ArtMaterial("S10e_"+name,color,glow); m.SetFloat("_Smoothness",.25f); EditorUtility.SetDirty(m); return m;
        }
        static GameObject EShape(string name,Transform parent,Vector3 p,Vector3 size,Material mat,PrimitiveType type=PrimitiveType.Sphere)
        {
            var g=GameObject.CreatePrimitive(type);g.name=name;Object.DestroyImmediate(g.GetComponent<Collider>());
            g.transform.position=p;g.transform.localScale=size;g.transform.SetParent(parent,true);g.GetComponent<Renderer>().sharedMaterial=mat;
            g.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;return g;
        }
        static void EMotion(GameObject g,ElementMotion.Motion motion,float speed=1) {var m=g.AddComponent<ElementMotion>();m.motion=motion;m.speed=speed;}
        static Material EParticleMat(string key)
        {
            var m=EMat("Particle_"+key,Color.white);m.shader=Shader.Find("Universal Render Pipeline/Particles/Unlit");
            m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+key+".png"));m.SetFloat("_Surface",1);m.SetFloat("_Blend",0);m.SetFloat("_SrcBlend",5);m.SetFloat("_DstBlend",10);m.SetFloat("_ZWrite",0);m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");m.renderQueue=3000;EditorUtility.SetDirty(m);return m;
        }
        static ParticleSystem EParticles(string name,Transform parent,Vector3 p,Color color,string sprite="Soft",float rate=8,float size=.04f,float radius=.1f,float rise=.15f)
        {
            var ps=Particles("E_"+name,parent,Vector3.zero,color,true);ps.transform.position=p;
            var main=ps.main;main.playOnAwake=true;main.startLifetime=1.8f;main.startSpeed=0;main.startSize=new ParticleSystem.MinMaxCurve(size*.5f,size);main.maxParticles=36;main.simulationSpace=ParticleSystemSimulationSpace.Local;
            var emission=ps.emission;emission.rateOverTime=rate;var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=radius;
            var velocity=ps.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;velocity.y=rise;
            var fade=ps.colorOverLifetime;fade.enabled=true;var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[]{new GradientAlphaKey(0,0),new GradientAlphaKey(.75f,.2f),new GradientAlphaKey(0,1)});fade.color=gradient;
            ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=EParticleMat(sprite);return ps;
        }
        static void ERing(string name,Transform parent,Vector3 p,float radius,Color color,ElementMotion.Motion motion=ElementMotion.Motion.Ripple)
        {
            // A line renderer is a proper hollow ring, not an opaque flattened sphere.
            var g=new GameObject(name);g.transform.position=p;g.transform.SetParent(parent,true);var line=g.AddComponent<LineRenderer>();
            line.useWorldSpace=false;line.loop=true;line.widthMultiplier=.008f;line.positionCount=48;line.sharedMaterial=EMat("Ring_"+ColorUtility.ToHtmlStringRGB(color),color,1.1f);
            for(int i=0;i<48;i++){float a=i*Mathf.PI/24;line.SetPosition(i,new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius));}EMotion(g,motion);
        }
        static void EElement(Transform parent,Vector3 p,RuneId rune,float size=.20f)
        {
            var root=new GameObject("E_Element_"+rune).transform;root.position=p;root.SetParent(parent,true);
            Color c=Elements[(int)rune]; var glow=EMat(rune.ToString(),c,.9f);var dark=EMat("Coal",new Color(.07f,.035f,.022f));
            if(rune==RuneId.Earth)
            {
                var rock=EShape("MossRock",root,p,new Vector3(size,size*.65f,size),EMat("MossStone",new Color(.25f,.18f,.095f)));rock.GetComponent<MeshFilter>().sharedMesh=GemMesh();rock.transform.Rotate(25,15,37);
                for(int i=0;i<4;i++){var cut=EShape("GreenCrack",root,p+new Vector3((i-1.5f)*size*.18f,size*.1f,-size*.22f),new Vector3(.009f,size*.65f,.01f),glow,PrimitiveType.Cube);cut.transform.Rotate(0,0,i%2==0?25:-28);}
                EParticles("Moss",root,p,c,"Soft",3,.018f,size*.3f,.035f);
            }
            else if(rune==RuneId.Water)
            {
                EShape("ShallowBowl",root,p,new Vector3(size*1.15f,size*.12f,size*1.15f),EMat("Bowl",new Color(.24f,.29f,.32f)),PrimitiveType.Cylinder);
                var water=EMat("WaterSurface",c);water.shader=Shader.Find("MagicEscape/ElementWater");water.SetColor("_BaseColor",new Color(.015f,.08f,.22f));water.SetColor("_EmissionColor",new Color(.15f,.65f,1));EditorUtility.SetDirty(water);
                EShape("FlowingWater",root,p+Vector3.up*size*.14f,new Vector3(size,size*.015f,size),water,PrimitiveType.Cylinder);
                ERing("WaterRipple",root,p+Vector3.up*size*.17f,size*.4f,c);
                var drop=EShape("OrbitingDrop",root,p+Vector3.up*size*.35f,new Vector3(size*.16f,size*.27f,size*.16f),glow);EMotion(drop,ElementMotion.Motion.Orbit);
                EParticles("WaterDrops",root,p,c,"Drop",6,.025f,size*.32f,-.08f);
            }
            else if(rune==RuneId.Fire)
            {
                for(int i=0;i<3;i++) {float x=(i-1)*size*.25f;EShape("Charcoal",root,p+new Vector3(x,0,0),new Vector3(size*.4f,size*.25f,size*.65f),dark);var f=EShape("TongueOfFire",root,p+new Vector3(x,size*.33f,0),new Vector3(size*.16f,size*.65f,size*.18f),glow);f.GetComponent<MeshFilter>().sharedMesh=GemMesh();EMotion(f,ElementMotion.Motion.Flame,1+i*.2f);}
                EParticles("Embers",root,p+Vector3.up*size*.3f,c,"Spark",10,.035f,size*.28f,.23f);
            }
            else if(rune==RuneId.Wind)
            {
                for(int j=0;j<3;j++)ERing("WindSpiral",root,p+Vector3.up*j*size*.26f,size*(.25f+j*.12f),c,ElementMotion.Motion.Halo);
                var ps=EParticles("WhirlingLeaves",root,p+Vector3.up*size*.3f,c,"Leaf",8,.055f,size*.5f,.04f);var v=ps.velocityOverLifetime;v.orbitalY=3f;v.radial=.015f;
            }
            else if(rune==RuneId.Light)
            {
                var orb=EShape("RadiantOrb",root,p+Vector3.up*size*.25f,Vector3.one*size*.55f,glow);EMotion(orb,ElementMotion.Motion.Orbit,.4f);
                ERing("GoldenHalo",root,p+Vector3.up*size*.3f,size*.55f,c,ElementMotion.Motion.Halo);
                for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var ray=EShape("LightRay",root,p+new Vector3(Mathf.Sin(a)*size*.5f,size*.25f,Mathf.Cos(a)*size*.5f),new Vector3(.008f,.008f,size*.28f),glow,PrimitiveType.Cube);ray.transform.rotation=Quaternion.Euler(0,i*45,0);}
                EParticles("LightMotes",root,p,c,"Spark",4,.035f,size*.4f,.08f);
            }
            else
            {
                for(int i=0;i<3;i++){var smoke=EShape("SmokeOrb",root,p+new Vector3((i-1)*size*.15f,size*.18f,0),Vector3.one*size*(.45f+i*.12f),EMat("ShadowSmoke",new Color(.065f,.012f,.1f),.3f));EMotion(smoke,ElementMotion.Motion.Smoke,.7f+i*.3f);}
                EParticles("PurpleSmoke",root,p,c,"Soft",9,.15f,size*.25f,.1f);
            }
        }
        public static void S10ePhase1()
        {
            EOpen();ERoot("Art_FifthPass");
            foreach(var rune in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
            {
                foreach(var t in rune.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("E_Element_")).ToArray())Object.DestroyImmediate(t.gameObject);
                EElement(rune.transform,rune.transform.position+Vector3.up*.065f,rune.Rune,.24f);
                var icon=rune.transform.Find("ArtB_RuneIcon_"+rune.Rune);if(icon!=null){icon.position+=new Vector3(0,.003f,-.14f);icon.localScale=Vector3.Scale(icon.localScale,new Vector3(1.25f,1.25f,1));}
            }
            foreach(var pickup in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                var p=pickup.transform.position;string id=((ItemData)typeof(ItemPickup).GetField("item",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(pickup)).itemId;
                if(id==ItemIds.CrystalSapphire)EElement(pickup.transform,p,RuneId.Water,.12f);
                else if(id==ItemIds.Moonpetal){EParticles("MoonPollen",pickup.transform,p,Elements[1],"Spark",5,.024f,.12f,.035f);var l=CLight("PetalMoonbeam",p+new Vector3(.1f,1.2f,-.15f),Lunar,1.4f,2,p,28);l.shadows=UnityEngine.LightShadows.None;}
                else if(id==ItemIds.SalamanderScale)EParticles("BottleEmbers",pickup.transform,p,Elements[2],"Spark",7,.025f,.045f,.065f);
                else if(id==ItemIds.CrystalDust)EParticles("BottledStars",pickup.transform,p,new Color(.7f,.9f,1),"Spark",8,.03f,.06f,.015f);
                else if(id==ItemIds.Toadstool){EParticles("Poison",pickup.transform,p,new Color(.22f,.48f,.035f,.6f),"Soft",7,.12f,.11f,.12f);foreach(var r in pickup.GetComponentsInChildren<Renderer>())if(r.name=="Cap")r.sharedMaterial=EMat("DangerCap",new Color(.65f,.025f,.025f));}
            }
            foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
                EElement(socket.transform.Find("PlacedCrystal"),socket.transform.position+Vector3.up*.18f,socket.Side==SocketSide.Left?RuneId.Water:RuneId.Fire,.12f);
            // Crystal surfaces show moving caustics; contents remain visible around their cut edges.
            var sapphire=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10c_Sapphire.mat");sapphire.shader=Shader.Find("MagicEscape/ElementWater");sapphire.SetColor("_BaseColor",new Color(.015f,.06f,.22f));sapphire.SetColor("_EmissionColor",new Color(.12f,.5f,1));EditorUtility.SetDirty(sapphire);
            var potion=SceneComponent<PotionPuzzle>();Set(potion,"stepColors",new[]{new Color(.45f,.75f,1),new Color(1,.28f,.04f),new Color(1,.72f,.15f)});
            var liquid=Find("Liquid");EParticles("Steam",liquid.transform,liquid.transform.position+Vector3.up*.05f,new Color(.7f,.76f,.8f,.28f),"Soft",5,.17f,.25f,.18f);
            EParticles("Bubbles",liquid.transform,liquid.transform.position+Vector3.up*.02f,new Color(.55f,.72f,.8f,.7f),"Bubble",8,.055f,.25f,.09f);
            var smoke=EParticles("WrongBlackSmoke",potion.transform,new Vector3(3,1.1f,2.5f),new Color(.025f,.018f,.035f,.9f),"Soft",0,.3f,.3f,.35f);var main=smoke.main;main.loop=false;main.playOnAwake=false;var em=smoke.emission;em.SetBursts(new[]{new ParticleSystem.Burst(0,30)});Set(potion,"smokeVfx",smoke);
            File.WriteAllText("../docs/validation/s10e/phase1-audit.txt","Original procedural elements; no new colliders, no per-particle lights. Each emitter <=36 particles. Ingredient transforms unchanged.\n");Save();
        }
        static Material EPicture(string file,float glow=.15f)
        {
            var m=EMat(file,Color.white,glow);m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+file+".png");m.SetTexture("_EmissionMap",m.mainTexture);EditorUtility.SetDirty(m);return m;
        }
        public static void S10ePhase2()
        {
            EOpen();var root=ERoot("VisualClues");var clues=root.gameObject.AddComponent<VisualClues>();
            Find("ChalkboardLabel").SetActive(false);
            var board=EShape("ChalkSequence",root,new Vector3(-5.77f,1.5f,-4),new Vector3(1.22f,.95f,1),EPicture("ChalkSequence"),PrimitiveType.Quad);board.transform.rotation=Quaternion.Euler(0,-90,0);
            Set(Find("Chalkboard").GetComponent<InspectableObject>(),"picture",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"ChalkSequence.png"));
            var book=Find("RecipeBook");Set(book.GetComponent<InspectableObject>(),"picture",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"PictureRecipe.png"));
            var page=EShape("OpenPictureRecipe",root,new Vector3(4.2f,1.165f,3.6f),new Vector3(.64f,.4f,1),EPicture("PictureRecipe",.28f),PrimitiveType.Quad);page.transform.rotation=Quaternion.Euler(90,0,0);
            string[] romans={"I","II","III","IV"};string[] names={"Earth","Water","Fire","Light"};
            for(int i=0;i<4;i++)
            {
                var painting=Find("Painting_"+romans[i]);var tex=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/S10b/Story_"+(i+1)+".png");Set(painting.GetComponent<InspectableObject>(),"picture",tex);
                var frame=painting.transform.Find("ArtB_PictureFrame_"+romans[i]);
                var icon=frame.Find("ArtB_CornerRune_"+names[i]);if(icon!=null){icon.localScale=new Vector3(.31f,.31f,1);icon.localPosition=new Vector3(-.35f,.25f,-.025f);}
                int index=Array.IndexOf(new[]{"III","I","IV","II"},romans[i]);var old=GameObject.Find("ArtC_Picture_"+index);if(old!=null)old.GetComponent<Light>().enabled=false;
                var l=CLight("AnswerFrame_"+romans[i],new Vector3(-5.1f,2.35f,painting.transform.position.z),Elements[(int)(RuneId)Enum.Parse(typeof(RuneId),names[i])],2.6f,2.6f,new Vector3(-5.8f,1.65f,painting.transform.position.z),64);l.enabled=false;clues.frameLights[i]=l;
            }
            var reveal=Find("Inscription").transform.Find("RevealedText");reveal.Find("InscriptionText").gameObject.SetActive(false);
            foreach(string name in new[]{"Water","Fire"})
            {
                var icon=reveal.Find("ArtB_InscriptionIcon_"+name);icon.position=new Vector3(name=="Water"?-3.55f:-2.45f,1.76f,3.82f);icon.localScale=new Vector3(.46f,.46f,1);
            }
            var effects=new GameObject("RevealedSocketElements").transform;effects.SetParent(root,false);clues.socketElements=effects.gameObject;
            foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
            {
                bool water=socket.Side==SocketSide.Left;var pos=socket.transform.position;
                EElement(effects,pos+Vector3.up*.35f,water?RuneId.Water:RuneId.Fire,.25f);
                if(water){var drops=EParticles("SocketWaterfall",effects,pos+Vector3.up*.55f,Elements[1],"Drop",16,.04f,.12f,-.34f);ERing("SocketPool",effects,pos+Vector3.up*.075f,.19f,Elements[1]);}
                else EParticles("SocketFlames",effects,pos+Vector3.up*.25f,Elements[2],"Spark",15,.07f,.12f,.32f);
            }
            effects.gameObject.SetActive(false);Set(Find("Liquid").GetComponent<GlowFeedback>(),"affectBaseColor",true);
            var objective=Resources.FindObjectsOfTypeAll<UnityEngine.RectTransform>().First(t=>t.name=="ObjectivePanel"&&t.gameObject.scene.IsValid());objective.sizeDelta=new Vector2(1000,390);objective.anchoredPosition=new Vector2(-420,290);
            var hint=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="HintText"&&t.gameObject.scene.IsValid());hint.rectTransform.sizeDelta=new Vector2(950,270);hint.rectTransform.anchoredPosition=new Vector2(0,-40);hint.fontSize=23;
            Save();
        }
        public static void S10ePhase3()
        {
            EOpen();var root=ERoot("RoomPresentation");var p=root.gameObject.AddComponent<RoomPresentation>();
            p.openingDarkenedLights=Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(l=>l.enabled&&!l.name.Contains("AnswerFrame")&&!l.transform.IsChildOf(Find("ExitDoor").transform)).ToArray();
            p.candles=new Light[6];p.candleFlames=new Renderer[6];
            for(int i=0;i<6;i++)
            {
                var pos=new Vector3(i%2==0?-1.9f:1.9f,.12f,4.15f-(i/2)*1.45f);
                EShape("OpeningCandle",root,pos,new Vector3(.1f,.15f,.1f),EMat("CandleWax",new Color(.64f,.52f,.30f)),PrimitiveType.Cylinder);
                var f=EShape("OpeningFlame",root,pos+Vector3.up*.22f,new Vector3(.04f,.12f,.04f),EMat("CandleFlame",new Color(1,.5f,.12f),2));EMotion(f,ElementMotion.Motion.Flame);p.candleFlames[i]=f.GetComponent<Renderer>();
                var light=new GameObject("E_CandleLight_"+i).AddComponent<Light>();light.transform.SetParent(root,true);light.transform.position=pos+Vector3.up*.3f;light.color=Candle;light.intensity=.6f;light.range=2.2f;light.shadows=UnityEngine.LightShadows.None;p.candles[i]=light;
            }
            p.sealLamps=Enumerable.Range(1,3).Select(i=>Find("SealLight_"+i)).ToArray();
            var circle=Find("ArtB_AltarCircle");p.altarCircle=circle.GetComponent<GlowFeedback>()??circle.AddComponent<GlowFeedback>();Set(p.altarCircle,"targetRenderer",circle.GetComponent<Renderer>());Set(p.altarCircle,"intensity",1f);
            p.brewSignal=EParticles("BrewSignal",root,new Vector3(3,.25f,2.5f),Elements[2],"Spark",0,.09f,.35f,.25f);var main=p.brewSignal.main;main.loop=false;main.playOnAwake=false;var emission=p.brewSignal.emission;emission.SetBursts(new[]{new ParticleSystem.Burst(0,30)});
            p.sealSignal=EParticles("SealSignal",root,new Vector3(-3,1.7f,3.7f),Elements[4],"Spark",0,.07f,.6f,.08f);main=p.sealSignal.main;main.loop=false;main.playOnAwake=false;emission=p.sealSignal.emission;emission.SetBursts(new[]{new ParticleSystem.Burst(0,30)});
            p.coreShards=EParticles("CoreShards",root,new Vector3(-3,1.3f,3.6f),Elements[4],"Spark",0,.12f,.2f,.4f);main=p.coreShards.main;main.loop=false;main.playOnAwake=false;main.startSpeed=.7f;emission=p.coreShards.emission;emission.SetBursts(new[]{new ParticleSystem.Burst(0,36)});
            p.coreRenderers=Find("SealCore").GetComponentsInChildren<Renderer>(true);
            var dawn=new GameObject("E_DawnLight").AddComponent<Light>();dawn.transform.SetParent(root,true);dawn.transform.position=new Vector3(0,2,6.7f);dawn.transform.LookAt(new Vector3(0,.8f,1));dawn.type=LightType.Spot;dawn.spotAngle=100;dawn.innerSpotAngle=65;dawn.range=9;dawn.color=new Color(1,.71f,.40f);dawn.intensity=9;dawn.shadows=UnityEngine.LightShadows.None;dawn.enabled=false;p.dawn=dawn;
            var canvas=SceneComponent<UIManager>().transform;
            var overlay=Rect("PresentationFade",canvas,new Vector2(1920,1080),Vector2.zero).gameObject.AddComponent<UnityEngine.UI.Image>();overlay.color=Color.clear;overlay.raycastTarget=false;overlay.transform.SetAsLastSibling();p.fade=overlay;
            var clear=Resources.FindObjectsOfTypeAll<UnityEngine.RectTransform>().First(t=>t.name=="ClearPanel"&&t.gameObject.scene.IsValid());p.clearGroup=clear.GetComponent<CanvasGroup>();if(p.clearGroup==null)p.clearGroup=clear.gameObject.AddComponent<CanvasGroup>();
            var paper=EPicture("PictureRecipe"); // UI uses the quiet original parchment rather than recipe symbols.
            var im=clear.GetComponent<UnityEngine.UI.Image>();im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(DArt+"ClothPanel.png");im.color=new Color(.72f,.60f,.40f,1);
            RenderSettings.ambientLight=new Color(.052f,.062f,.078f);Save();
        }
        public static void S10ePhase4()
        {
            EOpen();var before=ColliderManifest();var decor=ERoot("Art_FifthFinish");
            // The old transparent soot glaze still reads as a lit rectangular card in URP.
            // Remove that surface rather than pretending the rebuilt sconce fixed it.
            foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.name.EndsWith("WallSoot")))r.enabled=false;
            var piles=Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.parent!=null&&t.parent.name=="Art_FourthPass"&&t.name=="ArtB_Fit_Book_Stack_1"&&t.position.y<.1f&&t.position.x<0).OrderBy(t=>t.position.z).ToArray();
            Vector3[] homes={new Vector3(-5.35f,0,-3.05f),new Vector3(-5.08f,0,-3.26f),new Vector3(-4.95f,0,2.9f),new Vector3(-5.25f,.20f,-3.12f),new Vector3(-5.18f,0,3.22f)};
            for(int i=0;i<piles.Length;i++){piles[i].position=homes[i%homes.Length];piles[i].rotation=Quaternion.Euler(0,new[]{-37,14,62,-14,27}[i%5],i==3?7:0);}
            foreach(var lamp in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.name=="ArtB_Fit_Lantern_Wall").ToArray())
            {
                foreach(var r in lamp.GetComponentsInChildren<Renderer>())r.enabled=false;
                var pos=lamp.position;var bronze=EMat("SconceBronze",new Color(.20f,.12f,.055f));
                EShape("SconceMount",decor,pos+new Vector3(0,.22f,.10f),new Vector3(.10f,.18f,.045f),bronze,PrimitiveType.Sphere);
                var arm=EShape("SconceArm",decor,pos+new Vector3(0,.07f,-.05f),new Vector3(.035f,.035f,.28f),bronze,PrimitiveType.Cube);
                EShape("SconceCup",decor,pos+new Vector3(0,.10f,-.17f),new Vector3(.17f,.025f,.17f),bronze,PrimitiveType.Cylinder);
                EShape("SconceWax",decor,pos+new Vector3(0,.24f,-.17f),new Vector3(.075f,.13f,.075f),EMat("CandleWax",new Color(.64f,.52f,.30f)),PrimitiveType.Cylinder);
                var flame=EShape("SconceFlame",decor,pos+new Vector3(0,.41f,-.17f),new Vector3(.035f,.10f,.035f),EMat("CandleFlame",new Color(1,.5f,.12f),2));EMotion(flame,ElementMotion.Motion.Flame);
            }
            CLight("EastDeskBounce",new Vector3(3.5f,2,-2.7f),new Color(.55f,.63f,.72f),1.5f,5.5f);
            var p=SceneComponent<RoomPresentation>();if(p.clearGroup==null){var clearObject=Resources.FindObjectsOfTypeAll<UnityEngine.RectTransform>().First(t=>t.name=="ClearPanel"&&t.gameObject.scene.IsValid());p.clearGroup=clearObject.GetComponent<CanvasGroup>();if(p.clearGroup==null)p.clearGroup=clearObject.gameObject.AddComponent<CanvasGroup>();}p.openingDarkenedLights=p.openingDarkenedLights.Concat(new[]{Find("ArtC_EastDeskBounce").GetComponent<Light>()}).Distinct().ToArray();
            p.roomFill=Find("Moonlight").GetComponent<Light>();p.roomFill.intensity=.22f;RenderSettings.ambientLight=new Color(.105f,.12f,.15f);
            p.sealLatch=decor.gameObject.AddComponent<AudioSource>();p.sealLatch.clip=AssetDatabase.LoadAssetAtPath<AudioClip>(EArt+"SealLatch.wav");p.sealLatch.playOnAwake=false;p.sealLatch.volume=.4f;p.sealLatch.spatialBlend=0;
            if(p.sealLatch.clip==null)throw new InvalidOperationException("Run python Tools/Generate-SealAudio.py before S10e");
            p.dawnWindow=Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).First(r=>r.name.EndsWith("_HallGlass"));p.dawnWindowMaterial=EMat("DawnWindow",new Color(1,.68f,.28f),1.8f);
            var rays=new GameObject("DawnRays").transform;rays.SetParent(decor,false);p.dawnRays=rays.gameObject;
            var rayMaterial=EMat("DawnRays",Color.white);rayMaterial.shader=Shader.Find("MagicEscape/MoonShaft");rayMaterial.SetColor("_Color",new Color(1,.58f,.20f,.13f));EditorUtility.SetDirty(rayMaterial);
            Vector3 start=new Vector3(0,2.15f,7.5f),end=new Vector3(0,.65f,1.7f),axis=(end-start).normalized;
            for(int j=0;j<2;j++)
            {
                var side=j==0?Vector3.Cross(axis,Vector3.up).normalized:Vector3.Cross(axis,Vector3.right).normalized;
                var mesh=new Mesh{name="Original dawn ribbon",vertices=new[]{start-side*.55f,start+side*.55f,end-side*1.2f,end+side*1.2f},uv=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(0,1),new Vector2(1,1)},triangles=new[]{0,2,1,1,2,3}};mesh.RecalculateNormals();mesh.RecalculateBounds();
                string meshPath="Assets/Settings/S10e_DawnRay_"+j+".asset";var saved=AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);if(saved==null){AssetDatabase.CreateAsset(mesh,meshPath);saved=mesh;}else{EditorUtility.CopySerialized(mesh,saved);Object.DestroyImmediate(mesh);}
                var ray=new GameObject("DawnRibbon_"+j,typeof(MeshFilter),typeof(MeshRenderer));ray.transform.SetParent(rays,false);ray.GetComponent<MeshFilter>().sharedMesh=saved;ray.GetComponent<Renderer>().sharedMaterial=rayMaterial;ray.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            rays.gameObject.SetActive(false);
            var smokeMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10e_ShadowSmoke.mat");smokeMaterial.shader=Shader.Find("MagicEscape/ElementSmoke");smokeMaterial.SetColor("_BaseColor",new Color(.07f,.008f,.11f));smokeMaterial.SetColor("_EmissionColor",new Color(.16f,.018f,.27f));EditorUtility.SetDirty(smokeMaterial);
            p.sealOff=new ParticleSystem[3];
            for(int i=0;i<3;i++){var ps=EParticles("SealExtinguish_"+i,decor,p.sealLamps[i].transform.position,new Color(1,.65f,.15f),"Spark",0,.045f,.1f,-.08f);var main=ps.main;main.loop=false;main.playOnAwake=false;main.startLifetime=.65f;var em=ps.emission;em.SetBursts(new[]{new ParticleSystem.Burst(0,14)});p.sealOff[i]=ps;}
            foreach(string name in new[]{"Sapphire","Ruby"})
            {
                var mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10c_"+name+".mat");mat.shader=Shader.Find("MagicEscape/ElementCrystal");bool water=name=="Sapphire";mat.SetColor("_BaseColor",water?new Color(.025f,.10f,.38f):new Color(.38f,.012f,.025f));mat.SetColor("_EmissionColor",water?new Color(.08f,.45f,1):new Color(1,.17f,.015f));mat.SetFloat("_Opacity",.28f);mat.SetFloat("_Flow",water?1:0);EditorUtility.SetDirty(mat);
            }
            var bottleGlass=EMat("ClearBottle",new Color(.24f,.38f,.42f));bottleGlass.shader=Shader.Find("MagicEscape/ElementCrystal");bottleGlass.SetColor("_BaseColor",new Color(.10f,.19f,.20f));bottleGlass.SetColor("_EmissionColor",new Color(.25f,.4f,.42f));bottleGlass.SetFloat("_Opacity",.18f);bottleGlass.SetFloat("_Flow",0);EditorUtility.SetDirty(bottleGlass);
            foreach(var pickup in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(pickup.ItemId==ItemIds.SalamanderScale||pickup.ItemId==ItemIds.CrystalDust)foreach(var r in pickup.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Potion_1"||r.name=="Potion_2"))r.sharedMaterial=bottleGlass;
            // A luminous interior is seen through the cut crystal rather than painted on an opaque shell.
            foreach(var crystal in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(t=>t.name=="PlacedCrystal"||t.name=="Reward_Sapphire").ToArray())
            {
                bool water=crystal.name=="Reward_Sapphire"||crystal.parent.name=="Socket_Left";var pos=crystal.position;
                foreach(var t in crystal.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="ShallowBowl"||t.name=="FlowingWater"||t.name=="Charcoal").ToArray())Object.DestroyImmediate(t.gameObject);
                var core=EShape(water?"InnerWaterDrop":"InnerEmber",crystal,pos,new Vector3(.035f,.07f,.035f),EMat(water?"InnerWater":"InnerFire",water?Elements[1]:Elements[2],1.4f));EMotion(core,water?ElementMotion.Motion.Orbit:ElementMotion.Motion.Flame,water ? .75f : 1.5f);
            }
            string path=EArt+"EndingParchment.png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteBorder=new Vector4(20,20,20,20);importer.SaveAndReimport();
            var clear=p.clearGroup.GetComponent<UnityEngine.UI.Image>();clear.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);clear.type=UnityEngine.UI.Image.Type.Sliced;clear.color=Color.white;
            foreach(var t in p.clearGroup.GetComponentsInChildren<TMPro.TMP_Text>(true))t.color=new Color(.20f,.13f,.06f);
            // Button captions sit on dark cloth, so retain their light ink.
            foreach(var button in p.clearGroup.GetComponentsInChildren<UnityEngine.UI.Button>(true))foreach(var t in button.GetComponentsInChildren<TMPro.TMP_Text>())t.color=new Color(.86f,.77f,.57f);
            if(!before.SequenceEqual(ColliderManifest()))throw new InvalidOperationException("S10e finish altered collider geometry or transforms");
            File.WriteAllText("../docs/validation/s10e/phase4-audit.txt","Floor books grouped in irregular corner clusters, wall lantern backplates replaced by metal arm/cup/wax shapes. East desk fill + brighter stage-0 ambient. Collider manifest unchanged.\n");
            File.WriteAllLines("../docs/validation/s10e/scene-manifest.txt",Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(t=>Hierarchy(t)+"|"+t.position.ToString("F4")+"|"+string.Join(",",t.GetComponents<Component>().Select(c=>c.GetType().Name))).OrderBy(x=>x));Save();
        }
        /// <summary>Reconstruct all five passes from source; phase methods are checkpoints on the saved scene.</summary>
        public static void S10e() {S10d();S10ePhase1();S10ePhase2();S10ePhase3();S10ePhase4();}
        public static void S10eTypographyRepair()
        {
            EOpen();var credits=Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>().First(t=>t.name=="ArtCredits"&&t.gameObject.scene.IsValid());
            credits.text=credits.text.Replace("\uCA0C","/");
            foreach(char c in credits.text.Where(c=>!char.IsControl(c)))if(!credits.font.HasCharacter(c,false,false))throw new InvalidOperationException("Credit glyph missing U+"+((int)c).ToString("X4"));
            Save();
        }
    }
}

