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

			StructuredBuffer<float4> _Origins, _Destinations;
			float _Interpolator;

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
				
				uint instanceID = GetIndirectInstanceID(svInstanceID);
				
				float4 pos = float4(lerp(_Origins[svInstanceID].xyz, _Destinations[svInstanceID].xyz, _Interpolator), v.vertex.a);

				i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);

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

				return float4(col, 1);
			}

			ENDCG
		}
	}
}