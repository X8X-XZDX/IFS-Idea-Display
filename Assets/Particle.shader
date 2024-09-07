Shader "Custom/Particle" {

	SubShader {

		Pass {
			Cull Off

			Tags {
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma vertex vp
			#pragma fragment fp

			#include "UnityCG.cginc"
			#define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
			#include "UnityIndirect.cginc"
			#include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"

			struct VertexData {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 normal : TEXCOORD2;
			};

			StructuredBuffer<float4> _Positions;

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
				
				uint instanceID = GetIndirectInstanceID(svInstanceID);

				float4 pos = v.vertex * 0.025f + float4(_Positions[svInstanceID].xyz, 0);

				i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);
				i.normal = UnityObjectToWorldNormal(v.normal);

				return i;
			}

			float4 fp(v2f i) : SV_TARGET {
				float3 col = 1;

				i.worldPos *= 0.1f;

				col.r = abs(sin(i.worldPos.x));
				col.g = abs(sin(i.worldPos.y));
				col.b = abs(i.worldPos.z);

				col *= 2.5f;
				col += 0.15f;
				// col = 1;

				// col *= DotClamped(_WorldSpaceLightPos0.xyz, i.normal) + 0.1f;

				return float4(col, 1);
			}

			ENDCG
		}
	}
}