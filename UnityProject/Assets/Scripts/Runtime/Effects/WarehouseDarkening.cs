using System.Collections;
using UnityEngine;
namespace MagicEscape
{
    /// <summary>Shared PC/VR presentation only. Never locks input or mutates puzzle/clearing state.</summary>
    [DefaultExecutionOrder(50)]
    public sealed class WarehouseDarkening : MonoBehaviour
    {
        public WarehouseLightingProfile profile;
        public Light[] backgroundLights, localLights;
        public Light[] wickLights;
        public Renderer[] flames;
        public ParticleSystem[] smoke;
        public int[] extinguishAt;
        public Renderer brewLiquid;
        MaterialPropertyBlock brewBlock;
        Color brewEmission,brewBase;
        bool brewCaptured;
        float[] originalBackground, originalWicks;
        Vector3[] flameScale;
        float[] wickLevel;
        float stagePosition;
        Coroutine transition;
        bool initialized;
        public int Stage {get;private set;}
        public bool Transitioning => transition!=null;
        void Initialize()
        {
            if(initialized)return;
            initialized=true;
            foreach(var motion in FindObjectsByType<AtmosphereMotion>(FindObjectsSortMode.None))
                if(motion.kind==AtmosphereMotion.Kind.Flame)motion.enabled=false;
            originalBackground=new float[backgroundLights.Length];
            for(int i=0;i<backgroundLights.Length;i++)originalBackground[i]=backgroundLights[i]!=null?backgroundLights[i].intensity:0;
            originalWicks=new float[wickLights.Length];wickLevel=new float[wickLights.Length];flameScale=new Vector3[flames.Length];
            for(int i=0;i<wickLights.Length;i++)originalWicks[i]=wickLights[i].intensity;
            for(int i=0;i<flames.Length;i++)flameScale[i]=flames[i].transform.localScale;
        }
        public void SetStage(int stage,bool animate=true)
        {
            Initialize();Stage=Mathf.Clamp(stage,0,3);
            if(Stage>=2 && !brewCaptured && brewLiquid!=null)
            {
                brewBlock=new MaterialPropertyBlock();brewLiquid.GetPropertyBlock(brewBlock);
                brewEmission=brewBlock.GetColor("_EmissionColor");brewBase=brewBlock.GetColor("_BaseColor");
                brewCaptured=true;
            }
            if(transition!=null)StopCoroutine(transition);
            transition=null;
            if(animate)transition=StartCoroutine(Blend());
            else {stagePosition=Stage;Apply();}
        }
        IEnumerator Blend()
        {
            float start=stagePosition;
            for(float t=0;t<1;t+=Time.deltaTime/Mathf.Max(.1f,profile.transitionSeconds))
            {
                stagePosition=Mathf.Lerp(start,Stage,Mathf.SmoothStep(0,1,t));Apply();yield return null;
            }
            stagePosition=Stage;Apply();transition=null;
        }
        void LateUpdate()
        {
            if(!initialized)return;
            // Low frequency bounded noise, no flashes or camera effects.
            var a=profile.stages[Mathf.FloorToInt(stagePosition)];
            for(int i=0;i<flames.Length;i++)if(flames[i].enabled)
                flames[i].transform.localScale=Vector3.Scale(flameScale[i],new Vector3(Mathf.Sqrt(wickLevel[i]),wickLevel[i]*(1+a.flicker*Mathf.Sin(Time.time*2.3f+i)),Mathf.Sqrt(wickLevel[i])));
            for(int i=0;i<wickLights.Length;i++)
            {
                // Legacy glow feedback can re-enable a candle after a puzzle event. This controller owns the wick.
                wickLights[i].enabled=wickLevel[i]>.001f;
                flames[i].enabled=wickLevel[i]>.001f;
                wickLights[i].intensity=originalWicks[i]*wickLevel[i]*(1+a.flicker*(2*Mathf.PerlinNoise(Time.time*1.3f,i*3.17f)-1));
            }
        }
        void Apply()
        {
            int lo=Mathf.FloorToInt(stagePosition),hi=Mathf.Min(3,lo+1);float t=stagePosition-lo;
            var a=profile.stages[lo];var b=profile.stages[hi];
            RenderSettings.ambientLight=Color.Lerp(a.ambient,b.ambient,t);
            RenderSettings.reflectionIntensity=Mathf.Lerp(a.reflection,b.reflection,t);
            var p=GetComponent<RoomPresentation>();
            if(p.roomFill!=null)p.roomFill.intensity=Mathf.Lerp(a.fill,b.fill,t);
            for(int i=0;i<backgroundLights.Length;i++)if(backgroundLights[i]!=null)
                backgroundLights[i].intensity=originalBackground[i]*Mathf.Lerp(a.background,b.background,t);
            for(int i=0;i<wickLights.Length;i++)
            {
                float fadeA=extinguishAt[i]<=lo?0:a.candle,fadeB=extinguishAt[i]<=hi?0:b.candle;
                float value=Mathf.Lerp(fadeA,fadeB,t);
                if(wickLevel[i]>.015f && value<=.015f && smoke[i]!=null)smoke[i].Play();
                wickLevel[i]=value;wickLights[i].enabled=value>.001f;
                flames[i].enabled=value>.001f;
                flames[i].transform.localScale=Vector3.Scale(flameScale[i],new Vector3(Mathf.Sqrt(value),value,Mathf.Sqrt(value)));
            }
            if(brewCaptured && brewLiquid!=null)
            {
                float attenuation=Mathf.Lerp(1,profile.solvedBrewGlow,Mathf.Clamp01(stagePosition-1));
                brewLiquid.GetPropertyBlock(brewBlock);
                brewBlock.SetColor("_EmissionColor",brewEmission*attenuation);
                brewBlock.SetColor("_BaseColor",brewBase*Mathf.Sqrt(attenuation));
                brewLiquid.SetPropertyBlock(brewBlock);
            }
            foreach(var light in localLights)if(light!=null)light.enabled=true;
        }
    }
}
