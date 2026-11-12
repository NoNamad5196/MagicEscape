using MagicEscape.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicEscape
{
    /// <summary>
    /// 게임 진행 총괄(=PuzzleManager 역할 통합). Core.ProgressionModel을 소유하고
    /// 흐름 상태(타이틀/플레이/일시정지/클리어)에 따라 커서·시간·플레이어 조작·UI를 맞춘다.
    /// 퍼즐 컴포넌트는 해결 시 ReportSolved를 호출하고, Active 전환은 PuzzleStateChanged로 받는다.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private RunePuzzle runePuzzle;
        [SerializeField] private PotionPuzzle potionPuzzle;
        [SerializeField] private SealPuzzle sealPuzzle;
        [SerializeField] private DoorController exitDoor;

        [Header("Debug")]
        [Tooltip("에디터 테스트용: 타이틀을 건너뛰고 바로 시작")]
        [SerializeField] private bool skipTitle;

        public ProgressionModel Progression { get; private set; }
        public bool IsPlaying => Progression != null && Progression.Flow == GameFlowState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("[GameManager] 씬에 GameManager가 2개 이상입니다. 중복을 제거합니다.", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Progression = new ProgressionModel();
            Progression.FlowStateChanged += OnFlowStateChanged;
            Progression.PuzzleStateChanged += OnPuzzleStateChanged;
            Progression.ExitOpened += OnExitOpened;
            ValidateReferences();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            if (UIManager.Instance != null) UIManager.Instance.ReadingChanged += _ => ApplyControlState();
            if (exitDoor != null) exitDoor.SetSolvedCount(0);
            OnFlowStateChanged(Progression.Flow);
            if (skipTitle) StartGame();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            Progression.Tick(Time.deltaTime);
        }

        // ---------- UI 버튼/입력에서 호출 ----------

        public void StartGame() => Progression.StartGame();

        public void TogglePause()
        {
            if (!Progression.Pause()) Progression.Resume();
        }

        public void Resume() => Progression.Resume();

        public void Restart()
        {
            Time.timeScale = 1f;
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ---------- 퍼즐/월드에서 호출 ----------

        /// <summary>퍼즐 해결 보고. 순서가 맞지 않으면 무시되고 경고 로그.</summary>
        public void ReportSolved(PuzzleId id)
        {
            if (!Progression.MarkSolved(id))
                Debug.LogWarning("[GameManager] ReportSolved(" + id + ") 무시됨: 현재 상태 " + Progression.GetState(id), this);
        }

        /// <summary>ExitTrigger 진입 시 호출.</summary>
        public void NotifyPlayerEscaped()
        {
            if (Progression.Escape()) AudioManager.Play(SfxId.Clear);
        }

        /// <summary>Tab 패널에 표시할 현재 목표.</summary>
        public string GetObjectiveText()
        {
            if (Progression.ExitOpen) return GameText.ObjectiveEscape;
            if (Progression.GetState(PuzzleId.Rune) == PuzzleState.Active) return GameText.ObjectiveRune;
            if (Progression.GetState(PuzzleId.Potion) == PuzzleState.Active) return GameText.ObjectivePotion;
            if (sealPuzzle != null)
            {
                switch (sealPuzzle.Stage)
                {
                    case SealStage.AwaitingCrystals: return GameText.ObjectiveSealCrystals;
                    case SealStage.ReadyToRelease: return GameText.ObjectiveSealCore;
                }
            }
            return GameText.ObjectiveSealReveal;
        }

        /// <summary>현재 활성 퍼즐의 힌트(실패 횟수 기준 미달이면 null).</summary>
        public string GetCurrentHint()
        {
            if (Progression.GetState(PuzzleId.Rune) == PuzzleState.Active && runePuzzle != null) return runePuzzle.CurrentHint;
            if (Progression.GetState(PuzzleId.Potion) == PuzzleState.Active && potionPuzzle != null) return potionPuzzle.CurrentHint;
            if (Progression.GetState(PuzzleId.Seal) == PuzzleState.Active && sealPuzzle != null) return sealPuzzle.CurrentHint;
            return null;
        }

        /// <summary>Explicit Tab safety net, independent of automatic failure hints.</summary>
        public string GetVisualClueText()
        {
            if(Progression.GetState(PuzzleId.Rune)==PuzzleState.Active)return GameText.Chalkboard;
            if(Progression.GetState(PuzzleId.Potion)==PuzzleState.Active)return GameText.Recipe;
            if(sealPuzzle!=null && sealPuzzle.Stage>=SealStage.AwaitingCrystals)return GameText.InscriptionRevealed;
            return GetCurrentHint();
        }

        // ---------- 내부 ----------

        private void OnFlowStateChanged(GameFlowState state)
        {
            Time.timeScale = state == GameFlowState.Paused ? 0f : 1f;
            ApplyControlState();
            UIManager ui = UIManager.Instance;
            if (ui != null)
            {
                ui.ShowFlow(state);
                if (state == GameFlowState.Cleared) ui.ShowClear(Progression.ElapsedSeconds);
            }
            if (state == GameFlowState.Playing) AudioManager.PlayBgm();
            Debug.Log("[GameManager] Flow → " + state);
        }

        private void OnPuzzleStateChanged(PuzzleId id, PuzzleState state)
        {
            if (exitDoor != null) exitDoor.SetSolvedCount(Progression.SolvedCount);
            if (UIManager.Instance != null) UIManager.Instance.RefreshObjective();
            Debug.Log("[GameManager] " + id + " → " + state);
        }

        private void OnExitOpened()
        {
            if (exitDoor != null) exitDoor.Open();
            else Debug.LogError("[GameManager] exitDoor 미지정 — 출구를 열 수 없습니다.", this);
        }

        private void ApplyControlState()
        {
            bool reading = UIManager.Instance != null && UIManager.Instance.IsReading;
            bool control = IsPlaying && !reading;
            if (player != null) player.ControlEnabled = control;
            Cursor.lockState = control ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !control;
        }

        private void ValidateReferences()
        {
            if (player == null) Debug.LogError("[GameManager] player 미지정", this);
            if (runePuzzle == null) Debug.LogError("[GameManager] runePuzzle 미지정", this);
            if (potionPuzzle == null) Debug.LogError("[GameManager] potionPuzzle 미지정", this);
            if (sealPuzzle == null) Debug.LogError("[GameManager] sealPuzzle 미지정", this);
            if (exitDoor == null) Debug.LogError("[GameManager] exitDoor 미지정", this);
        }
    }
}
