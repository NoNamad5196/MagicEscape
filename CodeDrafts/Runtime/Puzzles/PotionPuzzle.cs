using System.Collections;
using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// P2 마법약 제조(가마솥에 부착). 선택 슬롯의 아이템을 E로 투입한다.
    /// 정답: 월광초 꽃잎 → 불도마뱀 비늘 → 수정 가루. 오답 시 이번 시도 재료를 원위치에 재생성.
    /// 완료 시 계시의 물약 + 붉은 수정을 인벤토리에 직접 지급.
    /// </summary>
    [DisallowMultipleComponent]
    public class PotionPuzzle : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [Tooltip("교실의 재료 ItemPickup 4개(독버섯 포함). 오답 시 재생성 대상")]
        [SerializeField] private ItemPickup[] ingredientPickups = new ItemPickup[4];
        [SerializeField] private GlowFeedback liquidGlow;
        [SerializeField] private ParticleSystem bubbleVfx;
        [SerializeField] private ParticleSystem smokeVfx;
        [SerializeField] private ParticleSystem successVfx;

        [Header("Tuning")]
        [SerializeField] private int hintThreshold = PotionPuzzleLogic.DefaultHintThreshold;
        [Tooltip("재료 1·2·3개 투입 후 물약 색")]
        [SerializeField] private Color[] stepColors =
        {
            new Color(0.55f, 0.6f, 1f), new Color(1f, 0.45f, 0.2f), new Color(0.3f, 1f, 0.5f)
        };
        [SerializeField] private Color solvedColor = new Color(1f, 0.8f, 0.3f);

        public PotionPuzzleLogic Logic { get; private set; }
        public string CurrentHint => Logic != null && Logic.ShouldShowHint ? GameText.HintPotion : null;

        private void Awake()
        {
            Logic = new PotionPuzzleLogic(PotionPuzzleLogic.DefaultRecipe, PotionPuzzleLogic.DefaultIngredients, hintThreshold);
        }

        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            gm.Progression.PuzzleStateChanged += OnPuzzleStateChanged;
            if (gm.Progression.GetState(PuzzleId.Potion) == PuzzleState.Active) Activate();
        }

        private void OnDestroy()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.Progression != null) gm.Progression.PuzzleStateChanged -= OnPuzzleStateChanged;
        }

        private void OnPuzzleStateChanged(PuzzleId id, PuzzleState state)
        {
            if (id == PuzzleId.Potion && state == PuzzleState.Active) Activate();
        }

        private void Activate()
        {
            Logic.Activate();
            if (bubbleVfx != null) bubbleVfx.Play();
        }

        public string GetPrompt(PlayerInteraction interactor)
        {
            if (Logic.IsSolved) return null;
            InventoryManager inv = interactor.Inventory;
            string selected = inv != null ? inv.SelectedItemId : null;
            if (!Logic.IsActive || selected == null) return "가마솥 살펴보기";
            return inv.GetDisplayName(selected) + " 넣기";
        }

        public void Interact(PlayerInteraction interactor)
        {
            InventoryManager inv = interactor.Inventory;
            if (inv == null) return;
            string itemId = inv.SelectedItemId;

            if (Logic.IsActive && itemId == null)
            {
                interactor.Say(GameText.NoItemSelected);
                return;
            }

            PotionInputResult result = Logic.Add(itemId);
            if (PotionPuzzleLogic.ConsumesItem(result)) inv.Remove(itemId);
            Debug.Log("[PotionPuzzle] " + itemId + " → " + result + " (진행 " + Logic.ProgressCount + ", 실패 " + Logic.FailCount + ")");

            switch (result)
            {
                case PotionInputResult.NotActive:
                    interactor.Say(GameText.CauldronCold);
                    break;
                case PotionInputResult.NotIngredient:
                    interactor.Say(GameText.CannotInsert);
                    break;
                case PotionInputResult.Accepted:
                    SetLiquidColor(Logic.ProgressCount);
                    AudioManager.Play(SfxId.CauldronAccept);
                    interactor.Say(inv.GetDisplayName(itemId) + "을(를) 넣었다. (" + Logic.ProgressCount + "/" + Logic.RecipeLength + ")");
                    break;
                case PotionInputResult.Wrong:
                    HandleWrong(interactor);
                    break;
                case PotionInputResult.Solved:
                    SetLiquidColor(Logic.RecipeLength);
                    AudioManager.Play(SfxId.CauldronAccept);
                    StartCoroutine(SolveSequence(interactor, inv));
                    break;
            }
        }

        private void HandleWrong(PlayerInteraction interactor)
        {
            if (liquidGlow != null) liquidGlow.Off();
            if (smokeVfx != null) smokeVfx.Play();
            AudioManager.Play(SfxId.CauldronWrong);

            foreach (string id in Logic.LastFailedBatch) RespawnIngredient(id);
            interactor.Say(Logic.ShouldShowHint ? GameText.PotionWrong + "\n힌트: " + GameText.HintPotion : GameText.PotionWrong);
        }

        private void RespawnIngredient(string itemId)
        {
            foreach (ItemPickup pickup in ingredientPickups)
            {
                if (pickup != null && pickup.ItemId == itemId && !pickup.IsAvailable)
                {
                    pickup.Respawn();
                    return;
                }
            }
            Debug.LogWarning("[PotionPuzzle] 재생성할 ItemPickup을 찾지 못함: " + itemId, this);
        }

        private IEnumerator SolveSequence(PlayerInteraction interactor, InventoryManager inv)
        {
            yield return new WaitForSeconds(0.8f);
            if (liquidGlow != null) liquidGlow.SetGlow(solvedColor);
            if (successVfx != null) successVfx.Play();
            AudioManager.Play(SfxId.PotionSolved);

            if (!inv.TryAdd(ItemIds.PotionRevelation) || !inv.TryAdd(ItemIds.CrystalRuby))
                Debug.LogError("[PotionPuzzle] 보상 지급 실패(인벤토리 가득) — 칸 수를 확인하세요.", this);

            interactor.Say(GameText.PotionSolved);
            if (GameManager.Instance != null) GameManager.Instance.ReportSolved(PuzzleId.Potion);
        }

        private void SetLiquidColor(int step)
        {
            if (liquidGlow == null || stepColors == null || stepColors.Length == 0) return;
            int index = Mathf.Clamp(step - 1, 0, stepColors.Length - 1);
            liquidGlow.SetGlow(stepColors[index]);
        }
    }
}
