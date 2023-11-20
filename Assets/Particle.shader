Shader "Custom/Particle" {

	Properties {
		_Albedo ("Albedo", Color) = (1, 1, 1, 1)
	}

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

			float4 _Albedo;

			struct VertexData {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			v2f vp(VertexData v, uint svInstanceID : SV_INSTANCEID) {
                InitIndirectDrawArgs(0);

				v2f i;
				
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                
                float4 pos = v.vertex;

                pos.x += instanceID;

                i.pos = UnityObjectToClipPos(pos);
				i.worldPos = mul(unity_ObjectToWorld, pos);

				return i;
			}

			float4 fp(v2f i) : SV_TARGET {
                float3 col = _Albedo.rgb;

                return float4(1, 0, 0, 1);
			}

			ENDCG
		}
	}
}