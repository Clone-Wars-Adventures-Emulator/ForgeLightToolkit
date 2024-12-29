Shader "Custom/ShieldRigid"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _BumpMap1("BumpMap1", 2D) = "white" {}
        _BumpMap2("BumpMap2", 2D) = "white" {}
        _Fade("Fade", Float) = 0.0
        _OuterColor("OuterColor", Color) = (0, 0, 0, 0)
        _InnerColor("InnerColor", Color) = (0, 0, 0, 0)
        _BaseColor("BaseColor", Color) = (0, 0, 0, 0)
        _Bumpiness1("Bumpiness1", Float) = 0.0
        _Bumpiness2("Bumpiness2", Float) = 0.0
        _TexScrollX0("TexScrollX0", Float) = 0.0
        _TexScrollZ0("TexScrollZ0", Float) = 0.0
        _TexScrollX1("TexScrollX1", Float) = 0.0
        _TexScrollZ1("TexScrollZ1", Float) = 0.0
        _TexScrollX2("TexScrollX2", Float) = 0.0
        _TexScrollZ2("TexScrollZ2", Float) = 0.0
        _TexScale1("TexScale1", Float) = 0.0
        _TexScale2("TexScale2", Float) = 0.0
        _ZRange("ZRange", Float) = 0.0
        _Refraction("Refraction", Float) = 0.0
        _OuterFresnel("OuterFresnel", Float) = 0.0
        _OuterScale("OuterScale", Float) = 0.0
        _DoubleSided("DoubleSided", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM

        #pragma target 3.0
        #pragma surface surf BlinnPhong fullforwardshadows addshadow alpha:fade

        sampler2D _Diffuse;
        float3 _BaseColor;
		float3 _InnerColor;
		float3 _OuterColor;
		
		float _Bumpiness1;
        float _Bumpiness2;
		
        float _TexScale1;
        float _TexScrollX1;
        float _TexScrollZ1;
        sampler2D _BumpMap1;

        float _TexScale2;
        float _TexScrollX2;
        float _TexScrollZ2;
        sampler2D _BumpMap2;

        float _TexScrollX0;
        float _TexScrollZ0;
        struct Input
        {
            float2 uv_Diffuse;
            float2 uv_BumpMap1;
			float2 uv_BumpMap2;
		};

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 texScroll1 = IN.uv_BumpMap1;
            float2 texScroll2 = IN.uv_BumpMap2;
			
			float4 c = tex2D(_Diffuse, IN.uv_Diffuse);
			float3 base_color = c.rgb * _BaseColor;
			float3 inner_color = c.rgb * _InnerColor;
			float3 outer_color = c.a * _OuterColor;
			float4 n1 = tex2D(_BumpMap1, texScroll1);
            float4 n2 = tex2D(_BumpMap2, texScroll2);
			
			float bumpiness1 = n1;
			float bumpiness2 = n2;
			
            float texScrollX1 = _TexScrollX1 * _Time * 5.0;
            float texScrollZ1 = _TexScrollZ1 * _Time * 5.0;

            float texScrollX2 = _TexScrollX2 * _Time * 5.0;
            float texScrollZ2 = _TexScrollZ2 * _Time * 5.0;

            float texScrollX0 = _TexScrollX0 * _Time * 5.0;
            float texScrollZ0 = _TexScrollZ0 * _Time * 5.0;
			
			texScroll1 += float2(texScrollX1, texScrollZ1);
            texScroll2 += float2(texScrollX2, texScrollZ2);
			
			float3 b1 = UnpackNormal(n1);
            float3 b2 = UnpackNormal(n2);
			
			o.Normal = (b1 * bumpiness1) + (b2 * bumpiness2);
            o.Albedo = c.rgb * _BaseColor + _InnerColor;
            o.Alpha = c.a * .4;
			o.Emission = c.rgb;
			o.Specular = 0.1 *_InnerColor;
		    o.Gloss = 1 * _OuterColor;
		}

        ENDCG
    }
}