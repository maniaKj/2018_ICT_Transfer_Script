Shader "Custom/noise_Study" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_GridTex("Grid Map", 2D) = "white" {}
		[HDR]_GridColor("Grid Emission Color", Color) = (0,0,0,0)
		_MaskTex("Mask Map", 2D) = "white" {}
		_MaskCutoff("Maks Cut off", Range(0,1)) = 0.0
		_GridEmission("Grid Emission Value", float) = 0.0
		_NoiseSpeed("Noise Speed Value", float) = 1.0
		_NoiseBias("Noise Bias", Range(0,1)) = 0.0
		_NoiseWidth("Noise Width", Range(0,2)) = 0.5
		_NoiseColorChange("Noise Color Change Speed", float) = 1.0
		_ScaleSize("Scale Size", float) = 1.0
		_ScaleSpeed("Scale Speed", float) = 1.0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		

	}
	SubShader {
		Tags{ "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _GridTex;
		sampler2D _MaskTex;
		fixed4 _GridColor;
		float _GridEmission;
		float _NoiseSpeed;
		float _NoiseBias;
		float _NoiseWidth;
		float _NoiseColorChange;
		float _ScaleSize;
		float _ScaleSpeed;
		float _MaskCutoff;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GridTex;
			float2 uv_MaskTex;
			float3 viewDir;
			float3 worldPos;
			
		};

		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 grid = tex2D(_GridTex, IN.uv_GridTex);
			fixed4 mask = tex2D(_MaskTex, IN.uv_MaskTex);

			float rim = dot(o.Normal, IN.viewDir);
			float rim2 = pow(1 - saturate(rim), 3);
			
			//o.Alpha = rim2;
			rim = pow(1 - rim, 3);
			//o.Emission = step(_MaskCutoff,(mask.r))   * _GridColor * rim * c.a;
			//o.Alpha = rim + pow(frac(IN.worldPos.g - _Time.y * _ScaleSpeed), _ScaleSize);
			//o.Emission = float3(1,1,0);

			float time_Current = saturate(_Time.y);
			float outline;
			//if (frac(IN.worldPos.g - _Time.y * _ScaleSpeed) >= _MaskCutoff) outline = 1;
			//else outline = 0;
			float time_factor = frac(_Time.y*_NoiseSpeed);
			outline = (step(time_factor, 0.5)*time_factor + step(1- time_factor, 0.5) * (1 - time_factor)) * _NoiseWidth + _NoiseBias;
			o.Albedo = c.rgb;
			o.Emission = (step(grid.r,_GridEmission)) * step(mask.r, outline) * _GridColor;
			o.Alpha = c.a;

			

			
		}
		ENDCG
	}
	FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}
