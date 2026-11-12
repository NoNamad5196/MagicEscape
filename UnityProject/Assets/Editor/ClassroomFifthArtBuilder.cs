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
    }
}

