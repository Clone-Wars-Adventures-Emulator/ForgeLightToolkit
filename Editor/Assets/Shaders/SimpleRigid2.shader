Shader "Custom/SimpleRigid2"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _ScrollV("ScrollV", Float) = 0.0
        _ScrollU("ScrollU", Float) = 0.0
        _Glow("Glow", Float) = 0.0
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _FadeStencil("FadeStencil", Integer) = 0
        _DoubleSidedDefaultFalse("DoubleSidedDefaultFalse", Integer) = 0
        _Specular ("Specular", Float) = 0.5
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
        float _ScrollU;
        float _ScrollV;
        float4 _Tint;
        float _Glow;
        float _Specular;
        
        struct Input
        {
            float2 uv_Diffuse: TEXCOORD1;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 scroll = IN.uv_Diffuse;

            float scrollU = _ScrollU * _Time * 5.0;
            float scrollV = _ScrollV * _Time * 5.0;

            scroll += float2(scrollU, scrollV);

            float4 texture0 = tex2D(_Diffuse, scroll);

            float3 tint_color = texture0.rgb * _Tint;
            o.Albedo = float3(scroll.x, scroll.y, 0);
            
            float3 glow_color = texture0.rgb * texture0.a;
            glow_color *= _Glow * 0.25;
            o.Emission = glow_color; //texture0.rgb * tex2D(_Diffuse, scroll);
            o.Specular = _Specular;
            o.Gloss = 1;
        }

        ENDCG
    }
}
