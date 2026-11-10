using System;
using System.Collections.Generic;

namespace MagicEscape.Core
{
    /// <summary>
    /// 고정 칸 수 인벤토리(순수 로직). 아이템은 itemId 문자열로 보관한다.
    /// 선택 슬롯(SelectedIndex)의 아이템이 가마솥·봉인 장치에 사용된다.
    /// </summary>
    public sealed class InventoryModel
    {
        private readonly List<string> _items = new List<string>();

        /// <summary>보관 목록·선택 슬롯이 바뀔 때마다 발생.</summary>
        public event Action Changed;

        public InventoryModel(int capacity = 8)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            Capacity = capacity;
        }

        public int Capacity { get; }
        public int Count => _items.Count;
        public bool IsFull => _items.Count >= Capacity;
        public IReadOnlyList<string> Items => _items;

        /// <summary>선택된 슬롯 인덱스(0 ~ Capacity-1). 빈 슬롯을 가리킬 수 있다.</summary>
        public int SelectedIndex { get; private set; }

        /// <summary>선택된 슬롯의 itemId. 비어 있으면 null.</summary>
        public string SelectedItemId => SelectedIndex < _items.Count ? _items[SelectedIndex] : null;

        /// <summary>아이템 추가. null/빈 문자열이거나 가득 차면 false.</summary>
        public bool TryAdd(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || IsFull) return false;
            _items.Add(itemId);
            Changed?.Invoke();
            return true;
        }

        public bool Contains(string itemId) => !string.IsNullOrEmpty(itemId) && _items.Contains(itemId);

        /// <summary>해당 itemId 하나를 제거. 없으면 false.</summary>
        public bool Remove(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            int index = _items.IndexOf(itemId);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>인덱스의 아이템 제거. 뒤 아이템이 앞으로 당겨지며, 선택 인덱스는 같은 칸에 머문다(범위를 넘으면 마지막 아이템으로).</summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _items.Count) return;
            _items.RemoveAt(index);
            if (SelectedIndex >= _items.Count && _items.Count > 0) SelectedIndex = _items.Count - 1;
            if (_items.Count == 0) SelectedIndex = 0;
            Changed?.Invoke();
        }

        /// <summary>슬롯 선택. 0 ~ Capacity-1 범위 밖이면 무시.</summary>
        public void Select(int index)
        {
            if (index < 0 || index >= Capacity || index == SelectedIndex) return;
            SelectedIndex = index;
            Changed?.Invoke();
        }

        /// <summary>마우스 휠용 순환 선택(+1 / -1).</summary>
        public void SelectNext(int direction)
        {
            if (direction == 0) return;
            int next = ((SelectedIndex + Math.Sign(direction)) % Capacity + Capacity) % Capacity;
            Select(next);
        }

        public void Clear()
        {
            _items.Clear();
            SelectedIndex = 0;
            Changed?.Invoke();
        }
    }
}
