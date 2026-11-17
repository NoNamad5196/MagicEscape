Shader "MagicEscape/WarehouseIncision"
{
 Properties { [MainTexture] _BaseMap("Carved motif",2D)="black"{} _BaseColor("Pale mineral inlay",Color)=(.66,.68,.60,1) _EmissionColor("Feedback",Color)=(0,0,0,1) _Fill("Incision fill",Range(0,1))=0 }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout"}
 Pass { Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor,_EmissionColor;float _Fill;
 CBUFFER_END
 struct A{float4 p:POSITION;float2 uv:TEXCOORD0;};struct V{float4 p:SV_POSITION;float2 uv:TEXCOORD0;};
 V vert(A i){V o;o.p=TransformObjectToHClip(i.p.xyz);o.uv=i.uv;return o;}
 half4 frag(V i):SV_Target
 {
  float cut=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).r;clip(cut-.17);
  float edge=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv+float2(.003,-.005)).r;
  float grain=frac(sin(dot(floor(i.uv*430),float2(127.1,311.7)))*43758.5453);
  float fill=step(i.uv.y,_Fill)*step(.001,_Fill);
  return half4(_BaseColor.rgb*lerp(.28,.69,edge)*lerp(.88,1.06,grain)+_EmissionColor.rgb*.35+fill*half3(.5,.44,.28),1);
 }
 ENDHLSL }
 }
}
