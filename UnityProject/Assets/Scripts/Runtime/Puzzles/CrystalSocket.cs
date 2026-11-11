using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 봉인 장치의 수정 소켓(Left=물/사파이어, Right=불/루비). SealPuzzle.Awake에서 Bind된다.
    /// placedCrystalVisual은 시작 시 숨겨 두고 정답 삽입 시 표시한다.
    /// </summary>
    [DisallowMultipleComponent]
    public class CrystalSocket : MonoBehaviour, IInteractable
    {
        [SerializeField] private SocketSide side;
        [SerializeField] private GameObject placedCrystalVisual;
        [SerializeField] private GlowFeedback glow;
        [SerializeField] private Color placedColor = new Color(0.4f, 0.8f, 1f);
        [SerializeField] private Color rejectColor = new Color(1f, 0.15f, 0.1f);

        private SealPuzzle _puzzle;

        public SocketSide Side => side;

        private void Awake()
        {
            if (placedCrystalVisual != null) placedCrystalVisual.SetActive(false);
        }

        public void Bind(SealPuzzle puzzle) => _puzzle = puzzle;

        public string GetPrompt(PlayerInteraction interactor)
        {
            if (_puzzle == null || _puzzle.Logic.IsSocketFilled(side)) return null;
            return _puzzle.Stage == SealStage.AwaitingCrystals ? "수정 끼우기" : "소켓 살펴보기";
        }

        public void Interact(PlayerInteraction interactor)
        {
            if (_puzzle != null) _puzzle.InsertCrystal(this, interactor);
            else Debug.LogError("[CrystalSocket] SealPuzzle에 연결되지 않았습니다: " + name, this);
        }

        public void ShowPlaced()
        {
            if (placedCrystalVisual != null) placedCrystalVisual.SetActive(true);
            if (glow != null) glow.SetGlow(placedColor);
        }

        public void ShowRejected()
        {
            if (glow != null) glow.Flash(rejectColor);
        }
    }
}
