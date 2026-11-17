Shader "MagicEscape/WarehouseChalk"
{
 Properties { [MainTexture] _BaseMap("Sparse chalk",2D)="black"{} _EmissionMap("Same approved drawing",2D)="black"{} _BaseColor("Chalk",Color)=(.8,.84,.79,1) }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout"}
 Pass { Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor;
 CBUFFER_END
 struct A{float4 p:POSITION;float2 uv:TEXCOORD0;};struct V{float4 p:SV_POSITION;float2 uv:TEXCOORD0;};
 V vert(A i){V o;o.p=TransformObjectToHClip(i.p.xyz);o.uv=i.uv;return o;}
 half4 frag(V i):SV_Target{half ink=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).r;clip(ink-.10);return half4(_BaseColor.rgb*lerp(.42,.85,sqrt(ink)),1);}
 ENDHLSL }
 }
}
