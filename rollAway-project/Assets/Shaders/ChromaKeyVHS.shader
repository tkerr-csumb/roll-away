Shader "Custom/ChromaKeyVHS"
{
    Properties
    {
        _MainTex        ("Video Texture",       2D)            = "white" {}
        _ChromaColor    ("Chroma Key Color",    Color)         = (0, 1, 0, 1)
        _Threshold      ("Threshold",           Range(0, 1))   = 0.4
        _Softness       ("Softness",            Range(0, 1))   = 0.1
        _Jitter         ("Horizontal Jitter",   Range(0, 0.05))= 0.008
        _TrackingStrength("Tracking Noise",     Range(0, 0.1)) = 0.03
        _Aberration     ("Chromatic Aberration",Range(0, 0.02))= 0.005
        _ScanlineStr    ("Scanline Strength",   Range(0, 0.2)) = 0.05
        _NoiseStr       ("Luminance Noise",     Range(0, 0.3)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "RenderType"      = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "ChromaKeyVHS"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ChromaColor;
                float  _Threshold;
                float  _Softness;
                float  _Jitter;
                float  _TrackingStrength;
                float  _Aberration;
                float  _ScanlineStr;
                float  _NoiseStr;
            CBUFFER_END

            float rand(float2 seed)
            {
                return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float  t  = _Time.y;

                float jitter = (rand(float2(floor(uv.y * 240.0), t * 30.0)) - 0.5) * _Jitter;
                uv.x += jitter;

                float bandPos   = frac(t * 0.15);
                float bandWidth = 0.05;
                float inBand    = smoothstep(bandWidth, 0.0, abs(uv.y - bandPos));
                uv.x += rand(float2(uv.y, t * 0.7)) * _TrackingStrength * inBand;

                uv = saturate(uv);

                float2 rUV = saturate(uv + float2( _Aberration, 0.0));
                float2 bUV = saturate(uv + float2(-_Aberration, 0.0));

                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, rUV).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv ).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, bUV).b;
                half4 col = half4(r, g, b, 1.0);

                float  dist  = length(col.rgb - _ChromaColor.rgb);
                float  alpha = smoothstep(_Threshold - _Softness,
                                          _Threshold + _Softness, dist);

                float scanline = sin(IN.uv.y * 600.0) * _ScanlineStr;
                col.rgb = saturate(col.rgb - scanline);

                float noise = (rand(uv + float2(t * 0.1, t * 0.07)) - 0.5) * _NoiseStr;
                col.rgb = saturate(col.rgb + noise);

                col.a = alpha;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
