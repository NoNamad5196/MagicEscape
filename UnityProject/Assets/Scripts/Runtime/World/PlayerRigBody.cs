using UnityEngine;
namespace MagicEscape
{
    // Shared player identity for the exit trigger; VR head tracking can move within the origin.
    public sealed class PlayerRigBody : MonoBehaviour
    {
        public Transform head;
        void LateUpdate()
        {
            var cc = GetComponent<CharacterController>();
            if (head == null || cc == null) return;
            var p = transform.InverseTransformPoint(head.position);
            cc.height = Mathf.Clamp(p.y, .6f, 2.2f);
            cc.center = new Vector3(p.x, cc.height * .5f, p.z);
        }
    }
}
