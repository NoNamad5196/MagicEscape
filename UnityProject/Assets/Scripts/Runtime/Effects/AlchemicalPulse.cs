using System.Collections;
using UnityEngine;
namespace MagicEscape
{
    /// <summary>Presentation only: incision fills, then a short material effect; no puzzle state.</summary>
    public sealed class AlchemicalPulse : MonoBehaviour
    {
        public GameObject effect;
        public Renderer engraving;
        Coroutine pulse;
        MaterialPropertyBlock block;
        public void Trigger() { if(pulse!=null)StopCoroutine(pulse);pulse=StartCoroutine(Play()); }
        IEnumerator Play()
        {
            if(effect!=null)effect.SetActive(false);
            block=block??new MaterialPropertyBlock();
            for(float t=0;t<.22f;t+=Time.unscaledDeltaTime)
            {
                if(engraving!=null){engraving.GetPropertyBlock(block);block.SetFloat("_Fill",Mathf.Clamp01(t/.22f));engraving.SetPropertyBlock(block);}yield return null;
            }
            if(effect!=null)effect.SetActive(true);
            yield return new WaitForSecondsRealtime(.9f);
            if(effect!=null)effect.SetActive(false);
            if(engraving!=null){engraving.GetPropertyBlock(block);block.SetFloat("_Fill",0);engraving.SetPropertyBlock(block);}pulse=null;
        }
    }
}
