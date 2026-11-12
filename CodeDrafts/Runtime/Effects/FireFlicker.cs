using UnityEngine;
namespace MagicEscape
{
    public sealed class FireFlicker : MonoBehaviour
    {
        public Light source;
        public float brightness=.35f;
        void Update(){if(source!=null)source.intensity=brightness*(.65f+.5f*Mathf.PerlinNoise(Time.time*13,transform.position.x*7));}
    }
}
