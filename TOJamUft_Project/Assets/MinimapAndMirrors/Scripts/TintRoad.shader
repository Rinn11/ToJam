Shader "Custom/TintWhite"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            // 0.5 alpha white color
            Color (0.8, 0.8, 0.8, 0.55)
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; };
            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); return o; }
            fixed4 frag(v2f i) : SV_Target { return fixed4(0.8, 0.8, 0.8, 0.55); } // Return white color with 0.5 alpha
            ENDCG
        }
    }
}
