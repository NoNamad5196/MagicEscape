using System.Collections;
using System.Linq;
using MagicEscape.Core;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
namespace MagicEscape
{
    // One transaction adapter for both VR and desktop. Core remains the authority.
    public sealed class XriPuzzleSocket : XRSocketInteractor
    {
        public PotionPuzzle cauldron;
        public CrystalSocket crystalSocket;
        bool committing;
        protected override void OnEnable() { base.OnEnable(); selectEntered.AddListener(QueueCommit); }
        protected override void OnDisable() { selectEntered.RemoveListener(QueueCommit); base.OnDisable(); }
        public bool Accepts(XriItem item)
        {
            if (item == null || committing || GameManager.Instance == null || !GameManager.Instance.IsPlaying) return false;
            var presentation = FindFirstObjectByType<RoomPresentation>();
            if (presentation != null && presentation.InputBlocked) return false;
            if (cauldron != null) return cauldron.Logic.IsActive && PotionPuzzleLogic.DefaultIngredients.Contains(item.ItemId);
            var seal = FindFirstObjectByType<SealPuzzle>();
            return crystalSocket != null && seal != null && seal.Stage == SealStage.AwaitingCrystals && !seal.Logic.IsSocketFilled(crystalSocket.Side) &&
                (item.ItemId == ItemIds.CrystalSapphire || item.ItemId == ItemIds.CrystalRuby);
        }
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (interactablesSelected.Contains(interactable)) return true;
            var item = interactable.transform.GetComponent<XriItem>();
            return Accepts(item) && (!interactable.isSelected || interactablesSelected.Contains(interactable)) && base.CanSelect(interactable);
        }
        void QueueCommit(SelectEnterEventArgs args)
        {
            var item = args.interactableObject.transform.GetComponent<XriItem>();
            if (item != null && !committing) StartCoroutine(Commit(item));
        }
        IEnumerator Commit(XriItem item)
        {
            committing = true;
            yield return null; // Never mutate a selection inside the manager's select event traversal.
            if (item == null) { committing = false; yield break; }
            if (interactablesSelected.Contains(item.Grab)) interactionManager.SelectExit((IXRSelectInteractor)this, (IXRSelectInteractable)item.Grab);
            var player = FindFirstObjectByType<PlayerInteraction>();
            if (player == null || !item.SelectInventory()) { committing = false; yield break; }
            // Legacy P2 retry logic respawns unavailable pickups, including this attempt's last item.
            item.gameObject.SetActive(false);
            if (cauldron != null) cauldron.Interact(player);
            else crystalSocket.Interact(player);
            if (InventoryManager.Instance.Contains(item.ItemId)) item.Pickup.Respawn();
            committing = false;
        }
    }
}
