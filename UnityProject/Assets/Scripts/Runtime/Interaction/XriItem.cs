using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    [RequireComponent(typeof(XRGrabInteractable), typeof(ItemPickup))]
    public sealed class XriItem : MonoBehaviour
    {
        public ItemPickup Pickup { get; private set; }
        public XRGrabInteractable Grab { get; private set; }
        public string ItemId => Pickup.ItemId;
        void Awake()
        {
            // Shared carry layer must not push either rig's CharacterController.
            Physics.IgnoreLayerCollision(8, 10, true);
            Pickup = GetComponent<ItemPickup>(); Grab = GetComponent<XRGrabInteractable>();
            Grab.selectEntered.AddListener(OnGrab); Grab.selectExited.AddListener(OnRelease);
        }
        void OnDestroy() { if (Grab != null) { Grab.selectEntered.RemoveListener(OnGrab); Grab.selectExited.RemoveListener(OnRelease); } }
        void OnGrab(SelectEnterEventArgs args)
        {
            if (args.interactorObject is XriPuzzleSocket) return;
            foreach (var collider in Grab.colliders) collider.gameObject.layer = 10;
            SelectInventory();
        }
        void OnRelease(SelectExitEventArgs args) { foreach (var collider in Grab.colliders) collider.gameObject.layer = 9; }
        public bool SelectInventory()
        {
            if (!ClearingGate.Allows(this)) return false;
            var inv = InventoryManager.Instance;
            if (inv == null || (!inv.Contains(ItemId) && !inv.TryAdd(Pickup.Item))) return false;
            inv.Select(inv.Model.Items.ToList().IndexOf(ItemId));
            return true;
        }
    }
}
