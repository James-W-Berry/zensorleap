// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

Shader "Magic Leap/Unlit/LineTexture"
{
	Properties
	{
		_LineTex ("LineTexture", 2D) = "white" {}
		_Fade ("Line Tex Fade", Range(0,1))= 0
		//_Ramp ("Gradient Ramp", 2D) = "black" {}
		_Color ("Line Color", Color) = (1,1,1)
		_FailColor ("Fail Color", Color) = (1,0,0)
		_PassColor ("Pass Color", Color) = (0,1,0)
		_Point01 ("Line Point 1", Range(0,1))= 0
		//_Point02 ("Line Point 2", Range(0,1))= 0
		_Valid ("Valid Line", Range(0,1))= 0
	}
	SubShader
	{
		 Lighting Off
		 Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf NoLighting

	  fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
     {
         fixed4 c;
         c.rgb = s.Albedo; 
         c.a = s.Alpha;
         return c;
     }

      struct Input {
          float2 uv_LineTex;
		  float2 uv_Ramp;
      };
	  sampler2D _LineTex;
	  //sampler2D _Ramp;
	  fixed3 _Color;
	  fixed3 _PassColor;
	  fixed3 _FailColor;
	  fixed _Point01;
	  fixed _Point02;
	  fixed _Valid;
	  fixed _Fade;

      void surf (Input IN, inout SurfaceOutput o) {
		
		float3 linetex = tex2D(_LineTex, IN.uv_LineTex).rgb;
		//float3 ramp = tex2D(_Ramp, IN.uv_Ramp).rgb;
		float3 p01color = lerp(_FailColor,_PassColor,_Point01);
		//float3 p02color = lerp(_FailColor,_PassColor,_Point02);
		//float3 rampcomp = lerp(p01color,p02color,ramp.r);
		float3 validcomp = lerp(p01color,_Color,_Valid);
        
		o.Albedo = validcomp*saturate(linetex+_Fade);;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }