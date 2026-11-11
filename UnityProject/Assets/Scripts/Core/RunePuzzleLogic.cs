using System;
using System.Collections.Generic;

namespace MagicEscape.Core
{
    /// <summary>문양 입력 결과.</summary>
    public enum RuneInputResult
    {
        /// <summary>비활성/해결 완료 상태라 무시됨.</summary>
        Ignored,
        /// <summary>올바른 다음 문양.</summary>
        Progress,
        /// <summary>오답. 진행이 0으로 리셋되고 FailCount 증가.</summary>
        Wrong,
        /// <summary>마지막 문양까지 정답 → 퍼즐 해결.</summary>
        Solved
    }

    /// <summary>
    /// P1 마법 문양 해독 규칙. 입력 즉시 정답과 비교하고, 틀리면 처음부터 다시 입력해야 한다.
    /// </summary>
    public sealed class RunePuzzleLogic
    {
        /// <summary>정본 정답: Earth → Water → Fire → Light.</summary>
        public static readonly RuneId[] DefaultAnswer = { RuneId.Earth, RuneId.Water, RuneId.Fire, RuneId.Light };
        public const int DefaultHintThreshold = 3;

        private readonly RuneId[] _answer;
        private readonly int _hintThreshold;

        /// <summary>정답 문양 입력 시 (현재 진행 개수).</summary>
        public event Action<int> Progressed;
        /// <summary>오답 시 (누적 실패 횟수).</summary>
        public event Action<int> Failed;
        public event Action Solved;

        public RunePuzzleLogic(IReadOnlyList<RuneId> answer, int hintThreshold = DefaultHintThreshold)
        {
            if (answer == null) throw new ArgumentNullException(nameof(answer));
            if (answer.Count == 0) throw new ArgumentException("정답 문양이 비어 있다.", nameof(answer));
            _answer = new RuneId[answer.Count];
            for (int i = 0; i < answer.Count; i++) _answer[i] = answer[i];
            _hintThreshold = Math.Max(1, hintThreshold);
        }

        /// <summary>정본 정답으로 생성.</summary>
        public static RunePuzzleLogic CreateDefault() => new RunePuzzleLogic(DefaultAnswer);

        public int AnswerLength => _answer.Length;
        public int ProgressCount { get; private set; }
        public int FailCount { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsSolved { get; private set; }
        public bool ShouldShowHint => FailCount >= _hintThreshold;

        /// <summary>퍼즐을 입력 가능 상태로 만든다(ProgressionModel이 Active일 때 호출).</summary>
        public void Activate()
        {
            if (!IsSolved) IsActive = true;
        }

        /// <summary>정답 순서의 i번째 문양(연출·디버그용).</summary>
        public RuneId GetAnswerAt(int index) => _answer[index];

        public RuneInputResult Press(RuneId rune)
        {
            if (!IsActive || IsSolved) return RuneInputResult.Ignored;

            if (_answer[ProgressCount] != rune)
            {
                ProgressCount = 0;
                FailCount++;
                Failed?.Invoke(FailCount);
                return RuneInputResult.Wrong;
            }

            ProgressCount++;
            if (ProgressCount >= _answer.Length)
            {
                IsSolved = true;
                IsActive = false;
                Solved?.Invoke();
                return RuneInputResult.Solved;
            }

            Progressed?.Invoke(ProgressCount);
            return RuneInputResult.Progress;
        }
    }
}
