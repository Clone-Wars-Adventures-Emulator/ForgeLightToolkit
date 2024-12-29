Shader "Custom/LightBeam"
{
    Properties
    {
        _GradientTexture("GradientTexture", 2D) = "white" {}
        _TintSemantic("TintSemantic", Color) = (0, 0, 0, 0)
        _Density("Density", Float) = 0.0
        _Intensity("Intensity", Float) = 0.0
        _FallOff("FallOff", Float) = 0.0
        _ZRange("ZRange", Float) = 0.0
        _Fade("Fade", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf StandardSpecular fullforwardshadows alpha:fade

        float4 _TintSemantic;
        float _Intensity;

        sampler2D _GradientTexture;

        struct Input
        {
            float2 uv_GradientTexture;
        };

        void surf(Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float4 c = tex2D(_GradientTexture, IN.uv_GradientTexture);
            o.Albedo = c.a * _TintSemantic * 1;
            o.Alpha = (c.a * _Intensity) * _TintSemantic.a;
        }

        ENDCG
    }
}