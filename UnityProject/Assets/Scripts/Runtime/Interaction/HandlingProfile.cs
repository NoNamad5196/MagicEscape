using UnityEngine;
namespace MagicEscape
{
    public enum HandlingWeight { Lift, PushPull, Fixed }
    // Weight classification shared by the builder and both XRI rigs.
    public sealed class HandlingProfile : MonoBehaviour
    {
        public HandlingWeight weight = HandlingWeight.Fixed;
    }
}
