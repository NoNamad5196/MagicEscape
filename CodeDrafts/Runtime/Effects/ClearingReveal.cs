using UnityEngine;
namespace MagicEscape
{
    // The dim leak is visible even while gated; the reveal is local, smooth and replayable after reset.
    public sealed class ClearingReveal : MonoBehaviour
    {
        public ClearingGate gate;
        public Light source;
        ParticleSystem sparkle;
        bool seen;
        float blend;
        public int AnnouncementCount { get; private set; }
        void Awake() { sparkle=ClearingFeedback.MakeParticles(transform,"Reveal shimmer",new Color(1,.88f,.58f,.8f),.025f); }
        void LateUpdate()
        {
            bool visible=gate!=null && gate.IsAccessible;
            if(visible && !seen) { sparkle.Emit(24);AudioManager.Play(SfxId.Pickup);AnnouncementCount++; }
            seen=visible;
            blend=Mathf.MoveTowards(blend,visible?1:0,Time.deltaTime/.65f);
            if(source!=null)source.intensity=Mathf.Lerp(.12f,.85f,Mathf.SmoothStep(0,1,blend));
        }
    }
}
