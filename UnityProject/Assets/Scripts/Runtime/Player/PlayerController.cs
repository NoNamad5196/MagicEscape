using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 1인칭 이동(CharacterController) + 마우스 시점. GameManager가 ControlEnabled로 켜고 끈다.
    /// 계층: Player(이 컴포넌트, CharacterController) → CameraPivot(Main Camera).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraPivot;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float gravity = -15f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private bool invertY;

        private CharacterController _controller;
        private PlayerInputReader _input;
        private float _pitch;
        private float _verticalVelocity;

        /// <summary>false면 이동·시점 회전을 하지 않는다(타이틀/일시정지/읽기 패널/클리어).</summary>
        public bool ControlEnabled { get; set; }

        public Transform CameraPivot => cameraPivot;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputReader>();
            if (cameraPivot == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraPivot = cam.transform;
                Debug.LogWarning("[PlayerController] cameraPivot 미지정 → 자식 카메라를 사용합니다.", this);
            }
        }

        private void Update()
        {
            ApplyGravityAndMove(ControlEnabled ? _input.Move : Vector2.zero);
            if (ControlEnabled) ApplyLook(_input.Look);
        }

        private void ApplyGravityAndMove(Vector2 move)
        {
            if (_controller.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 horizontal = (transform.right * move.x + transform.forward * move.y) * walkSpeed;
            Vector3 velocity = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void ApplyLook(Vector2 look)
        {
            if (cameraPivot == null) return;
            float yaw = look.x * mouseSensitivity;
            float pitchDelta = look.y * mouseSensitivity * (invertY ? 1f : -1f);

            transform.Rotate(0f, yaw, 0f, Space.Self);
            _pitch = Mathf.Clamp(_pitch + pitchDelta, minPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}
