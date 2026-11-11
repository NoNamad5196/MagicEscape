using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 조사 가능한 단서 오브젝트(액자 4점, 칠판, 마법서 등). E로 읽기 패널을 연다.
    /// requirePuzzleSolved를 켜면 지정 퍼즐이 Solved 전에는 lockedMessage만 토스트로 보여 준다(마법서 결계).
    /// </summary>
    [DisallowMultipleComponent]
    public class InspectableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptVerb = "조사하기";
        [SerializeField] private string title = "단서";
        [TextArea(3, 8)] [SerializeField] private string body;

        [Header("잠금 조건 (선택)")]
        [SerializeField] private bool requirePuzzleSolved;
        [SerializeField] private PuzzleId requiredPuzzle = PuzzleId.Rune;
        [TextArea(2, 4)] [SerializeField] private string lockedMessage = GameText.RecipeSealed;

        [Tooltip("선택: 잠금 동안 보일 결계 오브젝트(해제 시 비활성화)")]
        [SerializeField] private GameObject barrierVisual;

        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (requirePuzzleSolved && gm != null)
            {
                gm.Progression.PuzzleStateChanged += OnPuzzleStateChanged;
                UpdateBarrier();
            }
        }

        private void OnDestroy()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.Progression != null) gm.Progression.PuzzleStateChanged -= OnPuzzleStateChanged;
        }

        public string GetPrompt(PlayerInteraction interactor) => promptVerb;

        public void Interact(PlayerInteraction interactor)
        {
            if (IsLocked())
            {
                interactor.Say(lockedMessage);
                return;
            }
            if (UIManager.Instance != null) UIManager.Instance.ShowRead(title, body);
        }

        private bool IsLocked()
        {
            if (!requirePuzzleSolved) return false;
            GameManager gm = GameManager.Instance;
            return gm == null || gm.Progression.GetState(requiredPuzzle) != PuzzleState.Solved;
        }

        private void OnPuzzleStateChanged(PuzzleId id, PuzzleState state)
        {
            if (id == requiredPuzzle) UpdateBarrier();
        }

        private void UpdateBarrier()
        {
            if (barrierVisual != null) barrierVisual.SetActive(IsLocked());
        }
    }
}
