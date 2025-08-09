Shader "Custom/RadarIndicator"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _IndicatorColor("Indiactor Color", Color) = (0,1,0,1)
        _IndicatorAngle("Indicator Angle", Float) = 0
        _IndicatorLength("Indicator Length", Float) = 4
        _IndicatorOuterLength("Indicator Outer Length", Float) = 7
        _IndicatorWedgeFadeOffset("Indicator Wedge Fade Offset", Float) = 1
        _IndicatorWedgePower("Indicator Wedge Power", Float) = 2
        _IndicatorFade("Indicator Width", Float) = 2
        _IndicatorIntensity("Indicator Intensity", Float) = 3.44
        _IndicatorActive("Inidactor Active?", Float) = 0
        _PixelWidth("Pixel Width", Integer) = 0
        _PixelHeight("Pixel Height", Integer) = 0
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
            TEXTURE2D(_MainTex);
            // This macro declares the sampler for the _MainTex texture.
            SAMPLER(sampler_MainTex);

            // To make the Unity shader SRP Batcher compatible, declare all
            // properties related to a Material in a a single CBUFFER block with
            // the name UnityPerMaterial.
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float4 _IndicatorColor;
                float _IndicatorAngle;
                float _IndicatorLength;
                float _IndicatorOuterLength;
                float _IndicatorWedgeFadeOffset;
                float _IndicatorWedgePower;
                float _IndicatorFade;
                float _IndicatorIntensity;
                float _IndicatorActive;
                int _PixelWidth;
                int _PixelHeight;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float2 pixelize_uv(float2 uv, float2 resolution) {
                return floor(uv * resolution) / resolution;
            }

            float greater_than_angle(float x, float y) {
                return max(sign(x - y), 0.0);
            }

            float greater_than_angle_smooth(float x, float y) {
                return x - y;
            }

            float angular_distance(float a, float b) {
                float diff = abs(a - b);
                return min(diff, 1.0 - diff); // wrap-around safety
            }

            float two_sided_fade(float uv_angle, float center_angle, float fade_width) {
                float d = angular_distance(uv_angle, center_angle);
                return saturate(1.0 - d / fade_width);
            }

            float wedge_fade(float uv_angle, float center_angle, float half_width, float fade_width) {
                float d = angular_distance(uv_angle, center_angle);

                // Outside wedge completely
                if (d > half_width) return 0.0;

                // Inside wedge core
                if (d < half_width - fade_width) return 1.0;

                // Fade zone
                return saturate((half_width - d) / fade_width);
            }

            float wedge_fade_variable_width(
                float uv_angle,
                float center_angle,
                float radius,
                float inner_width,     // width at center
                float outer_width,     // width at outer edge
                float width_offset,
                float fade_width,
                float width_pow
            ) {
                // Scale width by radius
                float scaled_width = lerp(inner_width, outer_width, saturate(radius - width_offset));
                scaled_width = pow(scaled_width, max(width_pow, 1.0));

                // Half width for math
                float half_width = scaled_width * 0.5;

                float d = angular_distance(uv_angle, center_angle);

                // Outside wedge completely
                if (d > half_width) return 0.0;

                // Inside wedge core
                if (d < half_width - fade_width) return 1.0;

                // Fade zone
                return saturate((half_width - d) / fade_width);
            }

            float4 frag(Varyings IN) : SV_Target
            {

                // Pixel resolution you want for the retro effect
                float2 pixelResolution = float2(_PixelWidth, _PixelHeight);

                // Snap UVs to pixel grid
                float2 pixelUV = floor(IN.uv * pixelResolution) / pixelResolution;

                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelUV);

                float pi = 3.141592656;

                float2 uv = pixelUV * 2.0 - 1.0;
                float len = length(uv);
                uv.y = -uv.y;
                float uv_angle = (atan2(uv.y, uv.x) + pi) / (pi * 2);

                float current_indicator_angle = _IndicatorAngle % 1.0;

                uv_angle = uv_angle - 1.0 * greater_than_angle(uv_angle, current_indicator_angle);

                /*float3 color = SAMPLE_TEXTURE2D(_RenderTex, sampler_RenderTex, IN.uv);

                color = lerp(color.rgb, _LineColor.rgb, _LineColor.a * greater_than_angle(uv_angle, current_line_angle - line_width));
                color = lerp(color.rgb, _TrailColor.rgb, _TrailColor.a * max(0.0, (1.0 - (current_line_angle - uv_angle) / trail_width)));*/

                //float4 line_color = float4(_LineColor.rgb, _LineColor.a * greater_than_angle(uv_angle, current_line_angle - line_width));
                //float4 trail_color = float4(_TrailColor.rgb, _TrailColor.a * max(0.0, (1.0 - (current_line_angle - uv_angle) / trail_width)));

                //float fade = two_sided_fade(
                //    uv_angle,
                //    current_indicator_angle,
                //    _IndicatorLength / 10 // or whatever fade size you want
                //);

                //float fade = wedge_fade(
                //    uv_angle,
                //    current_indicator_angle,
                //    _IndicatorLength * 0.5,    // half of total wedge width
                //    _IndicatorFade             // fade zone size
                //);

                float fade = wedge_fade_variable_width(
                    uv_angle,
                    current_indicator_angle,
                    len,                  // radius factor
                    _IndicatorLength * 0.1,   // width at center
                    _IndicatorOuterLength * 0.1, // width at outer edge
                    _IndicatorWedgeFadeOffset,
                    _IndicatorFade,
                    _IndicatorWedgePower
                );

                color *= float4(_IndicatorColor.rgb, _IndicatorColor.a * fade * _IndicatorIntensity);

                color.a *= _IndicatorActive;

                //return float4(color.rgb, 1);
                return color;
            }
            ENDHLSL
        }
    }
}
