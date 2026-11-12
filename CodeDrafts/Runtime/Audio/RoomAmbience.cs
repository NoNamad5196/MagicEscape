using UnityEngine;

namespace MagicEscape
{
    /// <summary>Quiet CC0 beds and sparse positional timber creaks, separate from puzzle SFX.</summary>
    public sealed class RoomAmbience : MonoBehaviour
    {
        public AudioSource drone, wind, timber;
        float nextCreak=16;
        float elapsed;
        void Update()
        {
            bool playing=GameManager.Instance!=null && GameManager.Instance.IsPlaying;
            int stage=GameManager.Instance!=null?GameManager.Instance.Progression.SolvedCount:0;
            Bed(drone,playing,.075f-stage*.015f);Bed(wind,playing,.10f-stage*.018f);
            if(drone!=null)drone.pitch=1-stage*.025f;
            if(!playing)return;
            elapsed+=Time.deltaTime;
            if(elapsed<nextCreak||timber==null)return;
            timber.transform.position=new Vector3(Mathf.Sin(elapsed)*4.5f,3.7f,Mathf.Cos(elapsed)*3.5f);
            timber.pitch=.85f+Mathf.PerlinNoise(elapsed,0)*.2f;timber.Play();
            nextCreak=elapsed+21+Mathf.PerlinNoise(0,elapsed)*15;
        }
        static void Bed(AudioSource bed,bool playing,float volume)
        {
            if(bed==null)return;
            if(playing&&!bed.isPlaying)bed.Play();
            bed.volume=Mathf.MoveTowards(bed.volume,playing?volume:0,Time.unscaledDeltaTime*.05f);
        }
    }
}
