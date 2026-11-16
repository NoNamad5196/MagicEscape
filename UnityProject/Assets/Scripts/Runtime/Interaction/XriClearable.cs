using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    public enum ClearingKind { Slide, Hinge, Cloth }
    // One manager-owned selection for both rigs. Heavy props never follow the camera in Y.
    public sealed class XriClearable : XRSimpleInteractable
    {
        public ClearingKind kind;
        public Vector3 closedPosition, openPosition;
        public Vector3 openEuler;
        public Vector3 minPosition, maxPosition;
        public float revealDistance = .65f;
        public float Progress { get; private set; }
        public bool IsCleared => Progress >= .95f;
        Vector3 handStart, startPosition;
        bool opening;
        VrXriDriver vr;
        Vector3 previousLeft, previousRight;
        bool handsTracked;
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            handStart = args.interactorObject.GetAttachTransform(this).position;
            startPosition = transform.position;
            if (kind != ClearingKind.Slide) opening = true;
        }
        void Update()
        {
            var gm = GameManager.Instance;
            var ui = UIManager.Instance;
            if (gm == null || !gm.IsPlaying || (ui != null && ui.IsReading)) return;
            if (kind == ClearingKind.Slide)
            {
                if (isSelected)
                {
                    var hand = interactorsSelecting[0].GetAttachTransform(this).position;
                    MoveWithinRails(startPosition + hand - handStart);
                }
                else PushWithTrackedHands();
                Progress = Mathf.Clamp01(Vector3.Distance(transform.position, closedPosition) / revealDistance);
            }
            else if (opening)
            {
                Progress = Mathf.MoveTowards(Progress, 1, Time.deltaTime * 1.8f);
                if (kind == ClearingKind.Hinge) transform.localRotation = Quaternion.Euler(openEuler * Progress);
                else
                {
                    transform.localPosition = Vector3.Lerp(closedPosition, openPosition, Progress);
                    transform.localScale = new Vector3(1, Mathf.Lerp(1, .12f, Progress), 1);
                }
            }
        }
        void PushWithTrackedHands()
        {
            if (vr == null) vr = FindFirstObjectByType<VrXriDriver>();
            if (vr == null || !vr.isActiveAndEnabled) { handsTracked=false; return; }
            var left=vr.left.transform.position; var right=vr.right.transform.position;
            if (handsTracked)
            {
                PushContact(left,previousLeft); PushContact(right,previousRight);
            }
            previousLeft=left;previousRight=right;handsTracked=true;
        }
        void PushContact(Vector3 hand,Vector3 previous)
        {
            var delta=hand-previous;
            // Ignore tracking jumps / teleportation; a virtual 9cm palm supplies contact only.
            if (delta.sqrMagnitude>.09f || delta.sqrMagnitude<.000001f) return;
            var body=GetComponent<Collider>();
            if (body != null && Vector3.Distance(body.ClosestPoint(previous),previous)<.09f)
                MoveWithinRails(transform.position+delta);
        }
        // Rails are wholly in a west-wall storage bay, never in the puzzle/exit circulation area.
        public void MoveWithinRails(Vector3 desired)
        {
            transform.position = new Vector3(Mathf.Clamp(desired.x,minPosition.x,maxPosition.x), closedPosition.y,
                Mathf.Clamp(desired.z,minPosition.z,maxPosition.z));
        }
        public void ResetProp()
        {
            opening = false; Progress = 0;
            if (kind == ClearingKind.Slide) transform.position = closedPosition;
            else { transform.localPosition = closedPosition; transform.localRotation = Quaternion.identity; transform.localScale = Vector3.one; }
        }
    }
}
