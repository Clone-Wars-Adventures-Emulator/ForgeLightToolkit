Shader "Custom/TintMaskSkin"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Bias("Bias", Integer) = 0
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
        fixed4 _Tint;

        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseTexture = tex2D(_Diffuse, IN.uv_Diffuse);
            fixed3 tintedColor = (baseTexture.a * _Tint.xyz) - baseTexture.rgb;
            fixed3 baseColor = baseTexture.a + baseTexture.xyz;
            o.Albedo = baseTexture.rgb;
            o.Alpha = tintedColor;
        }

        ENDCG
    }
}