Shader "Custom/TintMaskRigid"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _DoubleSided("DoubleSided", Integer) = 0
        _FadeStencil("FadeStencil", Integer) = 0
        _TintSemantic("TintSemantic", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Front

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows addshadow alphatest:_Cutoff

        sampler2D _Diffuse;
        sampler2D _TintMask;
        float4 _TintSemantic;

        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D(_Diffuse, IN.uv_Diffuse) * _TintSemantic;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
    }
}