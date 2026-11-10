using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>봉인 장치의 비문판. E로 물약 사용(해독 전) 또는 비문 읽기(해독 후).</summary>
    [DisallowMultipleComponent]
    public class SealInscription : MonoBehaviour, IInteractable
    {
        [SerializeField] private SealPuzzle puzzle;

        public string GetPrompt(PlayerInteraction interactor)
        {
            if (puzzle == null) return null;
            switch (puzzle.Stage)
            {
                case SealStage.Dormant: return "봉인 장치 살펴보기";
                case SealStage.AwaitingReveal: return "비문 조사하기";
                default: return "비문 읽기";
            }
        }

        public void Interact(PlayerInteraction interactor)
        {
            if (puzzle != null) puzzle.UseInscription(interactor);
            else Debug.LogError("[SealInscription] puzzle 미지정", this);
        }
    }
}
