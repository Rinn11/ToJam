Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Base Blur Size", Float) = 1.0
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            // Global variable set from C#:
            int GlobalAlcoholCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float blurRadius = _BlurSize * (GlobalAlcoholCount <= 10 
                        ? 0.5 * GlobalAlcoholCount // Linear increase until 10
                        : 5.0 + log(1.0 + (GlobalAlcoholCount - 10))); // Logarithmic growth after 10
                if (blurRadius < 1.0)
                {
                    // No blur: return original color immediately
                    return tex2D(_MainTex, uv);
                }

                float2 offset = blurRadius * _MainTex_TexelSize.xy;

                fixed4 col = tex2D(_MainTex, uv) * 0.36;
                col += tex2D(_MainTex, uv + offset) * 0.16;
                col += tex2D(_MainTex, uv - offset) * 0.16;
                col += tex2D(_MainTex, uv + offset.yx) * 0.16;
                col += tex2D(_MainTex, uv - offset.yx) * 0.16;

                return col;
            }

            ENDCG
        }
    }
}
