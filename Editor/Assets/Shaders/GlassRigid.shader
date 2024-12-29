Shader "Custom/GlassRigid"
{
    Properties
    {
       _Diffuse("Diffuse", 2D) = "white" {}
       _Tint("Tint", Color) = (1, 1, 1, 0.5)
	   _ChromaAbsorption("ChromaAbsorption", Float) = 0.0
       _Fresnel("Fresnel", Float) = 0.0
       _Cube ("Reflection Map", CUBE) = "" {}
	   _Reflection("Reflection", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf StandardSpecular fullforwardshadows addshadow alpha

        sampler2D _Diffuse;
		samplerCUBE _Cube;
		float4 _Tint;
		float _ChromaAbsorption;
		float _Fresnel;
		float _Reflection;
		
        struct Input
        {
            float2 uv_Diffuse;
			float3 worldRefl;
        };

        void surf(Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float4 texture0 = tex2D(_Diffuse, IN.uv_Diffuse);
			float3 texturecube0 = texCUBE (_Cube, IN.worldRefl);
			float3 reflection_intensity = texturecube0.rgb * _Reflection;
            o.Albedo = texture0.rgb + (texture0.rgb * _ChromaAbsorption);
			o.Specular = reflection_intensity;
            o.Alpha = texture0.a * _Tint.a;
		    //o.Emission = texture0.rgb * texture0.a;
		}

        ENDCG
    }
}