using System;
using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// Core.InventoryModel 래퍼. itemId ↔ ItemData(아이콘·이름) 조회를 담당한다.
    /// itemCatalog에 7개 ItemData를 모두 등록해야 아이콘이 표시된다.
    /// </summary>
    [DefaultExecutionOrder(-60)]
    [DisallowMultipleComponent]
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField] private int capacity = 8;
        [SerializeField] private ItemData[] itemCatalog;

        public InventoryModel Model { get; private set; }

        /// <summary>목록 또는 선택 슬롯 변경 시(HotbarUI 갱신용).</summary>
        public event Action Changed;

        public string SelectedItemId => Model.SelectedItemId;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("[InventoryManager] 중복 인스턴스 제거", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Model = new InventoryModel(capacity);
            Model.Changed += () => Changed?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool TryAdd(ItemData item)
        {
            if (item == null || string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogError("[InventoryManager] itemId가 비어 있는 ItemData를 추가하려 했습니다.", this);
                return false;
            }
            return TryAdd(item.itemId);
        }

        public bool TryAdd(string itemId)
        {
            bool ok = Model.TryAdd(itemId);
            if (ok) AudioManager.Play(SfxId.Pickup);
            return ok;
        }

        public bool Remove(string itemId) => Model.Remove(itemId);
        public bool Contains(string itemId) => Model.Contains(itemId);
        public void Select(int index) => Model.Select(index);
        public void SelectNext(int direction) => Model.SelectNext(direction);

        /// <summary>itemId에 해당하는 ItemData. 카탈로그에 없으면 null.</summary>
        public ItemData GetData(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || itemCatalog == null) return null;
            foreach (ItemData data in itemCatalog)
            {
                if (data != null && data.itemId == itemId) return data;
            }
            Debug.LogWarning("[InventoryManager] 카탈로그에 없는 itemId: " + itemId, this);
            return null;
        }

        public string GetDisplayName(string itemId)
        {
            ItemData data = GetData(itemId);
            return data != null && !string.IsNullOrEmpty(data.displayName) ? data.displayName : itemId;
        }
    }
}
