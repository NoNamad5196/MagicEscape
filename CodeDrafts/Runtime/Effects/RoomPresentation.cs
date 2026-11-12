using System.Collections;
using MagicEscape.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MagicEscape
{
    /// <summary>Presentation is orthogonal to the existing Core flow and ExitTrigger.</summary>
    [DefaultExecutionOrder(-20)]
    public sealed class RoomPresentation : MonoBehaviour
    {
        public Light[] candles;
        public Renderer[] candleFlames;
        public Light[] openingDarkenedLights;
        public GameObject[] sealLamps;
        public Image fade;
        public CanvasGroup clearGroup;
        public GlowFeedback altarCircle;
        public ParticleSystem brewSignal, sealSignal, coreShards;
        public Light dawn;
        public Renderer[] coreRenderers;
        float[] candleIntensity;
        Coroutine sequence;
        bool opening;
        public bool InputBlocked { get; private set; }
        public bool SkippedThisFrame => skipFrame==Time.frameCount;
        int skipFrame=-1;
        public int BrightnessStage { get; private set; }
        public int LitSealCount
        {
            get { int n=0;foreach(var lamp in sealLamps)if(lamp!=null&&lamp.activeSelf)n++;return n; }
        }
        void Awake()
        {
            candleIntensity=new float[candles.Length];for(int i=0;i<candles.Length;i++)candleIntensity[i]=candles[i]!=null?candles[i].intensity:0;
            if(fade!=null)fade.color=Color.clear;
            if(dawn!=null)dawn.enabled=false;
        }
        void Update()
        {
            var input=FindFirstObjectByType<PlayerInputReader>();
            if(opening&&input!=null&&input.AnyPressed)SkipPresentation();
        }
        public void BeginOpening()
        {
            if(sequence!=null)StopCoroutine(sequence);
            opening=true;SetBlocked(true);sequence=StartCoroutine(Opening());
        }
        IEnumerator Opening()
        {
            foreach(var lamp in sealLamps)if(lamp!=null)lamp.SetActive(false);
            foreach(var l in candles)if(l!=null)l.enabled=false;
            foreach(var l in openingDarkenedLights)if(l!=null)l.enabled=false;
            foreach(var r in candleFlames)if(r!=null)r.enabled=false;
            if(altarCircle!=null)altarCircle.Off();
            RenderSettings.ambientLight=Color.black;SetFade(Color.black,1);
            float t=0;int previousLamp=-1;
            while(t<12)
            {
                t+=Time.unscaledDeltaTime;
                SetFade(Color.black,1-Mathf.Clamp01(t/2));
                for(int i=0;i<candles.Length;i++)if(candles[i]!=null)candles[i].enabled=t>1+i*5f/Mathf.Max(1,candles.Length);
                for(int i=0;i<candleFlames.Length;i++)if(candleFlames[i]!=null)candleFlames[i].enabled=t>1+i*5f/Mathf.Max(1,candleFlames.Length);
                int lamp=Mathf.Clamp(Mathf.FloorToInt(t-6),-1,2);
                if(lamp>previousLamp)
                {
                    for(int i=0;i<=lamp;i++)if(sealLamps[i]!=null)sealLamps[i].SetActive(true);
                    AudioManager.Play(SfxId.SocketPlace);previousLamp=lamp;
                }
                RenderSettings.ambientLight=Color.Lerp(Color.black,StageAmbient(0),Mathf.Clamp01(t/10));
                if(t>10&&altarCircle!=null)altarCircle.SetGlow(new Color(.15f,.12f,.055f));
                yield return null;
            }
            CompleteOpening();
        }
        void CompleteOpening()
        {
            opening=false;sequence=null;SetFade(Color.black,0);
            foreach(var l in openingDarkenedLights)if(l!=null)l.enabled=true;
            foreach(var r in candleFlames)if(r!=null)r.enabled=true;
            SetProgress(GameManager.Instance!=null?GameManager.Instance.Progression.SolvedCount:0,false);
            for(int i=0;i<sealLamps.Length;i++)if(sealLamps[i]!=null)sealLamps[i].SetActive(i>=BrightnessStage);
            if(altarCircle!=null)altarCircle.SetGlow(new Color(.15f,.12f,.055f));SetBlocked(false);
        }
        /// <summary>Explicit deterministic test/debug path, also used by any-key opening skip.</summary>
        public void SkipPresentation()
        {
            if(sequence!=null)StopCoroutine(sequence);sequence=null;skipFrame=Time.frameCount;
            if(opening)CompleteOpening();
            else {SetFade(Color.white,0);if(clearGroup!=null){clearGroup.alpha=1;clearGroup.blocksRaycasts=true;}SetBlocked(false);}
        }
        static Color StageAmbient(int stage) => new Color(.052f,.062f,.078f)+new Color(.022f,.020f,.014f)*stage;
        public void SetProgress(int count,bool cue=true)
        {
            BrightnessStage=Mathf.Clamp(count,0,3);RenderSettings.ambientLight=StageAmbient(BrightnessStage);
            for(int i=0;i<candles.Length;i++)if(candles[i]!=null){candles[i].enabled=true;candles[i].intensity=candleIntensity[i]*(1+BrightnessStage*.2f);}
            if(!cue)return;
            AudioManager.Play(SfxId.SocketPlace);
            if(count==1&&brewSignal!=null)brewSignal.Play();
            if(count==2&&sealSignal!=null)sealSignal.Play();
            if(count==3)
            {
                if(coreShards!=null)coreShards.Play();foreach(var r in coreRenderers)if(r!=null)r.enabled=false;
                if(dawn!=null)dawn.enabled=true;
            }
        }
        public void BeginEnding()
        {
            if(sequence!=null)StopCoroutine(sequence);opening=false;SetBlocked(true);
            sequence=StartCoroutine(Ending());
        }
        IEnumerator Ending()
        {
            if(clearGroup!=null){clearGroup.alpha=0;clearGroup.blocksRaycasts=false;}
            for(float t=0;t<1;t+=Time.unscaledDeltaTime/1.25f){SetFade(Color.white,t);yield return null;}
            SetFade(Color.white,1);yield return new WaitForSecondsRealtime(.2f);
            if(clearGroup!=null)clearGroup.alpha=1;
            for(float t=1;t>0;t-=Time.unscaledDeltaTime/.6f){SetFade(Color.white,t);yield return null;}
            SetFade(Color.white,0);if(clearGroup!=null)clearGroup.blocksRaycasts=true;sequence=null;SetBlocked(false);
        }
        void SetFade(Color color,float alpha) { if(fade!=null){color.a=alpha;fade.color=color;fade.raycastTarget=alpha>.01f;} }
        void SetBlocked(bool blocked) {InputBlocked=blocked;if(GameManager.Instance!=null)GameManager.Instance.RefreshControlState();}
    }
}
