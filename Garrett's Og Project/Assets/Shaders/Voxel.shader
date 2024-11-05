Shader "Custom/Voxel" {

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
				float3 normal : TEXCOORD0;
                int voxel : TEXCOORD1;
				float occlusion : TEXCOORD2;
			};

			StructuredBuffer<int> _VoxelGrid;
			StructuredBuffer<float> _OcclusionGrid;
            int _GridSize, _GridBounds;
            float _VoxelSize;

            uint3 to3D(uint idx) {
                uint3 voxelRes = _GridSize;
                uint x = idx % (voxelRes.x);
                uint y = (idx / voxelRes.x) % voxelRes.y;
                uint z = idx / (voxelRes.x * voxelRes.y);

                return uint3(x, y, z);
            }

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
				InitIndirectDrawArgs(0);

				v2f i;
                
				uint instanceID = GetIndirectInstanceID(svInstanceID);

                uint x = instanceID % _GridSize;
                uint y = (instanceID / _GridSize) % _GridSize;
                uint z = instanceID / (_GridSize * _GridSize);
			
				int voxel = _VoxelGrid[instanceID];
                float3 voxelPos = float3(x, y, z);

				float4 pos = v.vertex;
                pos.xyz = (v.vertex.xyz + voxelPos) * _VoxelSize + (_VoxelSize * 0.5f) - _GridBounds * 0.5f;

				float occlusion = _OcclusionGrid[instanceID];

				i.pos = UnityObjectToClipPos(pos) * voxel;
				i.normal = UnityObjectToWorldNormal(v.normal);
                i.voxel = voxel;
				i.occlusion = occlusion;

				return i;
			}

			float4 fp(v2f i) : SV_TARGET {
				float3 col = 1;


                // if (i.voxel == 0) col = float3(1, 0, 0);
                // if (i.voxel == 1) col = float3(0, 1, 0);
                
				col *= saturate(DotClamped(_WorldSpaceLightPos0.xyz, i.normal) + 0.15f);

				col *= i.occlusion;
				return float4(saturate(col), 1);
			}

			ENDCG
		}
	}
}