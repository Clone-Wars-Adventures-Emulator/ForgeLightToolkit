Shader "Custom/SimpleRigidAlphaScreen"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Fade("ScrollV", Float) = 0.0
        _ScrollV("ScrollV", Float) = 0.0
        _ScrollU("ScrollU", Float) = 0.0
        _Glow("Glow", Float) = 0.0
        _DoubleSidedDefaultFalse("DoubleSidedDefaultFalse", Integer) = 0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "Transparent" }
        LOD 200
        Cull Front

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf StandardSpecular fullforwardshadows addshadow alpha

        sampler2D _Diffuse;
		float _Glow;

        struct Input
        {
            float2 uv_Diffuse;
        };

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float4 texture0 = tex2D (_Diffuse, IN.uv_Diffuse);
			float3 glow_color = texture0.rgb * texture0.a;
			glow_color *= _Glow;
            o.Albedo = texture0.rgb;
            o.Alpha = texture0.a/1.33;
			o.Specular = 0.5;
			o.Emission = glow_color;
        }

        ENDCG
    }
}