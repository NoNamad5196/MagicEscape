using System;
using System.Collections.Generic;

namespace MagicEscape.Core
{
    /// <summary>가마솥 투입 결과.</summary>
    public enum PotionInputResult
    {
        /// <summary>퍼즐이 Locked(가마솥이 차가움). 소모·실패 없음.</summary>
        NotActive,
        /// <summary>재료가 아닌 아이템(수정 등). 소모·실패 없음.</summary>
        NotIngredient,
        /// <summary>올바른 다음 재료. 소모됨.</summary>
        Accepted,
        /// <summary>순서 오류 또는 독버섯. 소모되며 LastFailedBatch 전체를 원위치에 재생성해야 함.</summary>
        Wrong,
        /// <summary>마지막 재료까지 정답. 소모됨.</summary>
        Solved
    }

    /// <summary>
    /// P2 마법약 제조 규칙. 재료를 정해진 순서로 넣어야 하며, 틀리면 이번 시도의 재료가 모두 원위치로 돌아간다.
    /// </summary>
    public sealed class PotionPuzzleLogic
    {
        public static readonly string[] DefaultRecipe =
        {
            ItemIds.Moonpetal, ItemIds.SalamanderScale, ItemIds.CrystalDust
        };

        public static readonly string[] DefaultIngredients =
        {
            ItemIds.Moonpetal, ItemIds.SalamanderScale, ItemIds.CrystalDust, ItemIds.Toadstool
        };

        public const int DefaultHintThreshold = 2;

        private readonly string[] _recipe;
        private readonly HashSet<string> _ingredients;
        private readonly int _hintThreshold;
        private readonly List<string> _currentBatch = new List<string>();
        private string[] _lastFailedBatch = new string[0];

        public event Action<int> Progressed;
        /// <summary>오답 시 (누적 실패 횟수). 재생성 대상은 LastFailedBatch.</summary>
        public event Action<int> Failed;
        public event Action Solved;

        public PotionPuzzleLogic(IReadOnlyList<string> recipe, IEnumerable<string> validIngredients,
            int hintThreshold = DefaultHintThreshold)
        {
            if (recipe == null) throw new ArgumentNullException(nameof(recipe));
            if (validIngredients == null) throw new ArgumentNullException(nameof(validIngredients));
            if (recipe.Count == 0) throw new ArgumentException("조합법이 비어 있다.", nameof(recipe));

            _recipe = new string[recipe.Count];
            for (int i = 0; i < recipe.Count; i++) _recipe[i] = recipe[i];
            _ingredients = new HashSet<string>(validIngredients);
            foreach (string id in _recipe) _ingredients.Add(id); // 조합법 재료는 항상 재료로 취급
            _hintThreshold = Math.Max(1, hintThreshold);
        }

        public static PotionPuzzleLogic CreateDefault() => new PotionPuzzleLogic(DefaultRecipe, DefaultIngredients);

        public int RecipeLength => _recipe.Length;
        public int ProgressCount => _currentBatch.Count;
        public int FailCount { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsSolved { get; private set; }
        public bool ShouldShowHint => FailCount >= _hintThreshold;

        /// <summary>현재 시도에서 투입된 재료(정답 진행분).</summary>
        public IReadOnlyList<string> CurrentBatch => _currentBatch;

        /// <summary>직전 오답 시도에서 투입된 모든 재료(틀린 재료 포함). 원위치 재생성 대상.</summary>
        public IReadOnlyList<string> LastFailedBatch => _lastFailedBatch;

        public void Activate()
        {
            if (!IsSolved) IsActive = true;
        }

        public bool IsIngredient(string itemId) => !string.IsNullOrEmpty(itemId) && _ingredients.Contains(itemId);

        /// <summary>결과에 따라 인벤토리에서 아이템을 소모해야 하는지.</summary>
        public static bool ConsumesItem(PotionInputResult result) =>
            result == PotionInputResult.Accepted || result == PotionInputResult.Wrong || result == PotionInputResult.Solved;

        public PotionInputResult Add(string itemId)
        {
            if (!IsActive || IsSolved) return PotionInputResult.NotActive;
            if (!IsIngredient(itemId)) return PotionInputResult.NotIngredient;

            _currentBatch.Add(itemId);
            int index = _currentBatch.Count - 1;

            if (_recipe[index] != itemId)
            {
                _lastFailedBatch = _currentBatch.ToArray();
                _currentBatch.Clear();
                FailCount++;
                Failed?.Invoke(FailCount);
                return PotionInputResult.Wrong;
            }

            if (_currentBatch.Count >= _recipe.Length)
            {
                IsSolved = true;
                IsActive = false;
                Solved?.Invoke();
                return PotionInputResult.Solved;
            }

            Progressed?.Invoke(_currentBatch.Count);
            return PotionInputResult.Accepted;
        }
    }
}
