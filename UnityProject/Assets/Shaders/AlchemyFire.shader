Shader "MagicEscape/AlchemyFire"
{
 Properties { _BaseMap("CC0 Flame02 16x4",2D)="black"{} _BaseColor("Tint",Color)=(1,1,1,1) _EmissionColor("Heat",Color)=(1,1,1,1) }
 SubShader {Tags {"RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent"}
 Pass {Blend One OneMinusSrcAlpha ZWrite Off Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor,_EmissionColor;
 CBUFFER_END
 struct A{float4 p:POSITION;float2 uv:TEXCOORD0;half4 c:COLOR;};struct V{float4 p:SV_POSITION;float2 uv:TEXCOORD0;half4 c:COLOR;};
 V vert(A i){V o;o.p=TransformObjectToHClip(i.p.xyz);o.uv=i.uv;o.c=i.c;return o;}
 half4 frag(V i):SV_Target
 {
 float f=fmod(floor(_Time.y*30),64);float2 cell=float2(fmod(f,16),3-floor(f/16));
 float2 uv=(clamp(i.uv,.005,.995)+cell)/float2(16,4);
 half4 t=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,uv);
 half a=saturate(max(t.r,max(t.g,t.b))*1.5);
 return half4(t.rgb*_BaseColor.rgb*1.8,a);
 }
 ENDHLSL }
 }
}
