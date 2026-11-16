using UnityEngine;
namespace MagicEscape
{
    // Neutral local illumination survives darkness. A small, brief pulse every 4.8s replaces outlines.
    [DefaultExecutionOrder(100)]
    public sealed class WarehouseItemBeacon : MonoBehaviour
    {
        public ItemPickup pickup;
        public Light source;
        public bool reward;
        bool seen;
        float appeared=-100;
        public int AnnouncementCount {get;private set;}
        void LateUpdate()
        {
            bool visible=pickup!=null && pickup.gameObject.activeInHierarchy && ClearingGate.Allows(pickup);
            if(visible&&!seen)
            {
                appeared=Time.time;
                if(reward){AudioManager.Play(SfxId.Pickup);AnnouncementCount++;}
            }
            seen=visible;
            if(source==null)return;
            source.enabled=visible;
            if(!visible)return;
            float age=Time.time-appeared;
            float pulse=Mathf.Pow(Mathf.Max(0,Mathf.Cos((Time.time+pickup.transform.position.x)*Mathf.PI*2/4.8f)),24);
            source.intensity=.55f+.25f*pulse+(reward?1.6f*Mathf.Max(0,1-age/.65f):0);
        }
    }
}
