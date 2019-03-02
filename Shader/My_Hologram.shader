Shader "Custom/My_Hologram" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_BackGroundAlpha("BackGround Alpha", Range(0,1)) = 1
		[space]
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Mask", 2D) = "white" {}
		_NoiseTileValue("Noise Tile Value", float) = 1
		_NoiseFlowY("Noise Flow speed Y", float) = 1
		_FlowTerm("flow Term", float) = 1
		_FlowWidth("Flow Width", float) = 1
		[space]
			
		_RampTex("Ramp Image", 2D) = "white" {}
		_RampReadSpeed("Ramp Read Speed", float) = 1
	}
	SubShader {
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
		float _BackGroundAlpha;
		float _NoiseTileValue;
		float _FlowTerm;
		float _FlowWidth;
		float _NoiseFlowY;
		float _RampReadSpeed;


		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 mask = tex2D (_NoiseTex,float2(IN.worldPos.x, IN.worldPos.y) * _NoiseTileValue);
			//fixed4 mask = tex2D(_NoiseTex, IN.uv_NoiseTex);
			fixed4 ramp = tex2D(_RampTex, float2(_Time.y * _RampReadSpeed, 0.5));

			float tmp = pow(frac(IN.worldPos.g * _FlowTerm + _Time.y * _NoiseFlowY), _FlowWidth);
			//o.Albedo = c.rgb;
			o.Emission = _EmissionColor * tmp * (1 - mask.r) * ramp.r;
			//o.Emission = ((1 - _BackGroundAlpha) * _EmissionColor * tmp * (1 - mask.r) * ramp.r) + _BackGroundAlpha * c.rgb * (1-tmp);
			o.Alpha = c.a * (_BackGroundAlpha + (1- _BackGroundAlpha) * tmp);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
