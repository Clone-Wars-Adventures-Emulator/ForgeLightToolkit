Shader "Custom/Crystal"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Refraction("Refraction", Float) = 0.0
        _InsideGlow("InsideGlow", Float) = 0.0
        _InsideSpecular("InsideSpecular", Float) = 0.0
        _OutsideGlow("OutsideGlow", Float) = 0.0
        _OutsideSpecular("OutsideSpecular", Float) = 0.0
        _TintSemantic("TintSemantic", Color) = (0, 0, 0, 0)
        _FadeStencil("FadeStencil", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf StandardSpecular fullforwardshadows addshadow alpha

        sampler2D _Diffuse;
        float _InsideGlow;
		float _OutsideGlow;
        float _InsideSpecular;
		
		struct Input
        {
            float2 uv_Diffuse;
        };

        void surf(Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float4 texture0 = tex2D(_Diffuse, IN.uv_Diffuse);
			float3 glow_color = texture0.rgb * texture0.a;
            float inside_spec = texture0.rgb * texture0.a;
			o.Albedo = texture0.rgb;
            o.Emission = glow_color;
			o.Specular = .5 * inside_spec;
            // TODO: Specular
        }

        ENDCG
    }
}