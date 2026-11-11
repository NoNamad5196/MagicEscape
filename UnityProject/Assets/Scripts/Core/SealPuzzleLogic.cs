using System;

namespace MagicEscape.Core
{
    /// <summary>봉인 장치 상호작용 결과.</summary>
    public enum SealActionResult
    {
        /// <summary>P3가 아직 Locked(Dormant). "봉인이 두 개 남아 있다" 안내. 소모 없음.</summary>
        Dormant,
        /// <summary>현재 단계에서 할 수 없는 행동(예: 비문 해독 전 소켓). 소모 없음.</summary>
        NotAvailable,
        /// <summary>이 단계에 맞지 않는 아이템(물약 아닌 것으로 비문 조사 등). 소모 없음.</summary>
        InvalidItem,
        /// <summary>비문 해독 성공. 물약 소모.</summary>
        Revealed,
        /// <summary>수정이 올바른 소켓에 고정됨. 수정 소모.</summary>
        CrystalPlaced,
        /// <summary>수정이 틀린 소켓 → 거부. 소모 없음, FailCount 증가.</summary>
        Rejected,
        /// <summary>이미 채워진 소켓. 소모 없음.</summary>
        AlreadyFilled,
        /// <summary>봉인핵 활성화 → 최종 봉인 해제.</summary>
        Released
    }

    /// <summary>
    /// P3 최종 봉인 해제 규칙.
    /// Dormant → AwaitingReveal(물약으로 비문 해독) → AwaitingCrystals(Left=사파이어, Right=루비)
    /// → ReadyToRelease(봉인핵 상호작용) → Released.
    /// </summary>
    public sealed class SealPuzzleLogic
    {
        public const int DefaultHintThreshold = 2;

        private readonly int _hintThreshold;
        private bool _leftFilled;
        private bool _rightFilled;

        public event Action<SealStage> StageChanged;
        public event Action<int> Failed;

        public SealPuzzleLogic(int hintThreshold = DefaultHintThreshold)
        {
            _hintThreshold = Math.Max(1, hintThreshold);
        }

        public SealStage Stage { get; private set; } = SealStage.Dormant;
        public int FailCount { get; private set; }
        public bool ShouldShowHint => FailCount >= _hintThreshold;
        public bool IsReleased => Stage == SealStage.Released;

        public bool IsSocketFilled(SocketSide side) => side == SocketSide.Left ? _leftFilled : _rightFilled;

        /// <summary>소켓에 맞는 수정 itemId.</summary>
        public static string ExpectedCrystal(SocketSide side) =>
            side == SocketSide.Left ? ItemIds.CrystalSapphire : ItemIds.CrystalRuby;

        /// <summary>인벤토리에서 아이템을 소모해야 하는 결과인지.</summary>
        public static bool ConsumesItem(SealActionResult result) =>
            result == SealActionResult.Revealed || result == SealActionResult.CrystalPlaced;

        /// <summary>P3가 Active가 되면 호출. Dormant → AwaitingReveal.</summary>
        public void Activate()
        {
            if (Stage == SealStage.Dormant) SetStage(SealStage.AwaitingReveal);
        }

        /// <summary>비문판에 선택 아이템 사용.</summary>
        public SealActionResult UseOnInscription(string itemId)
        {
            if (Stage == SealStage.Dormant) return SealActionResult.Dormant;
            if (Stage != SealStage.AwaitingReveal) return SealActionResult.NotAvailable;
            if (itemId != ItemIds.PotionRevelation) return SealActionResult.InvalidItem;

            SetStage(SealStage.AwaitingCrystals);
            return SealActionResult.Revealed;
        }

        /// <summary>소켓에 선택 아이템(수정) 삽입.</summary>
        public SealActionResult InsertCrystal(SocketSide side, string itemId)
        {
            if (Stage == SealStage.Dormant) return SealActionResult.Dormant;
            if (Stage != SealStage.AwaitingCrystals)
            {
                return IsSocketFilled(side) ? SealActionResult.AlreadyFilled : SealActionResult.NotAvailable;
            }
            if (IsSocketFilled(side)) return SealActionResult.AlreadyFilled;
            if (itemId != ItemIds.CrystalSapphire && itemId != ItemIds.CrystalRuby) return SealActionResult.InvalidItem;

            if (itemId != ExpectedCrystal(side))
            {
                FailCount++;
                Failed?.Invoke(FailCount);
                return SealActionResult.Rejected;
            }

            if (side == SocketSide.Left) _leftFilled = true;
            else _rightFilled = true;

            if (_leftFilled && _rightFilled) SetStage(SealStage.ReadyToRelease);
            return SealActionResult.CrystalPlaced;
        }

        /// <summary>중앙 봉인핵 상호작용 = 최종 조건 검증.</summary>
        public SealActionResult ActivateCore()
        {
            if (Stage == SealStage.Dormant) return SealActionResult.Dormant;
            if (Stage != SealStage.ReadyToRelease) return SealActionResult.NotAvailable;
            SetStage(SealStage.Released);
            return SealActionResult.Released;
        }

        private void SetStage(SealStage next)
        {
            if (Stage == next) return;
            Stage = next;
            StageChanged?.Invoke(next);
        }
    }
}
