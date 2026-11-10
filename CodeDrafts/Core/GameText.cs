namespace MagicEscape.Core
{
    /// <summary>
    /// 게임 내 고정 문구 정본(AGENTS.md §5와 동일). UI와 문서가 같은 문구를 쓰도록 한 곳에 모은다.
    /// </summary>
    public static class GameText
    {
        // 공통
        public const string InventoryFull = "인벤토리가 가득 찼다.";
        public const string NoItemSelected = "손에 든 아이템이 없다. 숫자키나 휠로 아이템을 선택하자.";

        // P1 — 마법 문양
        public const string Chalkboard = "생명은 정해진 순서로 깨어난다. 액자의 숫자를 따르라.";
        public const string HintRune = "벽에 걸린 액자의 숫자 순서를 따라가 보자.";
        public const string RuneWrong = "문양의 빛이 꺼졌다. 순서가 틀린 것 같다.";
        public const string RuneSolved = "제단의 봉인이 풀렸다! 제단 안에서 푸른 수정이 떠오른다.";

        // P2 — 마법약
        public const string Recipe =
            "계시의 물약 — ① 달빛 아래 핀 꽃잎 ② 불꽃 속에서 벗겨낸 비늘 ③ 별처럼 빛나는 가루. 독버섯은 결코 넣지 말 것.";
        public const string RecipeSealed = "마법서가 빛의 결계에 싸여 있다. 제단의 봉인을 먼저 풀어야 할 것 같다.";
        public const string CauldronCold = "가마솥이 차갑다. 제단의 봉인이 먼저인 것 같다.";
        public const string CannotInsert = "이건 넣을 수 없다.";
        public const string PotionWrong = "가마솥에서 검은 연기가 피어올랐다! 재료들이 제자리로 돌아갔다.";
        public const string HintPotion = "마법서의 순서를 다시 읽어 보자. 독버섯은 함정이다.";
        public const string PotionSolved = "계시의 물약이 완성되었다! 가마솥 바닥에서 붉은 수정도 얻었다.";

        // P3 — 최종 봉인
        public const string SealRemaining = "아직 봉인이 두 개 남아 있다.";
        public const string InscriptionBlurred = "비문이 흐릿해 읽을 수 없다.";
        public const string InscriptionRevealed = "왼쪽엔 흐르는 것을, 오른쪽엔 타오르는 것을.";
        public const string SocketRejected = "수정이 튕겨 나왔다. 이 자리가 아닌 것 같다.";
        public const string SocketNeedsReveal = "소켓이 무엇을 원하는지 아직 알 수 없다. 비문을 먼저 읽어야 한다.";
        public const string SocketFilled = "이미 수정이 끼워져 있다.";
        public const string HintSeal = "물은 흐르고 불은 타오른다. 수정의 색을 보자.";
        public const string CoreNotReady = "봉인핵이 아직 잠잠하다. 두 수정을 모두 끼워야 한다.";
        public const string SealReleased = "마지막 봉인이 풀렸다! 출구가 열린다.";

        // 엔딩
        public const string Ending = "봉인을 모두 풀고 교실을 탈출했다. 오늘의 실습은 만점이다!";
    }
}
