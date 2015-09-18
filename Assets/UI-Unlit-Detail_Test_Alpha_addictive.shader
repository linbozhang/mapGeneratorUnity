Shader "UI/Unlit/Detail_Test_Alpha_Addictive"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_DetailTex ("Detail (RGB) Alpha(A)", 2D) = "white" {}
		_Strength ("Detail Strength", Range(0.0, 1.0)) = 0.2
		_Add("additive",Range(1.0,10))=1.0
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Blend SrcAlpha One
		ColorMask [_ColorMask]

		Pass
		{
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float2 texcoord2 : TEXCOORD1;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				sampler2D _DetailTex;
				float4 _MainTex_ST;
				float4 _DetailTex_ST;
				float4 _DetailTex_TexelSize;
				fixed4 _Color;
				fixed _Strength;
				float _Add;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord2 = TRANSFORM_TEX(v.texcoord2, _DetailTex);
					o.color = v.color;
#ifdef UNITY_HALF_TEXEL_OFFSET
					o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
					fixed4 detail = tex2D(_DetailTex, i.texcoord2);
					col.rgb = lerp(col.rgb, detail.rgb, detail.a * _Strength);
					col.a=(col.a*( 1-detail.a));
					//col.a=lerp(col.a,detail.a,detail.a * _Strength);
					col = _Add*col * _Color;
					//clip (col.a - 0.01); discard the current pixel if the parameter is less than zero.
					return col;
				}
			ENDCG

		}
	}
}
