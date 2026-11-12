using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MagicEscape
{
    /// <summary>
    /// 모든 키보드/마우스 입력을 읽는 유일한 지점. (VR 확장 시 이 컴포넌트만 교체)
    /// Active Input Handling이 "Input System Package (New)", "Input Manager (Old)", "Both" 어느 것이어도 동작한다.
    /// 다른 스크립트보다 먼저 실행되도록 DefaultExecutionOrder를 낮게 둔다.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public class PlayerInputReader : MonoBehaviour
    {
#pragma warning disable 0414 // 구 Input Manager 단독 빌드에서는 사용되지 않음(Inspector 값 유지를 위해 항상 직렬화)
        [Tooltip("새 Input System의 마우스 delta(픽셀)를 구 Input Manager 'Mouse X' 단위로 맞추는 배율")]
        [SerializeField] private float newInputMouseScale = 0.1f;
#pragma warning restore 0414

        /// <summary>x = 좌우(A/D), y = 앞뒤(W/S). 길이 ≤ 1.</summary>
        public Vector2 Move { get; private set; }
        /// <summary>이번 프레임 마우스 이동량(구 Input Manager 단위).</summary>
        public Vector2 Look { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool PausePressed { get; private set; }
        public bool ObjectivePressed { get; private set; }
        public bool AnyPressed { get; private set; }
        /// <summary>숫자키 1~8로 선택한 슬롯(0-based). 없으면 -1.</summary>
        public int SlotPressed { get; private set; }
        /// <summary>휠 방향: +1(위) / -1(아래) / 0.</summary>
        public int Scroll { get; private set; }

        private void Update()
        {
            SlotPressed = -1;
#if ENABLE_INPUT_SYSTEM
            ReadNewInputSystem();
#elif ENABLE_LEGACY_INPUT_MANAGER
            ReadLegacyInput();
#else
            Debug.LogError("[PlayerInputReader] 활성화된 입력 백엔드가 없습니다. Player Settings > Active Input Handling을 확인하세요.");
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void ReadNewInputSystem()
        {
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;
            AnyPressed = kb != null && kb.anyKey.wasPressedThisFrame;
            if (kb == null)
            {
                ClearAll();
                return;
            }

            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float y = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            Move = Vector2.ClampMagnitude(new Vector2(x, y), 1f);
            Look = mouse != null ? mouse.delta.ReadValue() * newInputMouseScale : Vector2.zero;

            InteractPressed = kb.eKey.wasPressedThisFrame;
            PausePressed = kb.escapeKey.wasPressedThisFrame;
            ObjectivePressed = kb.tabKey.wasPressedThisFrame;

            float wheel = mouse != null ? mouse.scroll.ReadValue().y : 0;
            Scroll = wheel > 0.01f ? 1 : wheel < -0.01f ? -1 : 0;

            for (int i = 0; i < SlotKeys.Length; i++)
            {
                if (kb[SlotKeys[i]].wasPressedThisFrame) { SlotPressed = i; break; }
            }
        }

        private static readonly Key[] SlotKeys =
        {
            Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8
        };
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        private void ReadLegacyInput()
        {
            AnyPressed = Input.anyKeyDown;
            Move = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
            Look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            InteractPressed = Input.GetKeyDown(KeyCode.E);
            PausePressed = Input.GetKeyDown(KeyCode.Escape);
            ObjectivePressed = Input.GetKeyDown(KeyCode.Tab);

            float wheel = Input.mouseScrollDelta.y;
            Scroll = wheel > 0.01f ? 1 : wheel < -0.01f ? -1 : 0;

            for (int i = 0; i < 8; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { SlotPressed = i; break; }
            }
        }
#endif

        private void ClearAll()
        {
            Move = Vector2.zero;
            Look = Vector2.zero;
            InteractPressed = PausePressed = ObjectivePressed = false;
            AnyPressed = false;
            Scroll = 0;
        }
    }
}
