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
			#include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"

			struct VertexData {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float occlusion : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			
			StructuredBuffer<float> _OcclusionGrid;
			int _GridSize, _GridBounds;

			uint to1D(uint3 pos) {
				return pos.x + pos.y * _GridSize + pos.z * _GridSize * _GridSize;
			}


			v2f vp(VertexData v) {
				v2f i;

				// float3 pos = v.vertex.xyz;
				// pos += (_GridBounds / 2.0f);
				// pos /= _GridBounds;
				// pos *= _GridSize;

				// if (any(uint3(pos) > _GridSize) || any(pos < 0)) return;

				i.pos = UnityObjectToClipPos(v.vertex);
				// i.occlusion = _OcclusionGrid[to1D(pos)];
				i.occlusion = 0;
				i.worldPos = v.vertex.xyz;
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
				
				float3 pos = i.worldPos;
				pos += (_GridBounds / 2.0f);
				pos /= _GridBounds;
				pos *= _GridSize;
				float occlusion = _OcclusionGrid[to1D(pos)];

				return float4(col * occlusion, 1);
			}

			ENDCG
		}
	}
}