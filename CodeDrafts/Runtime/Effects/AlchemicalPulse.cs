using System.Collections;
using UnityEngine;
namespace MagicEscape
{
    /// <summary>Presentation only: incision fills, then a short material effect; no puzzle state.</summary>
    public sealed class AlchemicalPulse : MonoBehaviour
    {
        public GameObject effect;
        public Renderer engraving;
        public Renderer[] additionalEngravings = new Renderer[0];
        Coroutine pulse;
        MaterialPropertyBlock block;
        public void Trigger() { if(pulse!=null)StopCoroutine(pulse);pulse=StartCoroutine(Play()); }
        void SetFill(float value)
        {
            ApplyFill(engraving,value);
            foreach(var renderer in additionalEngravings) ApplyFill(renderer,value);
        }
        void ApplyFill(Renderer renderer,float value)
        {
            if(renderer==null)return;
            renderer.GetPropertyBlock(block);block.SetFloat("_Fill",value);renderer.SetPropertyBlock(block);
        }
        IEnumerator Play()
        {
            if(effect!=null)effect.SetActive(false);
            block=block??new MaterialPropertyBlock();
            for(float t=0;t<.22f;t+=Time.unscaledDeltaTime)
            {
                SetFill(Mathf.Clamp01(t/.22f));yield return null;
            }
            if(effect!=null)effect.SetActive(true);
            yield return new WaitForSecondsRealtime(.9f);
            if(effect!=null)effect.SetActive(false);
            SetFill(0);pulse=null;
        }
    }
}
