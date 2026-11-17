using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    // Non-inventory dressing uses the same XRGrabInteractable in PC and VR.
    // On release it parks on a supported surface; it can neither fly nor fall through the floor.
    public sealed class XriLooseProp : MonoBehaviour
    {
        public Vector3 home, parking;
        public Vector3 parkingEuler;
        public bool IsCleared => Vector3.Distance(transform.position, home) > .27f;
        XRGrabInteractable grab;
        void Awake()
        {
            grab = GetComponent<XRGrabInteractable>();
            grab.selectExited.AddListener(Park);
            grab.selectEntered.AddListener(Carry);
            Physics.IgnoreLayerCollision(8,10,true);
        }
        void OnDestroy()
        {
            if (grab == null) return;
            grab.selectExited.RemoveListener(Park);
            grab.selectEntered.RemoveListener(Carry);
        }
        void Carry(SelectEnterEventArgs args) { gameObject.layer=10; }
        void Park(SelectExitEventArgs args) { gameObject.layer=9; transform.SetPositionAndRotation(parking, Quaternion.Euler(parkingEuler)); }
        public void ResetProp()
        {
            if(grab != null && grab.isSelected) grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
            gameObject.layer=9;transform.SetPositionAndRotation(home,Quaternion.identity);
        }
    }
}
