Shader "Custom/Particle" {

	SubShader {

		Pass {
			ZWrite On

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
				uint index : TEXCOORD3;
			};

			StructuredBuffer<float4> _Positions;
			float4x4 _FinalTransform;
			int _BatchIndex;

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
				
				uint instanceCount = GetIndirectInstanceCount();
				uint instanceID = GetIndirectInstanceID(svInstanceID);
				uint commandID = GetCommandID(0);

				float4 particlePos = _Positions[svInstanceID + commandID * instanceCount];

				// float4 pos = v.vertex + mul(_FinalTransform, float4(particlePos.xyz, 1));
				float4 pos = v.vertex + float4(particlePos.xyz * 2, 1.0f);
				// pos.x += _BatchIndex * 4;

				i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.index = commandID;
				// i.index = particlePos.w;
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

				// col *= float3(hash(_BatchIndex * 2), hash(_BatchIndex * 4), hash(_BatchIndex * 3));
				// col = 1;

				// col *= saturate(DotClamped(_WorldSpaceLightPos0.xyz, i.normal) + 0.15f);

				// col = pow(saturate(length(i.worldPos * 0.25f)), 0.75f) + 0.01f;
				// col += hash(length(i.worldPos.xyz) * 10000) * 0.05f;
				col = 0;
				return float4(col, 1);
			}

			ENDCG
		}
	}
}