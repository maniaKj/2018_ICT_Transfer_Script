Shader "Custom/Shaking_Study" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex("MaskTexture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Cutoff("CutOff", float) = 0.5
		_MoveSpeed("move_Speed",float) = 1.0
		_VertexAmount("Vertex Move Amount", float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue" = "AlphaTest" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
		

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MaskTex;
		//float _Move;
		//float _Timing;
		float _MoveSpeed;
		float _VertexAmount;

		void vert(inout appdata_full v) {
			//v.vertex.x += sin(_Time.y * _MoveSpeed);
			//v.vertex.y += sin(abs(v.texcoord.x * _VertexAmount) + _Time.y);
			//v.vertex.x += _VertexAmount	* (step(0.5, sin(_Time.y * 2.0 + v.vertex.y * 1.0)) * step(0.95, sin(_Time.y*_MoveSpeed * 0.5)));
			float tmp = sin(_Time.y*_MoveSpeed);
			//v.vertex.x += _VertexAmount * v.texcoord.x * tmp * step(0.85, tmp);
			v.vertex.xyz += _VertexAmount * v.normal.yzx * tmp * step(0.85, tmp);
		}

		struct Input {
			float2 uv_MainTex;
			float4 color:COLOR;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
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
			//float3 viewNormal = mul((float3x3)UNITY_MATRIX_V, IN.worldNormal.rgb);
			o.Albedo = c.rgb;
			//o.Emission = IN.color.rgb;
			//o.Emission = viewNormal;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
