Shader "Custom/3D_Text_Dissolve" {
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
		_DissolveTex("Dissolve Texture", 2D) = "white" {}

		[Space(20)]
		[HDR]_BaseEmission("Base Emission", Color) = (1,1,1,1)
		[HDR]_BorderEmission("Border Emission", Color) = (1,1,1,1)

		[Space(20)]
		_DissolveValue("Dissolve Value", Range(0,1)) = 0
		_DissolveBorder("Dissolve Border Mul", float) = 1.15

	}

		SubShader{

			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma surface surf Standard alphatest:_Cutoff
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _DissolveTex;

			struct Input {
				float2 uv_MainTex;
				float2 uv_DissolveTex;
			};

			fixed4 _Color;
			fixed4 _BaseEmission;
			fixed4 _BorderEmission;
			float _DissolveValue;
			float _DissolveBorder;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 mask = tex2D(_DissolveTex, IN.uv_DissolveTex);

				float Dissolve_Secton = step(mask.r, _DissolveValue);
				float Dissolve_Secton2 = step(mask.r, _DissolveValue * _DissolveBorder);
				
				o.Emission = Dissolve_Secton * _BaseEmission + (Dissolve_Secton + Dissolve_Secton2) % 2 * _BorderEmission;
				o.Alpha = c.a * Dissolve_Secton2;
			}
			ENDCG
	}
}
