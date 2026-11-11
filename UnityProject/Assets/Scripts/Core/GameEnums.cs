namespace MagicEscape.Core
{
    /// <summary>게임 전체 흐름 상태. Title → Playing ⇄ Paused → Cleared.</summary>
    public enum GameFlowState
    {
        Title,
        Playing,
        Paused,
        Cleared
    }

    /// <summary>퍼즐 식별자. P1=Rune, P2=Potion, P3=Seal.</summary>
    public enum PuzzleId
    {
        Rune,
        Potion,
        Seal
    }

    /// <summary>퍼즐 진행 상태.</summary>
    public enum PuzzleState
    {
        Locked,
        Active,
        Solved
    }

    /// <summary>제단의 마법 문양 6종.</summary>
    public enum RuneId
    {
        Earth,
        Water,
        Fire,
        Wind,
        Light,
        Shadow
    }

    /// <summary>최종 봉인 장치(P3)의 하위 단계.</summary>
    public enum SealStage
    {
        Dormant,
        AwaitingReveal,
        AwaitingCrystals,
        ReadyToRelease,
        Released
    }

    /// <summary>봉인 장치의 수정 소켓 위치.</summary>
    public enum SocketSide
    {
        Left,
        Right
    }
}
