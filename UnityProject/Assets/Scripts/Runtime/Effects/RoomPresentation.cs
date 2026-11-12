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
        public ParticleSystem[] sealOff;
        public Light dawn;
        public Light roomFill;
        public Renderer dawnWindow;
        public Material dawnWindowMaterial;
        public GameObject dawnRays;
        public AudioSource sealLatch;
        public Renderer[] coreRenderers;
        float[] candleIntensity;
        Coroutine sequence;
        bool opening;
        bool[] lightWasEnabled;
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
        PlayerInputReader input;
        void Start() {input=FindFirstObjectByType<PlayerInputReader>();}
        void LateUpdate()
        {
            if(BrightnessStage<3)return;
            for(int i=0;i<candleFlames.Length;i++)if(candleFlames[i]!=null)
            {
                float sway=Mathf.Sin(Time.time*9+i)*8;
                candleFlames[i].transform.rotation=Quaternion.Euler(16+sway,0,sway*.3f);
                if(i<candles.Length&&candles[i]!=null)candles[i].intensity=candleIntensity[i]*(.45f+.13f*Mathf.PerlinNoise(Time.time*12,i));
            }
        }
        void Update()
        {
            if(opening&&input!=null&&input.AnyPressed)SkipPresentation();
        }
        public void BeginOpening()
        {
            if(sequence!=null)StopCoroutine(sequence);
            lightWasEnabled=new bool[openingDarkenedLights.Length];
            for(int i=0;i<openingDarkenedLights.Length;i++)lightWasEnabled[i]=openingDarkenedLights[i]!=null&&openingDarkenedLights[i].enabled;
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
                    PlayLatch();previousLamp=lamp;
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
            for(int i=0;i<openingDarkenedLights.Length;i++)if(openingDarkenedLights[i]!=null)openingDarkenedLights[i].enabled=lightWasEnabled!=null&&lightWasEnabled[i];
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
            else {SetFade(Color.black,0);if(clearGroup!=null){clearGroup.alpha=1;clearGroup.blocksRaycasts=true;}SetBlocked(false);}
        }
        static Color StageAmbient(int stage) => new Color(.105f,.12f,.15f)-new Color(.009f,.008f,.006f)*stage;
        public void SetProgress(int count,bool cue=true)
        {
            BrightnessStage=Mathf.Clamp(count,0,3);RenderSettings.ambientLight=StageAmbient(BrightnessStage);
            if(roomFill!=null)roomFill.intensity=.22f-BrightnessStage*.025f;
            for(int i=0;i<candles.Length;i++)if(candles[i]!=null){candles[i].enabled=true;candles[i].intensity=candleIntensity[i]*(1-BrightnessStage*.12f);}
            if(!cue)return;
            AudioManager.Play(SfxId.SocketPlace);
            if(sealOff!=null && count>0 && count<=sealOff.Length && sealOff[count-1]!=null)sealOff[count-1].Play();
            if(count==1&&brewSignal!=null)brewSignal.Play();
            if(count==2&&sealSignal!=null)sealSignal.Play();
            if(count==3)
            {
                if(coreShards!=null)coreShards.Play();foreach(var r in coreRenderers)if(r!=null)r.enabled=false;
                if(dawn!=null)dawn.enabled=true;
                if(dawnWindow!=null&&dawnWindowMaterial!=null)dawnWindow.sharedMaterial=dawnWindowMaterial;
                if(dawnRays!=null)dawnRays.SetActive(true);
                SetBlocked(true);sequence=StartCoroutine(ExitReveal());
            }
        }
        public void BeginEnding()
        {
            if(sequence!=null)StopCoroutine(sequence);opening=false;SetBlocked(true);
            sequence=StartCoroutine(Ending());
        }
        IEnumerator ExitReveal()
        {
            var door=FindFirstObjectByType<DoorController>();
            while(door!=null&&!door.PassageReady)yield return null;
            sequence=null;SetBlocked(false);
        }
        IEnumerator Ending()
        {
            if(clearGroup!=null){clearGroup.alpha=0;clearGroup.blocksRaycasts=false;}
            for(float t=0;t<1;t+=Time.unscaledDeltaTime/1.25f){SetFade(Color.black,t);yield return null;}
            SetFade(Color.black,1);yield return new WaitForSecondsRealtime(.2f);
            if(clearGroup!=null)clearGroup.alpha=1;
            for(float t=1;t>0;t-=Time.unscaledDeltaTime/.6f){SetFade(Color.black,t);yield return null;}
            SetFade(Color.black,0);if(clearGroup!=null)clearGroup.blocksRaycasts=true;sequence=null;SetBlocked(false);
        }
        void SetFade(Color color,float alpha) { if(fade!=null){color.a=alpha;fade.color=color;fade.raycastTarget=alpha>.01f;} }
        void PlayLatch() {if(sealLatch!=null&&sealLatch.clip!=null)sealLatch.PlayOneShot(sealLatch.clip);else AudioManager.Play(SfxId.SocketPlace);}
        void SetBlocked(bool blocked) {InputBlocked=blocked;if(GameManager.Instance!=null)GameManager.Instance.RefreshControlState();}
    }
}
