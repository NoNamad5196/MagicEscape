using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    // Presentation only: both rigs share XRI hover/selection, never an alternate input path.
    public sealed class ClearingFeedback : MonoBehaviour, IInteractable
    {
        XRBaseInteractable interactable;
        XriClearable clearable;
        Renderer[] surfaces;
        MaterialPropertyBlock[] original;
        AudioSource drag, impact;
        AudioClip scrapeClip, thumpClip;
        ParticleSystem dust;
        Vector3 previous;
        bool highlighted, moving, wasSelected;
        public bool Highlighted => highlighted;
        public int ImpactCount { get; private set; }
        public string GetPrompt(PlayerInteraction player)
        {
            if (clearable != null && clearable.kind != ClearingKind.Slide && clearable.IsCleared) return null;
            if (clearable == null) return "[좌클릭] 들기";
            return clearable.kind == ClearingKind.Slide ? "[좌클릭 유지+WASD] 밀기" :
                clearable.kind == ClearingKind.Cloth ? "[좌클릭] 걷기" : "[좌클릭] 열기";
        }
        public void Interact(PlayerInteraction player) { } // XRI alone owns selection.
        void Awake()
        {
            interactable = GetComponent<XRBaseInteractable>(); clearable = GetComponent<XriClearable>();
            surfaces = GetComponentsInChildren<Renderer>(); original = new MaterialPropertyBlock[surfaces.Length];
            for (int i=0;i<surfaces.Length;i++) { original[i]=new MaterialPropertyBlock(); surfaces[i].GetPropertyBlock(original[i]); }
            drag=gameObject.AddComponent<AudioSource>(); impact=gameObject.AddComponent<AudioSource>();
            foreach(var s in new[]{drag,impact}) { s.playOnAwake=false; s.spatialBlend=1; s.minDistance=.6f; s.maxDistance=6; }
            scrapeClip=MakeSound("Wood friction",true); thumpClip=MakeSound("Wood landing",false);
            drag.clip=scrapeClip; drag.loop=true; drag.volume=.17f; impact.volume=.32f;
            dust=MakeParticles(transform,"Clearing dust",new Color(.55f,.48f,.35f,.28f),.10f);
            previous=transform.position;
        }
        void LateUpdate()
        {
            var gm=GameManager.Instance; var ui=UIManager.Instance;
            bool playing=gm!=null && gm.IsPlaying && (ui==null || !ui.IsReading);
            bool near=Camera.main!=null && Vector3.Distance(Camera.main.transform.position,transform.position)<1.3f;
            bool focus=playing && interactable!=null && (interactable.isHovered || interactable.isSelected || near) && GetPrompt(null)!=null;
            if(focus!=highlighted)
            {
                highlighted=focus;
                for(int i=0;i<surfaces.Length;i++)
                {
                    if(!focus) { surfaces[i].SetPropertyBlock(original[i]); continue; }
                    var block=new MaterialPropertyBlock(); surfaces[i].GetPropertyBlock(block);
                    var mat=surfaces[i].sharedMaterial;
                    if(mat!=null && mat.HasProperty("_BaseColor")) block.SetColor("_BaseColor",Color.Lerp(mat.GetColor("_BaseColor"),new Color(.85f,.73f,.45f),.38f));
                    block.SetColor("_EmissionColor",new Color(.12f,.09f,.04f)); surfaces[i].SetPropertyBlock(block);
                }
            }
            bool displaced=Vector3.Distance(previous,transform.position)>.001f;
            // Hinge/cloth have no translational motion: their normalized progress is observed too.
            bool now=playing && (displaced || (clearable!=null && clearable.Progress>0 && clearable.Progress<.99f && clearable.kind!=ClearingKind.Slide));
            if(now && !moving) { drag.Play(); dust.Emit(8); }
            bool released=wasSelected && !interactable.isSelected;
            if(!now && moving) drag.Stop();
            if(released || (!now && moving && !interactable.isSelected)) { impact.PlayOneShot(thumpClip); dust.Emit(14); ImpactCount++; }
            moving=now;wasSelected=interactable.isSelected; previous=transform.position;
        }
        void OnDestroy() { if(scrapeClip!=null)Destroy(scrapeClip); if(thumpClip!=null)Destroy(thumpClip); }
        static AudioClip MakeSound(string name,bool friction)
        {
            const int rate=22050; int count=friction?rate:rate/4; var data=new float[count];
            var random=new System.Random(5196); float low=0;
            for(int i=0;i<count;i++)
            {
                float t=(float)i/rate; low=Mathf.Lerp(low,(float)random.NextDouble()*2-1,.18f);
                float envelope=friction ? .55f+.22f*Mathf.Sin(t*57) : Mathf.Exp(-t*22)*Mathf.Min(1,t*200);
                data[i]=(low*.65f+Mathf.Sin(t*(friction?390:95)*Mathf.PI*2)*.22f)*envelope;
            }
            var clip=AudioClip.Create(name,count,1,rate,false);clip.SetData(data,0);return clip;
        }
        public static ParticleSystem MakeParticles(Transform parent,string name,Color color,float size)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);
            var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=ps.main;main.playOnAwake=false;main.loop=false;main.startLifetime=.65f;main.startSpeed=.22f;main.startSize=size;main.startColor=color;main.maxParticles=48;
            var emission=ps.emission;emission.enabled=false;
            var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=.20f;
            var renderer=ps.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial=ParticleMaterial;
            return ps;
        }
        static Material particleMaterial;
        static Material ParticleMaterial
        {
            get
            {
                if(particleMaterial==null)
                {
                    particleMaterial=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
                    particleMaterial.hideFlags=HideFlags.HideAndDontSave;
                    // Tiny procedural VFX falloff, not puzzle illustration content.
                    var texture=new Texture2D(16,16,TextureFormat.RGBA32,false);texture.hideFlags=HideFlags.HideAndDontSave;
                    for(int y=0;y<16;y++)for(int x=0;x<16;x++)
                    {
                        float radius=Vector2.Distance(new Vector2(x,y),new Vector2(7.5f,7.5f))/7.5f;
                        texture.SetPixel(x,y,new Color(1,1,1,Mathf.Pow(Mathf.Clamp01(1-radius),2)));
                    }
                    texture.Apply();particleMaterial.SetTexture("_BaseMap",texture);
                    particleMaterial.SetFloat("_Surface",1);particleMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    particleMaterial.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    particleMaterial.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    particleMaterial.SetFloat("_SrcBlendAlpha",1);particleMaterial.SetFloat("_DstBlendAlpha",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    particleMaterial.SetFloat("_ZWrite",0);particleMaterial.SetFloat("_Cull",0);particleMaterial.renderQueue=3000;
                    particleMaterial.SetOverrideTag("RenderType","Transparent");
                }
                return particleMaterial;
            }
        }
    }
}
