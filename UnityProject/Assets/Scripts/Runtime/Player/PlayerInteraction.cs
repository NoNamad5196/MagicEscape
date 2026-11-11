using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 화면 중앙 Raycast로 IInteractable을 찾고, 프롬프트 표시와 E키 상호작용을 처리한다.
    /// 또한 인벤토리 슬롯 선택(1~8/휠), Tab(목표), ESC(일시정지/읽기 닫기) 입력을 GameManager·UI로 전달한다.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [DisallowMultipleComponent]
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private float interactDistance = 2.5f;
        [Tooltip("상호작용 Raycast 대상 레이어. Player 레이어는 제외할 것")]
        [SerializeField] private LayerMask interactMask = ~0;

        private PlayerInputReader _input;
        private IInteractable _current;

        public InventoryManager Inventory => InventoryManager.Instance;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>();
            if (viewCamera == null) Debug.LogError("[PlayerInteraction] viewCamera가 없습니다.", this);
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            UIManager ui = UIManager.Instance;
            if (gm == null || ui == null) return;

            if (_input.PausePressed)
            {
                if (ui.IsReading) ui.CloseRead();
                else gm.TogglePause();
                return;
            }

            if (!gm.IsPlaying)
            {
                ClearFocus(ui);
                return;
            }

            if (ui.IsReading)
            {
                ClearFocus(ui);
                if (_input.InteractPressed) ui.CloseRead();
                return;
            }

            HandleInventoryInput();
            if (_input.ObjectivePressed) ui.ToggleObjective();

            UpdateFocus(ui);
            if (_input.InteractPressed && _current != null)
            {
                _current.Interact(this);
                AudioManager.Play(SfxId.Interact);
            }
        }

        private void HandleInventoryInput()
        {
            InventoryManager inv = Inventory;
            if (inv == null) return;
            if (_input.SlotPressed >= 0) inv.Select(_input.SlotPressed);
            else if (_input.Scroll != 0) inv.SelectNext(-_input.Scroll); // 휠 아래 = 다음 칸
        }

        private void UpdateFocus(UIManager ui)
        {
            _current = null;
            if (viewCamera == null) return;

            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Ignore))
            {
                IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
                if (target != null)
                {
                    string prompt = target.GetPrompt(this);
                    if (!string.IsNullOrEmpty(prompt))
                    {
                        _current = target;
                        ui.SetPrompt(prompt);
                        return;
                    }
                }
            }
            ui.SetPrompt(null);
        }

        private void ClearFocus(UIManager ui)
        {
            _current = null;
            ui.SetPrompt(null);
        }

        /// <summary>상호작용 대상이 플레이어에게 메시지를 띄울 때 사용.</summary>
        public void Say(string message)
        {
            if (UIManager.Instance != null) UIManager.Instance.ShowToast(message);
        }
    }
}
