Shader "MagicEscape/NearRim"
{
    Properties { _Color("Rim",Color)=(.2,.32,.35,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent+1" }
        Pass
        {
            Blend One One
            ZWrite Off
            Offset -1,-1
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            CBUFFER_END
            struct A { float4 p:POSITION; float3 n:NORMAL; };
            struct V { float4 p:SV_POSITION; float3 w:TEXCOORD0; float3 n:TEXCOORD1; };
            V vert(A a) { V o;o.w=TransformObjectToWorld(a.p.xyz);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(a.n);return o; }
            half4 frag(V i):SV_Target {float3 v=GetWorldSpaceViewDir(i.w);float near=saturate((2.5-length(v))/.8);float rim=pow(1-saturate(dot(normalize(v),normalize(i.n))),4);return half4(_Color.rgb*rim*near*.25,1);}
            ENDHLSL
        }
    }
}
