using UnityEngine;
using MagicEscape.Core;

namespace MagicEscape
{
    /// <summary>Presentation only. Never writes puzzle state, input or camera orientation.</summary>
    public sealed class AtmosphereMotion : MonoBehaviour
    {
        public enum Kind { Flame, Lantern, SealRing }
        public Kind kind;
        public Light source;
        public SealPuzzle seal;
        public Renderer[] emissives;
        public float seed;
        Vector3 origin;
        Quaternion rotation;
        float brightness;
        CharacterController walker;
        MaterialPropertyBlock block;

        void Awake()
        {
            origin=transform.localPosition; rotation=transform.localRotation;
            brightness=source!=null?source.intensity:0;
            walker=GetComponentInParent<CharacterController>();
            block=new MaterialPropertyBlock();
        }
        void LateUpdate()
        {
            float t=Time.time;
            if(kind==Kind.Flame)
            {
                int stage=GameManager.Instance!=null?GameManager.Instance.Progression.SolvedCount:0;
                if(source!=null) source.intensity=brightness*(1+stage*.2f)*(.94f+.08f*Mathf.PerlinNoise(seed,t*4)+.025f*Mathf.Sin(t*13+seed));
            }
            else if(kind==Kind.Lantern)
            {
                float speed=walker==null?0:new Vector2(walker.velocity.x,walker.velocity.z).magnitude;
                float a=Mathf.Clamp01(speed/3.2f);
                transform.localPosition=origin+new Vector3(Mathf.Sin(t*5)*.012f,Mathf.Sin(t*10)*.008f,0)*a;
                transform.localRotation=rotation*Quaternion.Euler(Mathf.Sin(t*5)*.6f*a,Mathf.Sin(t*2.5f)*.8f*a,0);
            }
            else
            {
                var stage=seal!=null?seal.Stage:SealStage.Dormant;
                bool released=stage==SealStage.Released;
                float rate=stage==SealStage.ReadyToRelease?32:released?8:13;
                transform.localRotation=rotation*Quaternion.AngleAxis(t*rate+seed,Vector3.forward);
                Color c=released?new Color(1,.65f,.18f):stage==SealStage.ReadyToRelease?new Color(.65f,.4f,1):new Color(.25f,.28f,.55f);
                float pulse=(.7f+.15f*Mathf.Sin(t*2.2f+seed))*(stage==SealStage.Dormant?.45f:1);
                foreach(var r in emissives) { if(r==null)continue; r.GetPropertyBlock(block);block.SetColor("_EmissionColor",c*pulse*2);r.SetPropertyBlock(block); }
                if(source!=null) {source.color=c;source.intensity=pulse*(released?.28f:.6f);}
            }
        }
    }
}
