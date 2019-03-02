Shader "Custom/Line_Dissolve" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Cut("Activating Bar", Range(0,1)) = 0
		_CutBorder("Cut Border Value", Range(1,10)) = 0
		[HDR]_OutColor("OutColor", Color) = (1,1,1,1)
		[HDR]_OutColor2("OutColor2", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard alpha:fade


		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _NoiseTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_NoiseTex;
		float3 worldPos;
		float3 localPos;
	};

	fixed4 _Color;
	float _Cut;
	float _CutBorder;
	float4 _OutColor;
	float4 _OutColor2;


		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		float4 noise = tex2D(_NoiseTex, IN.uv_NoiseTex);

		float alpha, outline;
		alpha = step(_Cut, noise.r);
		outline = step(noise.r, _Cut * _CutBorder);

		float step_Tmp = step(outline, 0.5);
		o.Alpha = alpha;
		o.Emission = _OutColor2 * (1 - step_Tmp) + _OutColor * step_Tmp;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
