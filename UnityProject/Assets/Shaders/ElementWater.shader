Shader "MagicEscape/ElementWater"
{
    Properties { _BaseColor("Water",Color)=(0.03,0.3,0.65,1) _EmissionColor("Reflection",Color)=(0.2,0.7,1,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor; half4 _EmissionColor;
            CBUFFER_END
            V vert(A i) { V o; o.positionCS=TransformObjectToHClip(i.positionOS.xyz); o.uv=i.uv; return o; }
            half4 frag(V i):SV_Target
            {
                float r=length(i.uv-.5);
                float wave=pow(saturate(sin(r*95-_Time.y*3)*.5+.5),10);
                float caustic=pow(saturate(sin(i.uv.x*31+sin(i.uv.y*22+_Time.y)*2+_Time.y)*.5+.5),14);
                return half4(_BaseColor.rgb+_EmissionColor.rgb*(wave*.6+caustic*.3),1);
            }
            ENDHLSL
        }
    }
}
