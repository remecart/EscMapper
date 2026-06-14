Shader "Custom/TilemapShadowBake"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.01
        _ShadowOpacity ("Shadow Opacity", Range(0,1)) = 0.4

        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull    Off
        Lighting Off
        ZWrite  Off

        // Pass 1: mark shadow pixels as stencil=1, but ONLY where stencil==0.
        // stencil=0 → untouched (draw shadow here)
        // stencil=1 → already marked by a shadow tile (skip, prevents overlap darkening)
        // stencil=2 → blocked by TilemapShadowBlocker (skip, no shadow here)
        Pass
        {
            Name "SHADOW_STENCIL_WRITE"

            Stencil
            {
                Ref   1
                Comp  NotEqual  // skip if stencil!=0 (already marked=1, or blocked=2)
                Pass  Replace   // mark as 1
                Fail  Keep
                ZFail Keep
            }

            ColorMask 0
            ZWrite    Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _Color;
            fixed4    _RendererColor;
            float     _AlphaCutoff;

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex   = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color    = IN.color * _Color * _RendererColor;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                if (c.a < _AlphaCutoff) discard;
                return fixed4(0,0,0,1);
            }
            ENDCG
        }

        // Pass 2: draw shadow color only where stencil==1 (marked by Pass 1).
        // Resets stencil to 0 after drawing so overlapping tiles don't redraw.
        // Pixels with stencil==2 (blocker) are never reached because Pass 1
        // uses NotEqual — stencil=2 != 1 so Pass 1 skips them, they stay at 2,
        // and Pass 2's Comp Equal (ref=1) also skips them. Shadow invisible. 
        Pass
        {
            Name "SHADOW_COLOR"

            Stencil
            {
                Ref   1
                Comp  Equal
                Pass  Zero  // reset to 0 after drawing — no overlap redraw
                Fail  Keep
                ZFail Keep
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _Color;
            fixed4    _RendererColor;
            float     _AlphaCutoff;
            float     _ShadowOpacity;

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex   = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color    = IN.color * _Color * _RendererColor;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                if (c.a < _AlphaCutoff) discard;
                return fixed4(c.rgb, _ShadowOpacity);
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
