using UnityEngine;
namespace MagicEscape
{
    public sealed class XriRewardSpawner : MonoBehaviour
    {
        public ItemPickup[] rewards;
        void LateUpdate()
        {
            var inv = InventoryManager.Instance;
            if (inv == null) return;
            foreach (var pickup in rewards)
                if (pickup != null && inv.Contains(pickup.ItemId) && !pickup.gameObject.activeSelf) pickup.Reveal();
        }
    }
}
