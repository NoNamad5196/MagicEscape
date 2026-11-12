Shader "MagicEscape/ElementSmoke"
{
    Properties { _BaseColor("Smoke",Color)=(0.09,0.015,0.14,1) _EmissionColor("Violet",Color)=(0.15,0.02,0.25,1) }
    SubShader
    {
        Tags {"RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct A {float4 p:POSITION;float3 n:NORMAL;};
            struct V {float4 p:SV_POSITION;float3 w:TEXCOORD0;float3 n:TEXCOORD1;};
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;half4 _EmissionColor;
            CBUFFER_END
            V vert(A i){V o;o.w=TransformObjectToWorld(i.p.xyz);o.p=TransformWorldToHClip(o.w);o.n=TransformObjectToWorldNormal(i.n);return o;}
            half4 frag(V i):SV_Target
            {
                float3 q=i.w*33+float3(_Time.y*.7,-_Time.y*1.3,_Time.y*.4);
                float noise=sin(q.x+sin(q.y))*sin(q.y+sin(q.z))*sin(q.z+sin(q.x));
                float edge=saturate(dot(normalize(i.n),GetWorldSpaceNormalizeViewDir(i.w)));
                float density=saturate(.45+noise*.55)*smoothstep(0,.5,edge);
                return half4(_BaseColor.rgb+_EmissionColor.rgb*(.2+noise*.35),density*.78);
            }
            ENDHLSL
        }
    }
}
