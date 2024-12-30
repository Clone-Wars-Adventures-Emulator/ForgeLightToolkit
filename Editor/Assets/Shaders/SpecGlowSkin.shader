Shader "Custom/SpecGlowSkin"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Bias("Bias", Integer) = 0.0
        _Glow("Glow", Float) = 0.0
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _FadeStencil("FadeStencil", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf BlinnPhong fullforwardshadows addshadow

        sampler2D _Diffuse;
        float4 _Tint;
        float _Glow;
        
        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 texture0 = tex2D(_Diffuse, IN.uv_Diffuse);
            float3 tint_color = texture0.rgb * _Tint;
            o.Albedo = lerp(tint_color, texture0.rgb, texture0.a);
            
            float3 glow_color = texture0.rgb;
            glow_color *= _Glow * 0.25;
            o.Emission = glow_color; //texture0.rgb * tex2D(_Diffuse, scroll);
            o.Specular = 1;
            o.Gloss = 1;
        }

        ENDCG
    }
}