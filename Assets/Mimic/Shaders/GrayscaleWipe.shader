Shader "Custom/GrayscaleWipe"
{
    Properties
    {
        _Progress ("Progress", Range(0, 1)) = 0
        _Softness ("Softness", Range(0, 0.2)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "GrayscaleWipe"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Progress;
            float _Softness;

            half4 Frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                half3 grayscale = half3(gray, gray, gray);

                float mask = 1.0 - smoothstep(_Progress, _Progress + _Softness, input.texcoord.x);

                col.rgb = lerp(col.rgb, grayscale, mask);

                return col;
            }
            ENDHLSL
        }
    }
}