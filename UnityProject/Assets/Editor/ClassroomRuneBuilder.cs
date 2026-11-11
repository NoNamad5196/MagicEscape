using System.Linq;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static T SceneComponent<T>() where T:Component => Resources.FindObjectsOfTypeAll<T>().First(x=>x.gameObject.scene.IsValid());
        static GameObject Shape(string name,Transform parent,Vector3 pos,Vector3 scale,PrimitiveType type=PrimitiveType.Cube,Color? color=null)
        {
            var g=GameObject.CreatePrimitive(type); g.name=name; g.transform.SetParent(parent,false); g.transform.localPosition=pos; g.transform.localScale=scale; g.layer=9;
            string path="Assets/Settings/Color_"+ColorUtility.ToHtmlStringRGB(color??new Color(.45f,.42f,.36f))+".asset";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat==null){ mat=new Material(Shader.Find("Universal Render Pipeline/Lit")); mat.color=color??new Color(.45f,.42f,.36f); mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",Color.black); AssetDatabase.CreateAsset(mat,path); }
            g.GetComponent<Renderer>().sharedMaterial=mat; return g;
        }
        static GlowFeedback Glow(GameObject g) {var glow=g.AddComponent<GlowFeedback>(); Set(glow,"targetRenderer",g.GetComponent<Renderer>()); return glow;}
        static ParticleSystem Particles(string name,Transform parent,Vector3 pos,Color color,bool loop=false)
        {
            var ps=Component<ParticleSystem>(name,parent); ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); ps.transform.localPosition=pos;
            var main=ps.main; main.playOnAwake=false; main.loop=loop; main.duration=1; main.startLifetime=1; main.startSpeed=.4f; main.startSize=.08f; main.startColor=color;
            var emission=ps.emission; emission.rateOverTime=loop?10:0; if(!loop) emission.SetBursts(new[]{new ParticleSystem.Burst(0,20)});
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Sphere; shape.radius=.2f;
            const string path="Assets/Settings/GrayboxParticles.asset"; var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")); AssetDatabase.CreateAsset(mat,path);} ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=mat;
            return ps;
        }
        static ItemPickup Pickup(string name,Transform parent,Vector3 pos,int index,bool hidden=false)
        {
            var g=Shape(name,parent,pos,Vector3.one*.18f,PrimitiveType.Sphere,ItemColors[index]); var pickup=g.AddComponent<ItemPickup>(); Set(pickup,"item",Item(index)); Set(pickup,"hiddenAtStart",hidden); return pickup;
        }
        public static void S6()
        {
            S5(); var puzzle=SceneComponent<RunePuzzle>(); puzzle.transform.position=new Vector3(0,0,.5f);
            Shape("AltarBase",puzzle.transform,new Vector3(0,.5f,0),new Vector3(1.6f,.5f,1.6f),PrimitiveType.Cylinder);
            var lid=Shape("AltarLid",puzzle.transform,new Vector3(0,1.01f,0),new Vector3(.5f,.03f,.5f),PrimitiveType.Cylinder); Set(puzzle,"altarLid",lid.transform);
            RuneId[] runes={RuneId.Light,RuneId.Shadow,RuneId.Earth,RuneId.Wind,RuneId.Water,RuneId.Fire}; var stones=new RuneStone[6];
            for(int i=0;i<6;i++)
            {
                float a=i*Mathf.PI/3; var pos=new Vector3(Mathf.Sin(a)*.6f,1.06f,Mathf.Cos(a)*.6f);
                var g=Shape("RuneStone_"+runes[i],puzzle.transform,pos,new Vector3(.22f,.03f,.22f),PrimitiveType.Cylinder); Glow(g); stones[i]=g.AddComponent<RuneStone>(); Set(stones[i],"rune",runes[i]);
                var text=WorldText("RuneLabel_"+runes[i],puzzle.transform,RuneStone.KoreanName(runes[i]),pos+Vector3.up*.07f,new Vector2(.3f,.15f),1.5f); text.transform.rotation=Quaternion.Euler(90,0,0);
            }
            Set(puzzle,"runeStones",stones); Set(puzzle,"rewardCrystal",Pickup("Reward_Sapphire",puzzle.transform,new Vector3(0,1.1f,0),4,true)); Set(puzzle,"solvedVfx",Particles("VFX_RuneSolved",puzzle.transform,new Vector3(0,1.3f,0),Color.yellow));
            var door=SceneComponent<DoorController>(); var lights=new GameObject[3];
            for(int i=0;i<3;i++){lights[i]=Shape("SealLight_"+(i+1),door.transform,new Vector3((i-1)*.5f,3,-.1f),Vector3.one*.18f,PrimitiveType.Sphere,Color.red); var light=Component<Light>("Light",lights[i].transform); light.color=Color.red; light.intensity=1; light.range=2;}
            Set(door,"sealLights",lights); puzzle.gameObject.SetActive(true); Save();
        }
    }
}
