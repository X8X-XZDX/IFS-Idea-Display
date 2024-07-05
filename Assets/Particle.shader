Shader "Custom/Particle" {

	SubShader {

		Pass {
			Tags {
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma vertex vp
			#pragma fragment fp

			#include "UnityCG.cginc"
			#define UNITY_INDIRECT_DRAW_ARGS IndirectDrawArgs
			#include "UnityIndirect.cginc"

			struct VertexData {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			StructuredBuffer<float3> _ParticlePositions;

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
				
				uint instanceID = GetIndirectInstanceID(svInstanceID);
				
				float4 pos = float4(_ParticlePositions[svInstanceID], v.vertex.a);

				i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);

				return i;
			}

			float4 fp(v2f i) : SV_TARGET {
				float3 col = 1;

				return float4(saturate(i.worldPos.x * 5) * 0.5f + 0.5f, saturate(i.worldPos.y * 5) * 0.5f + 0.5f, saturate(i.worldPos.z * 5) * 0.5f + 0.5f, 1);
			}

			ENDCG
		}
	}
}