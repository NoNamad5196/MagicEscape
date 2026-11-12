using System.Collections;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 오브젝트 발광 피드백(문양석·가마솥·소켓·봉인 빛 공용).
    /// URP Lit 머티리얼의 Emission을 켜 두어야 한다(머티리얼 Inspector에서 Emission 체크).
    /// MaterialPropertyBlock을 사용하므로 머티리얼 인스턴스를 복제하지 않는다.
    /// </summary>
    [DisallowMultipleComponent]
    public class GlowFeedback : MonoBehaviour
    {
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private Renderer targetRenderer;
        [Tooltip("선택: 발광과 함께 켜고 끌 Point Light")]
        [SerializeField] private Light glowLight;
        [SerializeField] private float intensity = 3f;
        [SerializeField] private bool affectBaseColor;

        private MaterialPropertyBlock _block;
        private Color _steadyColor = Color.black;
        private Coroutine _flash;

        private void Awake()
        {
            if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();
            _block = new MaterialPropertyBlock();
            Apply(Color.black);
        }

        /// <summary>지속 발광 색 설정. Color.black = 꺼짐.</summary>
        public void SetGlow(Color color)
        {
            _steadyColor = color;
            if (_flash == null) Apply(color);
        }

        public void Off() => SetGlow(Color.black);

        /// <summary>잠깐 다른 색으로 깜빡인 뒤 지속 색으로 복귀(오답=빨강 등). timeScale과 무관.</summary>
        public void Flash(Color color, float seconds = 0.6f)
        {
            if (!isActiveAndEnabled) return;
            if (_flash != null) StopCoroutine(_flash);
            _flash = StartCoroutine(FlashRoutine(color, seconds));
        }

        private IEnumerator FlashRoutine(Color color, float seconds)
        {
            Apply(color);
            yield return new WaitForSecondsRealtime(seconds);
            _flash = null;
            Apply(_steadyColor);
        }

        private void Apply(Color color)
        {
            bool on = color.maxColorComponent > 0.001f;
            if (targetRenderer != null)
            {
                targetRenderer.GetPropertyBlock(_block);
                _block.SetColor(EmissionColorId, color * (on ? intensity : 0f));
                if(affectBaseColor && on)_block.SetColor("_BaseColor",color);
                targetRenderer.SetPropertyBlock(_block);
            }
            if (glowLight != null)
            {
                glowLight.enabled = on;
                if (on) glowLight.color = color;
            }
        }
    }
}
