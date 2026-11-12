Shader "MagicEscape/OldCrystal" {
Properties { _BaseColor("Body",Color)=(.03,.15,.5,1) _EmissionColor("Inner light",Color)=(.01,.06,.2,1) _BaseMap("Mineral",2D)="white"{} }
SubShader { Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"} Pass {
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
struct A {float4 p:POSITION; float3 n:NORMAL; float2 uv:TEXCOORD0;};
struct V {float4 p:SV_POSITION; float3 n:TEXCOORD0; float3 w:TEXCOORD1; float2 uv:TEXCOORD2;};
CBUFFER_START(UnityPerMaterial)
float4 _BaseColor;float4 _EmissionColor;float4 _BaseMap_ST;
CBUFFER_END
TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
V vert(A a){V o;o.w=TransformObjectToWorld(a.p.xyz);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(a.n);o.uv=a.uv;return o;}
half4 frag(V i):SV_Target{float3 n=normalize(i.n),v=normalize(GetWorldSpaceViewDir(i.w));float rim=pow(1-saturate(abs(dot(n,v))),3);float vein=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).r;Light l=GetMainLight();float lit=.25+.4*saturate(dot(n,l.direction));return half4(_BaseColor.rgb*(lit+vein*.2)+_EmissionColor.rgb*(.65+rim*2)+rim*float3(.15,.19,.22),1);}
ENDHLSL
} }
}
