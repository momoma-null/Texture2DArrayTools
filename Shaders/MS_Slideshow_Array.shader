// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/Surface/Slideshow_Array"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainArr ("Albedo (RGB)", 2DArray) = "" {}
		[IntRange] _Number ("Number", Range(1, 64)) = 1
		_EmissionRatio ("Emission Ratio", Range(0, 1)) = .0
		_Glossiness ("Smoothness", Range(0, 1)) = .5
		_Metallic ("Metallic", Range(0, 1)) = .0
		_DisplayTime ("DisplayTime", Float) = 3.
		_FadeTime ("Fade Time", Float) = .5
	}
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.5
		//#pragma require 2darray

		UNITY_DECLARE_TEX2DARRAY(_MainArr);

		struct Input
		{
			float2 uv_MainArr;
		};

		fixed _Number;
		fixed _EmissionRatio;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _DisplayTime, _FadeTime;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float unitTime = _DisplayTime + _FadeTime;
			float t_I = floor(_Time.y / unitTime);
			float t_D = frac(_Time.y / unitTime);
			float d0 = fmod(t_I, _Number);
			float d1 = fmod(t_I + 1., _Number);
			fixed4 c0 = UNITY_SAMPLE_TEX2DARRAY(_MainArr, float3(IN.uv_MainArr, d0));
			fixed4 c1 = UNITY_SAMPLE_TEX2DARRAY(_MainArr, float3(IN.uv_MainArr, d1));
			c0 = lerp(c0, c1, smoothstep(_DisplayTime / unitTime, 1., t_D));
			c0 *= _Color;
			o.Albedo = c0.rgb * (1. - _EmissionRatio);
			o.Emission = c0.rgb * _EmissionRatio;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c0.a;
		}
		ENDCG
	}
	FallBack Off
}
