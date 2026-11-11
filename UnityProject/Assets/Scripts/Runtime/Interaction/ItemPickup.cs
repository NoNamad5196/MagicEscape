using MagicEscape.Core;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 줍는 아이템(재료 4종, P1 보상 푸른 수정). 획득 시 GameObject를 비활성화하고,
    /// 마법약 오답 시 PotionPuzzle이 Respawn()으로 원위치에 되살린다.
    /// </summary>
    [DisallowMultipleComponent]
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData item;
        [SerializeField] private string promptVerb = "줍기";
        [Tooltip("시작 시 숨김(예: P1 보상 수정은 제단이 열릴 때 Reveal)")]
        [SerializeField] private bool hiddenAtStart;

        private Vector3 _homePosition;
        private Quaternion _homeRotation;

        public ItemData Item => item;
        public string ItemId => item != null ? item.itemId : null;
        public bool IsAvailable => gameObject.activeSelf;

        private void Awake()
        {
            _homePosition = transform.position;
            _homeRotation = transform.rotation;
            if (item == null) Debug.LogError("[ItemPickup] ItemData 미지정: " + name, this);
            if (hiddenAtStart) gameObject.SetActive(false);
        }

        public string GetPrompt(PlayerInteraction interactor)
        {
            string itemName = item != null && !string.IsNullOrEmpty(item.displayName) ? item.displayName : name;
            return itemName + " " + promptVerb;
        }

        public void Interact(PlayerInteraction interactor)
        {
            InventoryManager inv = interactor.Inventory;
            if (inv == null || item == null) return;
            if (!inv.TryAdd(item))
            {
                interactor.Say(GameText.InventoryFull);
                return;
            }
            interactor.Say(item.displayName + " 획득");
            gameObject.SetActive(false);
        }

        /// <summary>원래 위치·회전으로 되돌려 다시 보이게 한다.</summary>
        public void Respawn()
        {
            transform.SetPositionAndRotation(_homePosition, _homeRotation);
            gameObject.SetActive(true);
        }

        /// <summary>숨겨진 보상 아이템을 등장시킨다.</summary>
        public void Reveal() => gameObject.SetActive(true);
    }
}
