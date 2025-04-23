Shader "Custom/FadeEdgeSprite" {
    Properties {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _FadeStrength ("Fade Strength", Range(0, 1)) = 1
        _FadePower ("Fade Power", Range(1, 10)) = 3
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        LOD 100

        Pass {
            Name "Unlit"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeStrength;
            float _FadePower;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;

                // Sample texture
                half4 col = tex2D(_MainTex, uv) * _Color;

                // Compute edge proximity
                float2 distToEdge = min(uv, 1.0 - uv); // distance to closest edge in X and Y
                float edgeFactor = min(distToEdge.x, distToEdge.y); // closest edge overall

                // Calculate fade amount
                float fade = pow(saturate(edgeFactor * 2), _FadePower); // 0 at edge, 1 at center
                float finalAlpha = col.a * lerp(1.0, fade, _FadeStrength);

                col.a = finalAlpha;
                return col;
            }
            ENDHLSL
        }
    }
}