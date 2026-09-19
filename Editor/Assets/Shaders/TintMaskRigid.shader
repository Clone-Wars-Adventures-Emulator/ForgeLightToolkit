Shader "FLTK/Built-In/TintMaskRigid"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _DoubleSided("DoubleSided", Integer) = 0
        _FadeStencil("FadeStencil", Integer) = 0
        _Tint("Tint", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows addshadow alphatest:_Cutoff

        sampler2D _Diffuse;
        sampler2D _TintMask;
        float4 _Tint;

        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D(_Diffuse, IN.uv_Diffuse) * _Tint;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
    }
}
