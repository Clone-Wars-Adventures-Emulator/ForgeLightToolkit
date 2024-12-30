Shader "Custom/DualTextureRigidGlow"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Diffuse2("Diffuse2", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Glow("Glow", Float) = 0.0
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
        sampler2D _Diffuse2;
        float4 _Tint;
        float _Glow;

        struct Input
        {
            float2 uv_Diffuse;
            float2 uv2_Diffuse2;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 texture0 = tex2D(_Diffuse, IN.uv_Diffuse);
            float4 texture1 = tex2D(_Diffuse2, IN.uv2_Diffuse2);
            float3 tint_color = texture0.rgb * _Tint;
            o.Albedo = lerp(tint_color, texture1.rgb, texture1.a);
            
            float3 glow_color = texture0.rgb * texture0.a;
            glow_color *= _Glow * 0.25;
            o.Emission = glow_color;
            o.Specular = 0.5;
            o.Gloss = 1;
        }
        ENDCG
    }
    Fallback "Diffuse"
}