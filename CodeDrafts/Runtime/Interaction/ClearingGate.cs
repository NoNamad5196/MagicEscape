using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
namespace MagicEscape
{
    // Defense in depth: collider + XRI registration + direct legacy adapter are all gated.
    [DefaultExecutionOrder(-50)]
    public sealed class ClearingGate : MonoBehaviour
    {
        public XriClearable blocker;
        public XriLooseProp books;
        public bool IsAccessible => (blocker == null || blocker.IsCleared) && (books == null || books.IsCleared);
        void Awake() { Apply(); }
        void OnEnable() { Apply(); }
        void Update() { Apply(); }
        void Apply()
        {
            bool visible = IsAccessible;
            foreach (var c in GetComponents<Collider>()) c.enabled = visible;
            foreach (var x in GetComponents<XRBaseInteractable>()) x.enabled = visible;
        }
        public static bool Allows(Component target)
        {
            var gate = target.GetComponent<ClearingGate>();
            return gate == null || gate.IsAccessible;
        }
    }
}
