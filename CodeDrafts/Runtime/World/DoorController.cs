using System.Collections;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 출구 문 + 봉인 빛 3개. 해결된 퍼즐 수만큼 봉인 빛을 끄고, Open() 시 경첩을 회전시켜 문을 연 뒤 ExitTrigger를 켠다.
    /// 계층: ExitDoor → DoorHinge(회전축, 문 모델은 그 자식) / SealLight_1..3 / ExitTrigger.
    /// </summary>
    [DisallowMultipleComponent]
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Transform hinge;
        [SerializeField] private float openAngle = -100f;
        [SerializeField] private float openSeconds = 2.0f;
        [SerializeField] private float openDelay = 1.0f;
        [Tooltip("문 위 봉인 빛 3개(순서 = P1, P2, P3)")]
        [SerializeField] private GameObject[] sealLights = new GameObject[3];
        [SerializeField] private ExitTrigger exitTrigger;
        [SerializeField] private ParticleSystem openVfx;

        public bool IsOpen { get; private set; }
        public bool PassageReady => exitTrigger != null && exitTrigger.gameObject.activeSelf;

        private void Awake()
        {
            if (hinge == null) Debug.LogError("[DoorController] hinge 미지정", this);
            if (exitTrigger != null) exitTrigger.gameObject.SetActive(false);
        }

        /// <summary>해결된 퍼즐 수(0~3)만큼 앞에서부터 봉인 빛을 끈다.</summary>
        public void SetSolvedCount(int solved)
        {
            for (int i = 0; i < sealLights.Length; i++)
            {
                if (sealLights[i] != null) sealLights[i].SetActive(i >= solved);
            }
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            StartCoroutine(OpenRoutine());
        }

        private IEnumerator OpenRoutine()
        {
            yield return new WaitForSeconds(openDelay);
            AudioManager.Play(SfxId.DoorOpen);
            if (openVfx != null) openVfx.Play();

            if (hinge != null)
            {
                Quaternion from = hinge.localRotation;
                Quaternion to = from * Quaternion.Euler(0f, openAngle, 0f);
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / Mathf.Max(0.01f, openSeconds);
                    hinge.localRotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                    yield return null;
                }
            }

            if (exitTrigger != null) exitTrigger.gameObject.SetActive(true);
            Debug.Log("[DoorController] 출구 개방 완료");
        }
    }
}
