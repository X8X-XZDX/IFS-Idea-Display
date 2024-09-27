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
			};


			v2f vp(VertexData v) {
				v2f i;

				i.pos = UnityObjectToClipPos(v.vertex);
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
				// col = 1;
				return float4(col, 1);
			}

			ENDCG
		}
	}
}