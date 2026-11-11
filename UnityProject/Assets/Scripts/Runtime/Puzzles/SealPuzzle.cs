using System.Collections;
using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// P3 최종 봉인 장치(출구 앞). 규칙은 Core.SealPuzzleLogic.
    /// 비문(SealInscription) → 소켓 2개(CrystalSocket) → 봉인핵(SealCore) 순으로 진행되며,
    /// 각 상호작용 컴포넌트가 이 클래스의 메서드를 호출한다.
    /// </summary>
    [DisallowMultipleComponent]
    public class SealPuzzle : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("물약 사용 전 숨겨져 있는 비문 글자/문양 오브젝트")]
        [SerializeField] private GameObject revealedInscription;
        [SerializeField] private GlowFeedback inscriptionGlow;
        [SerializeField] private CrystalSocket leftSocket;
        [SerializeField] private CrystalSocket rightSocket;
        [SerializeField] private GlowFeedback coreGlow;
        [SerializeField] private ParticleSystem revealVfx;
        [SerializeField] private ParticleSystem releaseVfx;

        [Header("Tuning")]
        [SerializeField] private int hintThreshold = SealPuzzleLogic.DefaultHintThreshold;
        [SerializeField] private Color readyColor = new Color(0.8f, 0.6f, 1f);
        [SerializeField] private Color releasedColor = new Color(1f, 0.85f, 0.4f);

        public SealPuzzleLogic Logic { get; private set; }
        public SealStage Stage => Logic != null ? Logic.Stage : SealStage.Dormant;
        public string CurrentHint => Logic != null && Logic.ShouldShowHint ? GameText.HintSeal : null;

        private void Awake()
        {
            Logic = new SealPuzzleLogic(hintThreshold);
            Logic.StageChanged += OnStageChanged;
            if (revealedInscription != null) revealedInscription.SetActive(false);
            if (leftSocket == null || rightSocket == null) Debug.LogError("[SealPuzzle] 소켓 참조 누락", this);
            if (leftSocket != null) leftSocket.Bind(this);
            if (rightSocket != null) rightSocket.Bind(this);
        }

        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            gm.Progression.PuzzleStateChanged += OnPuzzleStateChanged;
            if (gm.Progression.GetState(PuzzleId.Seal) == PuzzleState.Active) Logic.Activate();
        }

        private void OnDestroy()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.Progression != null) gm.Progression.PuzzleStateChanged -= OnPuzzleStateChanged;
        }

        private void OnPuzzleStateChanged(PuzzleId id, PuzzleState state)
        {
            if (id == PuzzleId.Seal && state == PuzzleState.Active) Logic.Activate();
        }

        // ---------- 상호작용 컴포넌트에서 호출 ----------

        public void UseInscription(PlayerInteraction interactor)
        {
            if (Stage == SealStage.AwaitingCrystals || Stage == SealStage.ReadyToRelease || Stage == SealStage.Released)
            {
                if (UIManager.Instance != null) UIManager.Instance.ShowRead("봉인 비문", GameText.InscriptionRevealed);
                return;
            }

            InventoryManager inv = interactor.Inventory;
            string itemId = inv != null ? inv.SelectedItemId : null;
            SealActionResult result = Logic.UseOnInscription(itemId);
            Consume(result, inv, itemId);

            switch (result)
            {
                case SealActionResult.Dormant: interactor.Say(GameText.SealRemaining); break;
                case SealActionResult.InvalidItem: interactor.Say(GameText.InscriptionBlurred); break;
                case SealActionResult.Revealed:
                    if (revealedInscription != null) revealedInscription.SetActive(true);
                    if (inscriptionGlow != null) inscriptionGlow.SetGlow(readyColor);
                    if (revealVfx != null) revealVfx.Play();
                    AudioManager.Play(SfxId.SealReveal);
                    if (UIManager.Instance != null) UIManager.Instance.ShowRead("봉인 비문", GameText.InscriptionRevealed);
                    break;
            }
        }

        public void InsertCrystal(CrystalSocket socket, PlayerInteraction interactor)
        {
            InventoryManager inv = interactor.Inventory;
            string itemId = inv != null ? inv.SelectedItemId : null;
            if (Stage == SealStage.AwaitingCrystals && itemId == null)
            {
                interactor.Say(GameText.NoItemSelected);
                return;
            }

            SealActionResult result = Logic.InsertCrystal(socket.Side, itemId);
            Consume(result, inv, itemId);
            Debug.Log("[SealPuzzle] " + socket.Side + " ← " + itemId + " : " + result);

            switch (result)
            {
                case SealActionResult.Dormant: interactor.Say(GameText.SealRemaining); break;
                case SealActionResult.NotAvailable: interactor.Say(GameText.SocketNeedsReveal); break;
                case SealActionResult.AlreadyFilled: interactor.Say(GameText.SocketFilled); break;
                case SealActionResult.InvalidItem: interactor.Say(GameText.CannotInsert); break;
                case SealActionResult.Rejected:
                    socket.ShowRejected();
                    AudioManager.Play(SfxId.SocketReject);
                    interactor.Say(Logic.ShouldShowHint ? GameText.SocketRejected + "\n힌트: " + GameText.HintSeal : GameText.SocketRejected);
                    break;
                case SealActionResult.CrystalPlaced:
                    socket.ShowPlaced();
                    AudioManager.Play(SfxId.SocketPlace);
                    break;
            }
        }

        public void ActivateCore(PlayerInteraction interactor)
        {
            SealActionResult result = Logic.ActivateCore();
            switch (result)
            {
                case SealActionResult.Dormant: interactor.Say(GameText.SealRemaining); break;
                case SealActionResult.NotAvailable: interactor.Say(GameText.CoreNotReady); break;
                case SealActionResult.Released: StartCoroutine(ReleaseSequence(interactor)); break;
            }
        }

        private IEnumerator ReleaseSequence(PlayerInteraction interactor)
        {
            if (coreGlow != null) coreGlow.SetGlow(releasedColor);
            if (releaseVfx != null) releaseVfx.Play();
            AudioManager.Play(SfxId.SealRelease);
            interactor.Say(GameText.SealReleased);
            yield return new WaitForSeconds(0.5f);
            if (GameManager.Instance != null) GameManager.Instance.ReportSolved(PuzzleId.Seal);
        }

        private void OnStageChanged(SealStage stage)
        {
            if (stage == SealStage.ReadyToRelease && coreGlow != null) coreGlow.SetGlow(readyColor);
            if (UIManager.Instance != null) UIManager.Instance.RefreshObjective();
        }

        private static void Consume(SealActionResult result, InventoryManager inv, string itemId)
        {
            if (inv != null && SealPuzzleLogic.ConsumesItem(result)) inv.Remove(itemId);
        }
    }
}
