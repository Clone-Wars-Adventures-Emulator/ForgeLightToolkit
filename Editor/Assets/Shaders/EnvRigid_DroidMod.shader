Shader "Custom/EnvRigid_DroidTest"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Cube ("Reflection Map", CUBE) = "" {}
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Reflection("Reflection", Float) = 0.0
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
        samplerCUBE _Cube;
        float4 _Tint;
        float _Reflection;
        float _Glow;

        struct Input
        {
            float2 uv_Diffuse;
            float3 worldRefl;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 texture0 = tex2D(_Diffuse, IN.uv_Diffuse);
            float3 texturecube0 = texCUBE (_Cube, IN.worldRefl);
            float3 tint_color = texture0.rgb * _Tint;
            float3 reflection_intensity = (texturecube0.rgb * texture0.a) * _Reflection;
            o.Albedo = tint_color + reflection_intensity;
            o.Specular = texture0.a * 0.5;
            o.Gloss = texture0.a * 0.5;
                        float3 glow_color = texture0.rgb * texture0.a;
            glow_color *= _Glow * 0.25;
            o.Emission = glow_color;
        }
        ENDCG
    }
    Fallback "Diffuse"
}