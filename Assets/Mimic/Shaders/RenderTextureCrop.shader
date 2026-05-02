Shader "Custom/RenderTextureCropSheet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Progress ("Progress", Range(-0.5, 1.5)) = 0
        _Direction ("Direction", Float) = 0

        _Softness ("Softness", Range(0, 0.2)) = 0.03
        _Tilt ("Tilt", Range(-1, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float _Progress;
            float _Direction;
            float _Softness;
            float _Tilt;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float centeredY = i.uv.y - 0.5;
                float edge = _Progress + centeredY * _Tilt;

                float alpha;

                if (_Direction < 0.5)
                {
                    alpha = 1.0 - smoothstep(
                        edge - _Softness,
                        edge + _Softness,
                        i.uv.x
                    );
                }
                else
                {
                    alpha = smoothstep(
                        edge - _Softness,
                        edge + _Softness,
                        i.uv.x
                    );
                }

                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
