using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace MagicEscape
{
    // Only the rig knows about PC input. Items use the very same XRI components as VR.
    public sealed class DesktopXriDriver : MonoBehaviour
    {
        public XRRayInteractor ray;
        public PlayerInputReader input;
        readonly List<IXRInteractable> targets = new List<IXRInteractable>();
        public XriItem Held => ray != null && ray.interactablesSelected.Count > 0 ? ray.interactablesSelected[0].transform.GetComponent<XriItem>() : null;
        void Update()
        {
            if (ray == null || input == null) return;
            var gm = GameManager.Instance; var ui = UIManager.Instance;
            var p = FindFirstObjectByType<RoomPresentation>();
            bool allowed = gm != null && gm.IsPlaying && (ui == null || !ui.IsReading) && (p == null || (!p.InputBlocked && !p.SkippedThisFrame));
            if (!allowed) { Drop(); return; }
            if (ray.hasSelection && ray.interactablesSelected[0] is XriClearable moving && moving.kind == ClearingKind.Slide && !input.PrimaryHeld) Drop();
            // InputReader owns hardware sampling; click toggles a manager-owned XRI selection.
            if (input.PrimaryPressed) Click();
            if (input.DropPressed) Drop();
        }
        public void Click()
        {
            if (!ray.TryGetCurrent3DRaycastHit(out var hit)) return;
            if (ray.hasSelection && Held == null) { Drop(); return; }
            var held = Held;
            if (held != null)
            {
                var socket = hit.collider.GetComponentInParent<XriPuzzleSocket>();
                if (socket != null && socket.Accepts(held))
                {
                    ray.EndManualInteraction();
                    ray.interactionManager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)held.Grab);
                }
                var press = hit.collider.GetComponentInParent<XRSimpleInteractable>();
                if (press != null)
                {
                    held.SelectInventory(); ray.EndManualInteraction();
                    ray.StartManualInteraction((IXRSelectInteractable)press); ray.EndManualInteraction();
                    if (!InventoryManager.Instance.Contains(held.ItemId)) held.gameObject.SetActive(false);
                }
                return;
            }
            ray.GetValidTargets(targets);
            foreach (var target in targets)
            {
                if (target is XRGrabInteractable grab) { ray.StartManualInteraction((IXRSelectInteractable)grab); return; }
                if (target is XriClearable clearing)
                {
                    ray.StartManualInteraction((IXRSelectInteractable)clearing);
                    if (clearing.kind != ClearingKind.Slide) ray.EndManualInteraction();
                    return;
                }
                if (target is XRSimpleInteractable press)
                {
                    ray.StartManualInteraction((IXRSelectInteractable)press); ray.EndManualInteraction(); return;
                }
            }
        }
        public void Drop() { if (ray.hasSelection) ray.EndManualInteraction(); }
        void OnDisable() { if (ray != null && ray.hasSelection) ray.EndManualInteraction(); }
    }
}
