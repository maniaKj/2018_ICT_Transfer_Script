Shader "Custom/Background_Noise_2" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		[space]
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Mask", 2D) = "white" {}
		_CutBorder("Cut Border", float) = 1.1
		_NoiseBias("Bias", Range(0,1)) = 0.5
		_NoiseWidth("Width", Range(0,2)) = 1
		_NoiseSpeed("Speed", float) = -0.5
		_AlphaBias("AlphaBias",Range(0,1)) = 0.0
		_TimeSpeed("Time speed Mul", float) = 1
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
#pragma surface surf Standard alphatest:_Cutoff
#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _NoiseTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_NoiseTex;
		float2 worldPos;
	};

	fixed4 _Color;
	fixed4 _EmissionColor;
	float _RampReadSpeed;
	float _CutBorder;
	float _TimeSpeed;
	float _NoiseBias;
	float _NoiseWidth;
	float _NoiseSpeed;
	float _AlphaBias;


	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed4 mask = tex2D(_NoiseTex,float2(IN.uv_NoiseTex.x, IN.uv_NoiseTex.y + _Time.y * _NoiseSpeed));

		float time_Value = _NoiseBias + _NoiseWidth * abs(0.5 - 1 * frac(_Time.y * _TimeSpeed));

		o.Emission = _EmissionColor;
		o.Alpha = c.a * (pow(abs(time_Value - mask.r), 1.5));
	}
	ENDCG
	}
		FallBack "Diffuse"
}
