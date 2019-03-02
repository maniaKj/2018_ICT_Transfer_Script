Shader "Custom/Background_Noise" {
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
		_TimeSpeed("Time speed Mul", float) = 1
		[space]

		_RampTex("Ramp Image", 2D) = "white" {}
		_RampReadSpeed("Ramp Read Speed", float) = 1
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
	sampler2D _RampTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_NoiseTex;
		float2 uv_RampTex;
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


	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed4 mask = tex2D(_NoiseTex,float2(IN.uv_NoiseTex.x, IN.uv_NoiseTex.y + _Time.y * _NoiseSpeed));
		//fixed4 mask = tex2D(_NoiseTex, IN.uv_NoiseTex);
		fixed4 ramp = tex2D(_RampTex, float2(_Time.y * _RampReadSpeed, 0.5));

		//float tmp = pow(frac(IN.worldPos.g * _FlowTerm + _Time.y * _NoiseFlowY), _FlowWidth);

		float time_Value = _NoiseBias + _NoiseWidth * abs(0.5 - 1 * frac(_Time.y * _TimeSpeed));
		float Dissolve_Section = step(time_Value, mask.r);
		float Dissolve_Section2 = step(Dissolve_Section + step(time_Value * _CutBorder, mask.r), 1.5);
		//fixed4 col = texColor * _MainColor + (glow * 0.35 * _MainColor) * (1 - Dissolve_Section2) + Dissolve_Section2 * _Emission_Color + rimColor;

		//o.Albedo = c.rgb;
		o.Emission = _EmissionColor * frac(mask.r * _Time.y) * c.rgb;
		//o.Emission = ((1 - _BackGroundAlpha) * _EmissionColor * tmp * (1 - mask.r) * ramp.r) + _BackGroundAlpha * c.rgb * (1-tmp);
		o.Alpha = c.a * (1 - (Dissolve_Section2 + Dissolve_Section ) % 2) * ramp.r;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
