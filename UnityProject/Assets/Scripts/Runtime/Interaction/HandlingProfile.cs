using UnityEngine;
namespace MagicEscape
{
    public enum HandlingWeight { Lift, PushPull, Fixed }
    // Stage 3 extension data only. No pushing, clearing or occlusion behaviour yet.
    public sealed class HandlingProfile : MonoBehaviour
    {
        public HandlingWeight weight = HandlingWeight.Fixed;
    }
}
