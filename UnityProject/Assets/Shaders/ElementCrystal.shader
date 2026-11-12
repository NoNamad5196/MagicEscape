Shader "MagicEscape/ElementCrystal"
{
    Properties
    {
        _BaseColor("Tint",Color)=(0.04,0.2,0.7,1)
        _EmissionColor("Element",Color)=(0.1,0.4,1,1)
        _Opacity("Opacity",Range(0,1))=0.25
        _Flow("Water caustics",Range(0,1))=1
    }
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
            struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;};
            struct V {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;float3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;};
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;half4 _EmissionColor;half _Opacity;half _Flow;
            CBUFFER_END
            V vert(A i){V o;o.positionWS=TransformObjectToWorld(i.positionOS.xyz);o.positionCS=TransformWorldToHClip(o.positionWS);o.normalWS=TransformObjectToWorldNormal(i.normalOS);o.uv=i.uv;return o;}
            half4 frag(V i):SV_Target
            {
                float fresnel=pow(1-saturate(dot(normalize(i.normalWS),GetWorldSpaceNormalizeViewDir(i.positionWS))),3);
                float wave=pow(saturate(sin(i.positionWS.y*95+sin(i.positionWS.x*45+_Time.y)*2-_Time.y*2)*.5+.5),12)*_Flow;
                return half4(_BaseColor.rgb+_EmissionColor.rgb*(fresnel*.8+wave*.35),saturate(_Opacity+fresnel*.5));
            }
            ENDHLSL
        }
    }
}
