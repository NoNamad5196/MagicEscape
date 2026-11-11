using System;

namespace MagicEscape.Core
{
    /// <summary>
    /// 게임 진행 정본: 흐름 상태, 퍼즐 3종의 잠금/해결 상태, 경과 시간.
    /// 시작: Rune=Active, Potion=Locked, Seal=Locked, Flow=Title.
    /// Rune 해결 → Potion Active. Rune·Potion 해결 → Seal Active. Seal 해결 → 출구 개방.
    /// </summary>
    public sealed class ProgressionModel
    {
        private readonly PuzzleState[] _states = { PuzzleState.Active, PuzzleState.Locked, PuzzleState.Locked };

        public event Action<PuzzleId, PuzzleState> PuzzleStateChanged;
        public event Action<GameFlowState> FlowStateChanged;
        public event Action ExitOpened;

        public GameFlowState Flow { get; private set; } = GameFlowState.Title;
        public float ElapsedSeconds { get; private set; }
        public bool ExitOpen => GetState(PuzzleId.Seal) == PuzzleState.Solved;

        /// <summary>해결된 퍼즐 수(0~3). 출구 봉인 빛 소등 개수와 같다.</summary>
        public int SolvedCount
        {
            get
            {
                int n = 0;
                foreach (PuzzleState s in _states) if (s == PuzzleState.Solved) n++;
                return n;
            }
        }

        public PuzzleState GetState(PuzzleId id) => _states[(int)id];

        public bool StartGame()
        {
            if (Flow != GameFlowState.Title) return false;
            SetFlow(GameFlowState.Playing);
            return true;
        }

        public bool Pause()
        {
            if (Flow != GameFlowState.Playing) return false;
            SetFlow(GameFlowState.Paused);
            return true;
        }

        public bool Resume()
        {
            if (Flow != GameFlowState.Paused) return false;
            SetFlow(GameFlowState.Playing);
            return true;
        }

        /// <summary>Playing 중일 때만 시간 누적.</summary>
        public void Tick(float deltaSeconds)
        {
            if (Flow == GameFlowState.Playing && deltaSeconds > 0f) ElapsedSeconds += deltaSeconds;
        }

        /// <summary>퍼즐 해결 보고. 해당 퍼즐이 Active가 아니면 무시(false).</summary>
        public bool MarkSolved(PuzzleId id)
        {
            if (GetState(id) != PuzzleState.Active) return false;
            SetState(id, PuzzleState.Solved);

            if (id == PuzzleId.Rune && GetState(PuzzleId.Potion) == PuzzleState.Locked)
                SetState(PuzzleId.Potion, PuzzleState.Active);

            if (GetState(PuzzleId.Rune) == PuzzleState.Solved &&
                GetState(PuzzleId.Potion) == PuzzleState.Solved &&
                GetState(PuzzleId.Seal) == PuzzleState.Locked)
                SetState(PuzzleId.Seal, PuzzleState.Active);

            if (id == PuzzleId.Seal) ExitOpened?.Invoke();
            return true;
        }

        /// <summary>출구 통과. 출구가 열려 있고 Playing일 때만 Cleared.</summary>
        public bool Escape()
        {
            if (!ExitOpen || Flow != GameFlowState.Playing) return false;
            SetFlow(GameFlowState.Cleared);
            return true;
        }

        private void SetState(PuzzleId id, PuzzleState state)
        {
            _states[(int)id] = state;
            PuzzleStateChanged?.Invoke(id, state);
        }

        private void SetFlow(GameFlowState next)
        {
            Flow = next;
            FlowStateChanged?.Invoke(next);
        }
    }
}
