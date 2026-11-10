using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 제단 위 문양석 1개(총 6개). E로 누르면 RunePuzzle에 입력을 전달한다.
    /// 같은 GameObject(또는 자식)에 Collider와 GlowFeedback이 필요하다.
    /// </summary>
    [RequireComponent(typeof(GlowFeedback))]
    [DisallowMultipleComponent]
    public class RuneStone : MonoBehaviour, IInteractable
    {
        [SerializeField] private RuneId rune;

        private RunePuzzle _puzzle;

        public RuneId Rune => rune;
        public GlowFeedback Glow { get; private set; }

        private void Awake()
        {
            Glow = GetComponent<GlowFeedback>();
        }

        /// <summary>RunePuzzle.Awake에서 호출.</summary>
        public void Bind(RunePuzzle puzzle) => _puzzle = puzzle;

        public string GetPrompt(PlayerInteraction interactor)
        {
            if (_puzzle == null || _puzzle.IsSolved) return null;
            return KoreanName(rune) + " 문양 누르기";
        }

        public void Interact(PlayerInteraction interactor)
        {
            if (_puzzle == null)
            {
                Debug.LogError("[RuneStone] RunePuzzle에 연결되지 않았습니다: " + name, this);
                return;
            }
            _puzzle.Press(this, interactor);
        }

        public static string KoreanName(RuneId id)
        {
            switch (id)
            {
                case RuneId.Earth: return "대지";
                case RuneId.Water: return "물";
                case RuneId.Fire: return "불";
                case RuneId.Wind: return "바람";
                case RuneId.Light: return "빛";
                case RuneId.Shadow: return "그림자";
                default: return id.ToString();
            }
        }
    }
}
