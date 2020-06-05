Shader "Custom/Thank_text_Shader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_DissolveTex("Dissolve Tex", 2D) = "white" {}

		_Cut("Alpha Cut", Range(0,1)) = 0
		_CutBorder("Cut Border Value", Range(1,10)) = 0
		[HDR]_OutColor("OutColor", Color) = (1,1,1,1)
		[HDR]_DissolveColor("DissolveColor", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard alphatest:_Cutoff


		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DissolveTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_DissolveTex;
	};

	fixed4 _Color;
	float _Cut;
	float _CutBorder;
	float4 _OutColor;
	float4 _DissolveColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		float4 Dissolve = tex2D(_DissolveTex, IN.uv_DissolveTex);
		o.Albedo = c.rgb;

		float Dissolve_Section, Dissolve_Section_2;
		Dissolve_Section = step(Dissolve.r, _Cut);
		Dissolve_Section_2 = step(Dissolve.r, _Cut * _CutBorder);

		o.Emission = step(1.5, Dissolve_Section + Dissolve_Section_2) * _OutColor + (Dissolve_Section + Dissolve_Section_2) % 2 * _DissolveColor;
		o.Alpha = Dissolve_Section_2 * c.a;

	}
	ENDCG
	}
		FallBack "Diffuse"
}
