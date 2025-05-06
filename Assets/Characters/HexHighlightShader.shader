Shader "Custom/HexHighlightShader"
{
    Properties
    {
        _Color ("Highlight Color", Color) = (1,1,1,0.5) // Default semi-transparent white
        _BreathSpeed ("Breathing Speed", Range(0.1, 3)) = 1 // How fast it breathes
    }
    
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enables transparency
        ZWrite Off // Prevents depth writing so it overlays properly

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _BreathSpeed;
            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = 0.5 + 0.3 * sin(_Time.y * _BreathSpeed); // Breathing effect
                return fixed4(_Color.rgb, alpha); // Set color with animated transparency
            }
            ENDCG
        }
    }
}
