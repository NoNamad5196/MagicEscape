using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MagicEscape
{
    /// <summary>
    /// 화면 하단 8칸 인벤토리 바. InventoryManager.Changed를 받아 아이콘·선택 테두리·선택 아이템 이름을 갱신한다.
    /// slotIcons/selectionFrames 배열 길이는 인벤토리 칸 수(8)와 같게 연결한다.
    /// </summary>
    [DisallowMultipleComponent]
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private GameObject[] selectionFrames;
        [SerializeField] private TMP_Text selectedNameText;

        private InventoryManager _inventory;

        private void Start()
        {
            _inventory = InventoryManager.Instance;
            if (_inventory == null)
            {
                Debug.LogError("[HotbarUI] InventoryManager가 씬에 없습니다.", this);
                return;
            }
            _inventory.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (_inventory != null) _inventory.Changed -= Refresh;
        }

        private void Refresh()
        {
            var model = _inventory.Model;
            for (int i = 0; i < slotIcons.Length; i++)
            {
                Image icon = slotIcons[i];
                if (icon == null) continue;
                ItemData data = i < model.Count ? _inventory.GetData(model.Items[i]) : null;
                icon.enabled = data != null && data.icon != null;
                icon.sprite = data != null ? data.icon : null;
            }

            if (selectionFrames != null)
            {
                for (int i = 0; i < selectionFrames.Length; i++)
                {
                    if (selectionFrames[i] != null) selectionFrames[i].SetActive(i == model.SelectedIndex);
                }
            }

            if (selectedNameText != null)
            {
                string id = model.SelectedItemId;
                selectedNameText.text = id == null ? string.Empty : _inventory.GetDisplayName(id);
            }
        }
    }
}
