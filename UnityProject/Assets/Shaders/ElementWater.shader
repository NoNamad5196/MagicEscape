Shader "MagicEscape/ElementWater"
{
 Properties { _BaseColor("Deep liquid",Color)=(.015,.065,.13,1) _EmissionColor("Reflection",Color)=(.15,.3,.42,1) _NormalMap("Original wave normals",2D)="bump"{} _Flow("Flow",Float)=1 }
 SubShader {Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
 Pass {
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma shader_feature_local _EMISSION
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 TEXTURE2D(_NormalMap);SAMPLER(sampler_NormalMap);
 CBUFFER_START(UnityPerMaterial)
 half4 _BaseColor,_EmissionColor;float _Flow;
 CBUFFER_END
 struct A{float4 p:POSITION;float3 n:NORMAL;float4 t:TANGENT;float2 uv:TEXCOORD0;};
 struct V{float4 p:SV_POSITION;float3 w:TEXCOORD1;float3 n:TEXCOORD2;float3 t:TEXCOORD3;float3 b:TEXCOORD4;float2 uv:TEXCOORD0;};
 V vert(A i){V o;float3 p=i.p.xyz;p.y+=sin(p.x*23+_Time.y*2)*cos(p.z*19-_Time.y)*.006;o.w=TransformObjectToWorld(p);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(i.n);o.t=TransformObjectToWorldDir(i.t.xyz);o.b=cross(o.n,o.t)*i.t.w;o.uv=i.uv;return o;}
 half4 frag(V i):SV_Target
 {
 float2 scroll=float2(_Time.y*.037,_Time.y*-.052)*_Flow;
 float3 a=UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,i.uv*3+scroll));
 float3 b=UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,i.uv*4-scroll.yx));
 float3 n=normalize(i.n+(i.t*(a.x+b.x)+i.b*(a.y+b.y))*.5);
 float3 v=GetWorldSpaceNormalizeViewDir(i.w);float fres=pow(1-saturate(dot(n,v)),4);
 Light l=GetMainLight();float spec=pow(saturate(dot(n,normalize(v+l.direction))),96);
 float depth=saturate(length(i.uv-.5)*1.5);half3 body=_BaseColor.rgb*lerp(.6,1.3,depth);
 half3 env=GlossyEnvironmentReflection(reflect(-v,n),.12,1);
 return half4(body+lerp(_EmissionColor.rgb*.05,env*.4+_EmissionColor.rgb*.22,fres)+spec*l.color*.5,1);
 }
 ENDHLSL }
 }
}
