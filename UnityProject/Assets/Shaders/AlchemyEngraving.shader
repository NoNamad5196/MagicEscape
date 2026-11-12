Shader "MagicEscape/AlchemyEngraving"
{
 Properties { _BaseMap("Incision",2D)="white"{} _BaseColor("Brass",Color)=(.65,.58,.40,1) _EmissionColor("Feedback",Color)=(0,0,0,1) _Fill("Incision fill",Range(0,1))=0 }
 SubShader { Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
 Pass { HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma shader_feature_local _EMISSION
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor,_EmissionColor; float _Fill;
 CBUFFER_END
 struct A {float4 p:POSITION;float2 uv:TEXCOORD0;};struct V {float4 p:SV_POSITION;float2 uv:TEXCOORD0;};
 V vert(A i){V o;o.p=TransformObjectToHClip(i.p.xyz);o.uv=i.uv;return o;}
 half4 frag(V i):SV_Target {half3 ink=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).rgb;float cut=smoothstep(.18,.45,ink.r);float fill=step(i.uv.y,_Fill)*step(.001,_Fill);return half4(lerp(half3(.027,.025,.022),_BaseColor.rgb*.45,cut)+cut*(_EmissionColor.rgb*.35+fill*half3(.7,.55,.27)),1);}
 ENDHLSL }
 }
}
