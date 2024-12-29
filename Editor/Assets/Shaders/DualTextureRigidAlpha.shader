Shader "Custom/DualTextureRigidAlpha"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Diffuse2("Diffuse2", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 1, 1, 1)
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf BlinnPhong fullforwardshadows addshadow alphatest:_Cutoff
	
        sampler2D _Diffuse;
        sampler2D _Diffuse2;
        float4 _Tint;

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
            o.Alpha = texture0.a;
			o.Specular = 0.5;
			o.Gloss = 0.25;
        }

        ENDCG
    }
}