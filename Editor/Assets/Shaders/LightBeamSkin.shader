Shader "FLTK/Built-In/LightBeamSkin"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Tint("Tint", Color) = (0, 0, 0, 0)
        _Density("Density", Float) = 0.0
        _Intensity("Intensity", Float) = 0.0
        _FallOff("FallOff", Float) = 0.0
        _ZRange("ZRange", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows alpha

        float4 _Tint;
        float _Intensity;

        sampler2D _Diffuse;

        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D(_Diffuse, IN.uv_Diffuse);
            o.Albedo = c.rgb * _Tint;
            o.Alpha = c.rgb * c.a * _Intensity;
        }

        ENDCG
    }
}
