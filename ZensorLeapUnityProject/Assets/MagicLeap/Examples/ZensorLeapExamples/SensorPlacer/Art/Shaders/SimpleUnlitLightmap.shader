// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

Shader "Magic Leap/Unlit/SimpleUnlitLightmap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Lightmap ("Lightmap", 2D) = "black" {}
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
          float2 uv_MainTex;
		  float2 uv2_Lightmap;
      };
	  sampler2D _MainTex;
	  sampler2D _Lightmap;

      void surf (Input IN, inout SurfaceOutput o) {
		
		float3 mainTex = tex2D(_MainTex, IN.uv_MainTex).rgb;
		float3 lightmapTex = tex2D(_Lightmap, IN.uv2_Lightmap).rgb;
        
		o.Albedo = mainTex * lightmapTex;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }