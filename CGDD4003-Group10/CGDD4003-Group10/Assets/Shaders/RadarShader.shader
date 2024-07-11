Shader "Custom/RadarShader"
{
    Properties
    {
        _RenderTex ("Render Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _LineAngle("Radar Line Angle", Float) = 0
        _LineWidth(" Radar Line Width", Float) = 1
        _LineColor("Radar Line Color", Color) = (0,1,0,1)
        _TrailColor("Radar Trail Color", Color) = (0,1,0,1)
        _TrailWidth(" Radar Trail Width", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

         Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            // This macro declares _MainTex as a Texture2D object.
            TEXTURE2D(_RenderTex);
            // This macro declares the sampler for the _MainTex texture.
            SAMPLER(sampler_RenderTex);

            // To make the Unity shader SRP Batcher compatible, declare all
            // properties related to a Material in a a single CBUFFER block with
            // the name UnityPerMaterial.
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _RenderTex_ST;
                float _LineAngle;
                float _LineWidth;
                float4 _LineColor;
                float4 _TrailColor;
                float _TrailWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _RenderTex);
                return OUT;
            }

            float greater_than_angle(float x, float y) {
                return max(sign(x - y), 0.0);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float pi = 3.141592656;

                float2 uv = IN.uv * 2.0 - 1.0;
                float len = length(uv);
                uv.y = -uv.y;
                float uv_angle = (atan2(uv.y, uv.x) + pi) / (pi * 2);

                float current_line_angle = _LineAngle % 1.0;

                uv_angle = uv_angle - 1.0 * greater_than_angle(uv_angle, current_line_angle);

                float line_width = _LineWidth / 100;
                float trail_width = _TrailWidth / 10;

                /*float3 color = SAMPLE_TEXTURE2D(_RenderTex, sampler_RenderTex, IN.uv);

                color = lerp(color.rgb, _LineColor.rgb, _LineColor.a * greater_than_angle(uv_angle, current_line_angle - line_width));
                color = lerp(color.rgb, _TrailColor.rgb, _TrailColor.a * max(0.0, (1.0 - (current_line_angle - uv_angle) / trail_width)));*/

                float4 line_color = float4(_LineColor.rgb, _LineColor.a * greater_than_angle(uv_angle, current_line_angle - line_width));
                float4 trail_color = float4(_TrailColor.rgb, _TrailColor.a * max(0.0, (1.0 - (current_line_angle - uv_angle) / trail_width)));

                float4 color = line_color + trail_color;

                //return float4(color.rgb, 1);
                return color;
            }
            ENDHLSL
        }
    }
}
