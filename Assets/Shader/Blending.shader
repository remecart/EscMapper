Shader "Unlit/Blending/GlowMultiplier_Fixed"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MinGlow ("Min Glow", Range(0, 1)) = 0.2
        _MaxGlow ("Max Glow", Range(0, 1)) = 0.5
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        GrabPass { "_BackgroundTex" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BackgroundTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _MinGlow;
            float _MaxGlow;
            float _AlphaCutoff;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

fixed4 frag (v2f i) : SV_Target
{
    fixed4 texColor = tex2D(_MainTex, i.texcoord);

    // Remove fully transparent pixels
    if (texColor.a < _AlphaCutoff)
        discard;

    // Sample background
    fixed4 bgColor = tex2Dproj(_BackgroundTex, UNITY_PROJ_COORD(i.screenPos));

    // Background brightness → glow multiplier
    float bgBrightness = dot(bgColor.rgb, float3(0.299, 0.587, 0.114));
    float glow = lerp(_MinGlow, _MaxGlow, bgBrightness);

    // Fade edges smoothly
    float fade = smoothstep(_AlphaCutoff, 1.0, texColor.a);

    // Apply glow multiplier & fade to sprite color
    fixed3 glowColor = texColor.rgb * _Color.rgb * glow * fade;

    // Additively blend over the background
    fixed3 finalColor = bgColor.rgb + glowColor;

    return fixed4(finalColor, 1.0); // alpha 1.0 so it writes the brightened color
}


            ENDCG
        }
    }

    Fallback "Diffuse"
}