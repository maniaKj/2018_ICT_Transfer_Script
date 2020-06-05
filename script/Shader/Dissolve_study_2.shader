Shader "Custom/Dissolve_study_2" {
		Properties{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
			_Metallic("Metallic", Range(0,1)) = 0.0
			_Cut("Alpha Cut", Range(0,1)) = 0
			_CutBorder("Cut Border Value", Range(1,10)) = 0
			[HDR]_OutColor("OutColor", Color) = (1,1,1,1)
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
		sampler2D _NoiseTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoiseTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Cut;
		float _CutBorder;
		float4 _OutColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			float4 noise = tex2D(_NoiseTex, IN.uv_NoiseTex);
			o.Albedo = c.rgb;

			float alpha, outline;
			if (noise.r >= _Cut * _CutBorder) { alpha = 1; outline = 0; }
			else if (noise.r < _Cut * _CutBorder && noise.r >= _Cut) { alpha = 1;  outline = 1; }
			else { alpha = 0; outline = 0; }
			o.Emission = outline * _OutColor.rgb;
			o.Alpha = c.a * alpha;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
