using System.Collections;
using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// P1 마법 문양 해독(중앙 제단). 규칙은 Core.RunePuzzleLogic, 이 컴포넌트는 발광·사운드·보상 연출만 담당.
    /// 정답: 대지 → 물 → 불 → 빛. 오답 시 전체 소등 + 빨간 점멸, 3회 실패부터 힌트.
    /// </summary>
    [DisallowMultipleComponent]
    public class RunePuzzle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RuneStone[] runeStones = new RuneStone[6];
        [Tooltip("제단 뚜껑(완료 시 localPosition += lidOpenOffset 로 이동)")]
        [SerializeField] private Transform altarLid;
        [SerializeField] private ItemPickup rewardCrystal;
        [SerializeField] private ParticleSystem solvedVfx;

        [Header("Tuning")]
        [SerializeField] private Vector3 lidOpenOffset = new Vector3(0f, 0f, 0.6f);
        [SerializeField] private float lidOpenSeconds = 1.5f;
        [SerializeField] private int hintThreshold = RunePuzzleLogic.DefaultHintThreshold;
        [SerializeField] private Color litColor = new Color(0.3f, 0.8f, 1f);
        [SerializeField] private Color wrongColor = new Color(1f, 0.15f, 0.1f);
        [SerializeField] private Color solvedColor = new Color(1f, 0.8f, 0.3f);

        public RunePuzzleLogic Logic { get; private set; }

        /// <summary>실패 기준 이상이면 힌트 문구, 아니면 null.</summary>
        public string CurrentHint => Logic != null && Logic.ShouldShowHint ? GameText.HintRune : null;
        public bool IsSolved => Logic != null && Logic.IsSolved;

        private void Awake()
        {
            Logic = new RunePuzzleLogic(RunePuzzleLogic.DefaultAnswer, hintThreshold);
            if (rewardCrystal == null) Debug.LogError("[RunePuzzle] rewardCrystal(푸른 수정 ItemPickup) 미지정", this);
            foreach (RuneStone stone in runeStones)
            {
                if (stone == null) Debug.LogError("[RunePuzzle] runeStones 배열에 빈 칸이 있습니다.", this);
                else stone.Bind(this);
            }
        }

        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            if (gm.Progression.GetState(PuzzleId.Rune) == PuzzleState.Active) Logic.Activate();
        }

        /// <summary>RuneStone이 E 입력 시 호출.</summary>
        public void Press(RuneStone stone, PlayerInteraction interactor)
        {
            RuneInputResult result = Logic.Press(stone.Rune);
            var clues=FindFirstObjectByType<VisualClues>();
            if(clues!=null)clues.ShowRuneProgress(Logic.ProgressCount);
            switch (result)
            {
                case RuneInputResult.Progress:
                    stone.Glow.SetGlow(litColor);
                    AudioManager.Play(SfxId.RunePress);
                    break;

                case RuneInputResult.Wrong:
                    foreach (RuneStone s in runeStones)
                    {
                        if (s == null) continue;
                        s.Glow.Off();
                        s.Glow.Flash(wrongColor);
                    }
                    AudioManager.Play(SfxId.RuneWrong);
                    interactor.Say(Logic.ShouldShowHint ? GameText.RuneWrong + "\n힌트: " + GameText.HintRune : GameText.RuneWrong);
                    break;

                case RuneInputResult.Solved:
                    stone.Glow.SetGlow(litColor);
                    AudioManager.Play(SfxId.RunePress);
                    StartCoroutine(SolveSequence(interactor));
                    break;
            }
            Debug.Log("[RunePuzzle] " + stone.Rune + " → " + result + " (진행 " + Logic.ProgressCount + ", 실패 " + Logic.FailCount + ")");
        }

        private IEnumerator SolveSequence(PlayerInteraction interactor)
        {
            yield return new WaitForSeconds(0.4f);
            foreach (RuneStone s in runeStones) if (s != null) s.Glow.SetGlow(solvedColor);
            AudioManager.Play(SfxId.RuneSolved);
            if (solvedVfx != null) solvedVfx.Play();

            if (altarLid != null)
            {
                Vector3 from = altarLid.localPosition;
                Vector3 to = from + lidOpenOffset;
                for (float t = 0f; t < 1f; t += Time.deltaTime / Mathf.Max(0.01f, lidOpenSeconds))
                {
                    altarLid.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                    yield return null;
                }
                altarLid.localPosition = to;
            }

            if (rewardCrystal != null) rewardCrystal.Reveal();
            interactor.Say(GameText.RuneSolved);
            if (GameManager.Instance != null) GameManager.Instance.ReportSolved(PuzzleId.Rune);
        }
    }
}
