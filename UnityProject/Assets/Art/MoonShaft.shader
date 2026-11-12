Shader "MagicEscape/MoonShaft"
{
    Properties { _BaseMap("Soft shaft",2D)="white"{} _Color("Colour",Color)=(.12,.2,.3,.12) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            CBUFFER_END
            struct A { float4 p:POSITION; float2 uv:TEXCOORD0; };
            struct V { float4 p:SV_POSITION; float2 uv:TEXCOORD0; };
            V vert(A a) { V o; o.p=TransformObjectToHClip(a.p.xyz);o.uv=a.uv;return o; }
            half4 frag(V i):SV_Target { float edge=pow(saturate(1-abs(i.uv.x*2-1)),2);float fade=sin(saturate(i.uv.y)*PI);return half4(_Color.rgb,_Color.a*edge*fade); }
            ENDHLSL
        }
    }
}
