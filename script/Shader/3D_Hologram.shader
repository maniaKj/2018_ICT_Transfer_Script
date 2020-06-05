Shader "Custom/3D_Hologram" {
	Properties
	{
		// General
		_Brightness("Brightness", Range(0.1, 6.0)) = 3.0
		_Alpha("Alpha", Range(0.0, 1.0)) = 1.0
		[HDR]_Emission_Color("Emission Color", Color) = (1,1,1,1)
		_Direction("Direction", Vector) = (0,1,0,0)
		// Main Color
		_MainTex("MainTexture", 2D) = "white" {}
		_MainColor("MainColor", Color) = (1,1,1,1)
		// Rim/Fresnel
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0.1, 10)) = 5.0
		// Glow
		_GlowTiling("Glow Tiling", Range(0.01, 1.0)) = 0.05
		_GlowSpeed("Glow Speed", Range(-10.0, 10.0)) = 1.0
		// Glitch
		_GlitchSpeed("Glitch Speed", Range(0, 50)) = 1.0
		_GlitchIntensity("Glitch Intensity", Float) = 0
		// Alpha Flicker
		_FlickerTex("Flicker Control Texture", 2D) = "white" {}
		_FlickerSpeed("Flicker Speed", Range(0.01, 100)) = 1.0

		_AlphaCut("Alpha Cut", Range(0,1)) = 0
		_AlphaMul("Alpha Mul", float) = 1
		_CutBorder("Cut Border", float) = 1.1
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		ColorMask RGB
		Cull Back

		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 worldVertex : TEXCOORD1;
		float3 viewDir : TEXCOORD2;
		float3 worldNormal : NORMAL;
	};

	sampler2D _MainTex;
	sampler2D _FlickerTex;
	float4 _Emission_Color;
	float4 _Direction;
	float4 _MainTex_ST;
	float4 _MainColor;
	float4 _RimColor;
	float _RimPower;
	float _GlitchSpeed;
	float _GlitchIntensity;
	float _Brightness;
	float _Alpha;
	float _GlowTiling;
	float _GlowSpeed;
	float _FlickerSpeed;
	float _AlphaCut;
	float _AlphaMul;
	float _CutBorder;

	v2f vert(appdata v)
	{
		v2f o;
		v.vertex.x += _GlitchIntensity * (step(0.5, sin(_Time.y * 2.0 + v.vertex.y * 1.0)) * step(0.99, sin(_Time.y*_GlitchSpeed * 0.5)));

		o.vertex = UnityObjectToClipPos(v.vertex);

		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
		o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldVertex.xyz));

		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 texColor = tex2D(_MainTex, i.uv);

	fixed4 VertexW = dot(i.worldVertex, normalize(float4(_Direction.xyz, 1.0)));
	fixed4 VertexZ = VertexW / 2;
	fixed4 dirVertex = (VertexW + 1) / 2;

	// Glow
	float glow = 0.0;
	glow = frac(dirVertex * _GlowTiling - _Time.x * _GlowSpeed);

	// Flicker
	fixed4 flicker = tex2D(_FlickerTex, _Time * _FlickerSpeed);

	// Rim Light
	half rim = 1.0 - saturate(dot(i.viewDir, i.worldNormal));
	fixed4 rimColor = _RimColor * pow(rim, _RimPower);
	//_CutBorder
	float Dissolve_Section = step(_AlphaCut, frac(dirVertex * _AlphaMul));
	float Dissolve_Section2 = step(Dissolve_Section + step(_AlphaCut * _CutBorder, frac(dirVertex * _AlphaMul)), 1.5);
	fixed4 col = texColor * _MainColor + (glow * 0.35 * _MainColor) * (1-Dissolve_Section2) + Dissolve_Section2 * _Emission_Color + rimColor;
	col.a = Dissolve_Section * texColor.a * _Alpha * (rim + glow) * flicker;

	col.rgb *= _Brightness;

	return col;
	}
		ENDCG
	}
	}
}
