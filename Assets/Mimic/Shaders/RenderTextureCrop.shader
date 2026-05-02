Shader "Custom/RenderTextureCropSheet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Edge ("Edge", Range(-2, 2)) = -1
        _Direction ("Direction", Vector) = (1, 0, 0, 0)

        _Softness ("Softness", Range(0, 0.3)) = 0.03
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

            float _Edge;
            float4 _Direction;
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

                float2 dir = _Direction.xy;

                if (length(dir) < 0.001)
                    dir = float2(1, 0);

                dir = normalize(dir);

                float2 perp = float2(-dir.y, dir.x);
                float2 centeredUV = i.uv - 0.5;

                float coord = dot(centeredUV, dir) - dot(centeredUV, perp) * _Tilt;

                float alpha;

                if (_Softness <= 0.0001)
                {
                    alpha = coord <= _Edge ? 1.0 : 0.0;
                }
                else
                {
                    alpha = 1.0 - smoothstep(
                        _Edge - _Softness,
                        _Edge + _Softness,
                        coord
                    );
                }

                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
