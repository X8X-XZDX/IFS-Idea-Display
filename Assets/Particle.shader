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
				int index : TEXCOORD3;
			};

			StructuredBuffer<float4> _Positions;

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
				
				uint instanceID = GetIndirectInstanceID(svInstanceID);

				float4 particlePos = _Positions[svInstanceID];

				float4 pos = v.vertex * 0.025f + float4(particlePos.xyz, 0);

				i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.index = particlePos.w;

				return i;
			}

			float hash(uint n) {
				// integer hash copied from Hugo Elias
				n = (n << 13U) ^ n;
				n = n * (n * n * 15731U + 0x789221U) + 0x1376312589U;
				return float(n & uint(0x7fffffffU)) / float(0x7fffffff);
			}

			float4 fp(v2f i) : SV_TARGET {
				float3 col = 1;

				i.worldPos *= 0.45f;

				col *= float3(hash(i.index * 2), hash(i.index * 4), hash(i.index * 3));
				// col = 1;

				col *= saturate(DotClamped(_WorldSpaceLightPos0.xyz, i.normal) + 0.15f);

				return float4(col, 1);
			}

			ENDCG
		}
	}
}