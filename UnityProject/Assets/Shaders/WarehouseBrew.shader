Shader "MagicEscape/WarehouseBrew"
{
 Properties { _BaseColor("Deep tincture",Color)=(.018,.046,.04,1) _EmissionColor("Brew feedback",Color)=(0,0,0,1) }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
 Pass {
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor,_EmissionColor;
 CBUFFER_END
 struct A{float4 p:POSITION;float3 n:NORMAL;float2 uv:TEXCOORD0;};
 struct V{float4 p:SV_POSITION;float3 w:TEXCOORD0;float3 n:TEXCOORD1;};
 V vert(A i){V o;o.w=TransformObjectToWorld(i.p.xyz);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(i.n);return o;}
 half4 frag(V i):SV_Target
 {
  // Analytic long waves, never a high-frequency normal-map weave or random noise.
  float2 q=i.w.xz;float t=_Time.y*.32;
  float a=dot(q,float2(4.3,2.1))+t,b=dot(q,float2(-2.5,3.6))-t*.7;
  float3 n=normalize(i.n+float3(cos(a)*.018,0,cos(b)*.014));
  float3 v=GetWorldSpaceNormalizeViewDir(i.w);float f=pow(1-saturate(dot(n,v)),4);
  Light l=GetMainLight();float spec=pow(saturate(dot(n,normalize(v+l.direction))),120);
  half3 env=GlossyEnvironmentReflection(reflect(-v,n),.25,1);
  half3 body=_BaseColor.rgb*(.68+.10*sin(a)*sin(b));
  float ring=pow(saturate(.5+.5*sin(length(q-float2(2.65,.8))*23-t)),8);
  half3 ripple=half3(.012,.024,.022)*ring*(.35+.65*f);
  return half4(body+ripple+half3(.002,.009,.008)+env*f*.13+spec*l.color*.12+_EmissionColor.rgb*.18,1);
 }
 ENDHLSL }
 }
}
