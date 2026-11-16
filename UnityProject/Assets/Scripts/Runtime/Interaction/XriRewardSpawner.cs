using UnityEngine;
using System.Collections.Generic;
namespace MagicEscape
{
    public sealed class XriRewardSpawner : MonoBehaviour
    {
        public ItemPickup[] rewards;
        readonly HashSet<ItemPickup> spawned = new HashSet<ItemPickup>();
        void LateUpdate()
        {
            var inv = InventoryManager.Instance;
            if (inv == null) return;
            foreach (var pickup in rewards)
                if (pickup != null && inv.Contains(pickup.ItemId) && spawned.Add(pickup)) pickup.Reveal();
        }
    }
}
