using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class XriPress : MonoBehaviour
    {
        XRSimpleInteractable press;
        void Awake() { press = GetComponent<XRSimpleInteractable>(); press.selectEntered.AddListener(Press); }
        void OnDestroy() { if (press != null) press.selectEntered.RemoveListener(Press); }
        void Press(SelectEnterEventArgs args)
        {
            if (!ClearingGate.Allows(this)) return;
            var gm = GameManager.Instance; var ui = UIManager.Instance;
            var presentation = FindFirstObjectByType<RoomPresentation>();
            if (gm == null || !gm.IsPlaying || (ui != null && ui.IsReading) || (presentation != null && presentation.InputBlocked)) return;
            var target = GetComponent<IInteractable>();
            var player = FindFirstObjectByType<PlayerInteraction>();
            if (target != null && player != null)
            {
                var inv = player.Inventory;
                string selected = inv != null ? inv.SelectedItemId : null;
                target.Interact(player);
                // A free hand may apply an inventory potion while the other hand holds it.
                // Consumption must retire that same physical item in either rig, not just in the PC driver.
                if (selected != null && !inv.Contains(selected)) StartCoroutine(RetireConsumed(selected));
            }
        }
        IEnumerator RetireConsumed(string itemId)
        {
            yield return null;
            foreach (var item in FindObjectsByType<XriItem>(FindObjectsSortMode.None))
            {
                if (item.ItemId != itemId) continue;
                foreach (var interactor in item.Grab.interactorsSelecting.ToArray())
                    item.Grab.interactionManager.SelectExit(interactor, (IXRSelectInteractable)item.Grab);
                item.gameObject.SetActive(false);
            }
        }
    }
}
