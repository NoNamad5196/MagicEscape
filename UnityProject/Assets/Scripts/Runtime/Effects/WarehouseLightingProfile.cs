using UnityEngine;
namespace MagicEscape
{
    [CreateAssetMenu(menuName="MagicEscape/Warehouse lighting")]
    public sealed class WarehouseLightingProfile : ScriptableObject
    {
        [System.Serializable] public class Stage
        {
            public Color ambient;
            public float reflection, fill, background, candle, flicker;
            public Stage(float a,float r,float f,float b,float c,float w)
            {ambient=new Color(a*.70f,a*.80f,a);reflection=r;fill=f;background=b;candle=c;flicker=w;}
        }
        public float transitionSeconds=1.6f;
        public float solvedBrewGlow=.06f;
        public float localIntensity=.65f, localRange=2.1f;
        public Color localColor=new Color(.35f,.55f,.7f);
        public Stage[] stages={
            new Stage(.15f,.65f,.22f,1,1,.025f),
            new Stage(.105f,.40f,.15f,.62f,.78f,.045f),
            new Stage(.065f,.22f,.085f,.30f,.53f,.065f),
            new Stage(.035f,.10f,.035f,.10f,.28f,.085f)};
    }
}
