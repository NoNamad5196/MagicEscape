using UnityEngine;

namespace MagicEscape
{
    /// <summary>Redundant visual feedback only; RunePuzzleLogic remains the answer authority.</summary>
    public sealed class VisualClues : MonoBehaviour
    {
        public Light[] frameLights = new Light[4];
        public GameObject socketElements;
        public int LitFrames { get; private set; }
        public void ShowRuneProgress(int count)
        {
            LitFrames=count;
            for(int i=0;i<frameLights.Length;i++) if(frameLights[i]!=null) frameLights[i].enabled=i<count;
        }
        public void RevealElements() { if(socketElements!=null)socketElements.SetActive(true); }
    }
}
