using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>봉인 장치의 중앙 봉인핵. 두 수정이 끼워진 뒤 E로 최종 해제.</summary>
    [DisallowMultipleComponent]
    public class SealCore : MonoBehaviour, IInteractable
    {
        [SerializeField] private SealPuzzle puzzle;

        public string GetPrompt(PlayerInteraction interactor)
        {
            if (puzzle == null || puzzle.Stage == SealStage.Released) return null;
            return puzzle.Stage == SealStage.ReadyToRelease ? "봉인핵 활성화" : "봉인핵 살펴보기";
        }

        public void Interact(PlayerInteraction interactor)
        {
            if (puzzle != null) puzzle.ActivateCore(interactor);
            else Debug.LogError("[SealCore] puzzle 미지정", this);
        }
    }
}
