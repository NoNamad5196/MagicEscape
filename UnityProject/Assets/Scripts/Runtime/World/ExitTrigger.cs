using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 문 바깥의 탈출 판정 영역. BoxCollider(Is Trigger) 필요. 문이 열릴 때 DoorController가 활성화한다.
    /// CharacterController는 Rigidbody 없이도 Trigger 이벤트를 발생시킨다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ExitTrigger : MonoBehaviour
    {
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerController>() == null) return;
            if (GameManager.Instance != null) GameManager.Instance.NotifyPlayerEscaped();
        }
    }
}
