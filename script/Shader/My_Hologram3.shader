Shader "Custom/My_Hologram3" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[HDR]_EmissionColor("Emission Color Scale", Color) = (0,0,0,0)
		[HDR]_EmissionColor2("Emission Color Background", Color) = (0,0,0,0)
		[space]
	_MainTex("Albedo (RGB)", 2D) = "white" {}
	_NoiseTex("Noise Mask", 2D) = "white" {}

	_NoiseFlowY("Noise Flow speed Y", float) = 1
		_FlowTerm("flow Term", float) = 1
		_FlowWidth("Flow Width", float) = 1
		[space]
	_AlphaCutTex("Alpha Mask", 2D) = "white" {}
	_AlphaCut("Alpha Cut", Range(0,1)) = 0
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
	sampler2D _AlphaCutTex;
	sampler2D _RampTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_NoiseTex;
		float2 uv_AlphaCutTex;
		float2 uv_RampTex;
		float2 worldPos;
	};

	fixed4 _Color;
	fixed4 _EmissionColor;
	fixed4 _EmissionColor2;
	float _FlowTerm;
	float _FlowWidth;
	float _NoiseFlowY;
	float _RampReadSpeed;
	float _AlphaCut;


	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed4 alphaTex = tex2D(_AlphaCutTex, IN.uv_AlphaCutTex);
		fixed4 mask = tex2D(_NoiseTex, IN.uv_NoiseTex);
		fixed4 ramp = tex2D(_RampTex, float2(_Time.y * _RampReadSpeed, 0.5));

		float Scale_Section = pow(frac(IN.worldPos.g * _FlowTerm + _Time.y * _NoiseFlowY), _FlowWidth);
		float Alpha_Cut_Section = step(_AlphaCut, alphaTex.r);
		float Dissolve_Section = step(Alpha_Cut_Section + step(_AlphaCut * 1.1, alphaTex.r), 1.5);
		float Emission_Section = Scale_Section * (1 - Dissolve_Section) * (1 - mask.r) + Dissolve_Section;
		o.Albedo = c.rgb;
		o.Emission = _EmissionColor * Emission_Section + c * _EmissionColor2 * (1 - Emission_Section);
		o.Alpha = c.a * Alpha_Cut_Section * ramp.r;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
