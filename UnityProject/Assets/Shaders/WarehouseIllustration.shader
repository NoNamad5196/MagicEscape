Shader "MagicEscape/WarehouseIllustration"
{
 Properties{[MainTexture] _BaseMap("Manuscript pigments",2D)="white"{} _BaseColor("Paper",Color)=(1,1,1,1)}
 SubShader{Tags{"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
 Pass{Tags{"LightMode"="UniversalForward"} Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor;
 CBUFFER_END
 struct A{float4 p:POSITION;float3 n:NORMAL;float2 uv:TEXCOORD0;};struct V{float4 p:SV_POSITION;float3 w:TEXCOORD0;float3 n:TEXCOORD1;float2 uv:TEXCOORD2;};
 V vert(A i){V o;o.w=TransformObjectToWorld(i.p.xyz);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(i.n);o.uv=i.uv;return o;}
 half4 frag(V i):SV_Target
 {
  // Preserve illumination intensity/darkening, but neutralize the lantern's colour cast on clue pigments.
  // No fixed emission, and no second lighting writer in runtime.
  float3 n=normalize(i.n);Light main=GetMainLight();
  half3 light=SampleSH(n)+main.color*abs(dot(n,main.direction))*main.distanceAttenuation;
  #ifdef _ADDITIONAL_LIGHTS
  uint count=GetAdditionalLightsCount();
  for(uint k=0;k<count;k++){Light l=GetAdditionalLight(k,i.w);light+=l.color*abs(dot(n,l.direction))*l.distanceAttenuation;}
  #endif
  float luminance=dot(light,float3(.2126,.7152,.0722));
  half3 paper=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).rgb*_BaseColor.rgb;
  return half4(paper*clamp(luminance,.045,.90),1);
 }
 ENDHLSL}
 }
}
