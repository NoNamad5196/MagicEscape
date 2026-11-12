using UnityEngine;

namespace MagicEscape
{
    /// <summary>Small, light-free elemental silhouettes. No gameplay or input ownership.</summary>
    public sealed class ElementMotion : MonoBehaviour
    {
        public enum Motion { Orbit, Ripple, Flame, Smoke, Halo }
        public Motion motion;
        public float speed = 1;
        Vector3 origin, scale;
        void Awake() { origin = transform.localPosition; scale = transform.localScale; }
        void Update()
        {
            float t = Time.time * speed;
            if (motion == Motion.Orbit)
            {
                transform.localPosition = origin + new Vector3(Mathf.Sin(t)*.055f, Mathf.Sin(t*1.7f)*.025f, Mathf.Cos(t)*.055f);
                transform.Rotate(0, Time.deltaTime*80*speed, 0, Space.Self);
            }
            else if (motion == Motion.Ripple)
            {
                float r = .65f + Mathf.Repeat(t*.35f, .65f);
                transform.localScale = new Vector3(scale.x*r, scale.y, scale.z*r);
            }
            else if (motion == Motion.Flame)
                transform.localScale = Vector3.Scale(scale, new Vector3(1+Mathf.Sin(t*5)*.12f, 1+Mathf.Sin(t*7)*.25f, 1));
            else if (motion == Motion.Smoke)
            {
                transform.localPosition = origin + Vector3.up*Mathf.Sin(t)*.025f;
                transform.Rotate(0, Time.deltaTime*35, Time.deltaTime*12);
            }
            else transform.Rotate(0, Time.deltaTime*25*speed, 0);
        }
    }
}
