// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Light" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
		_FalloffTex ("FallOff", 2D) = "" {}
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend One One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f 
			{
				float4 normal : TEXCOORD0;
				float4 uvShadow : TEXCOORD1;
				float4 uvFalloff : TEXCOORD2;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;

			float3 triplanar(float4 worldPos, float2 uv, float4 blendAxes) {
				//float3 scaledWorldPos = worldPos / scale;

				float4 xProjection = tex2D(_ShadowTex, uv) * blendAxes.x;
				float4 yProjection = tex2D(_ShadowTex, uv) * blendAxes.y;
				float4 zProjection = tex2D(_ShadowTex, uv) * blendAxes.z;
				
				return xProjection + yProjection + zProjection;
			}
			
			//v2f vert (float4 vertex : POSITION)
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvShadow = mul (unity_Projector, v.vertex);
				o.normal = float4(v.normal, 0);
				o.uvFalloff = mul (unity_ProjectorClip, v.vertex);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// in case of NPOT, escape the render to avoid clamping artifacts
				fixed4 res = fixed4(0,0,0,0);
				if(!(i.uvShadow.x <= 1. && i.uvShadow.x >= 0 && i.uvShadow.y <= 1. && i.uvShadow.y >= 0))
					return res;

				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS.rgb = float4(triplanar(i.pos, i.uvShadow, i.normal), 1) * _Color.rgb;
				texS.a = 1.0 - texS.a;

				fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				res = texS * texF.a;
				
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				
				return res;
			}

			ENDCG
		}
	}
}
