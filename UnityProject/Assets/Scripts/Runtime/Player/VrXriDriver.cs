using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
namespace MagicEscape
{
    [DefaultExecutionOrder(-50)]
    public sealed class VrXriDriver : MonoBehaviour
    {
        public PlayerInputReader input;
        public XRRayInteractor left, right, teleport;
        public SnapTurnProvider turn;
        void Update()
        {
            var gm = GameManager.Instance; var p = FindFirstObjectByType<RoomPresentation>();
            bool allowed = gm != null && gm.IsPlaying && (UIManager.Instance == null || !UIManager.Instance.IsReading) && (p == null || !p.InputBlocked);
            bool uiAllowed = p == null || !p.InputBlocked;
            left.uiPressInput.QueueManualState(uiAllowed && input.LeftSelect, uiAllowed && input.LeftSelect ? 1 : 0);
            right.uiPressInput.QueueManualState(uiAllowed && input.RightSelect, uiAllowed && input.RightSelect ? 1 : 0);
            left.selectInput.QueueManualState(allowed && input.LeftSelect, allowed && input.LeftSelect ? 1 : 0);
            right.selectInput.QueueManualState(allowed && input.RightSelect, allowed && input.RightSelect ? 1 : 0);
            teleport.selectInput.QueueManualState(allowed && input.TeleportHeld, allowed && input.TeleportHeld ? 1 : 0);
            turn.rightHandTurnInput.manualValue = allowed ? input.SnapTurn : Vector2.zero;
        }
    }
}
