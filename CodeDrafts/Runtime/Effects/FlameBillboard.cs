using UnityEngine;
namespace MagicEscape
{
    /// <summary>One camera-facing CC0 flame card; no particle warmup or invisible first frame.</summary>
    public sealed class FlameBillboard : MonoBehaviour
    {
        Camera view;
        void LateUpdate()
        {
            if(view==null)view=Camera.main;
            if(view!=null)transform.rotation=Quaternion.LookRotation(transform.position-view.transform.position,Vector3.up);
        }
    }
}
