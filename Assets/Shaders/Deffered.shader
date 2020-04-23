Shader "karonator/Deffered" {
	SubShader {
		Cull Off
		ZWrite Off
		ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _Tex0;
			uniform sampler2D _Tex1;

			uniform sampler2D _CameraDepthTexture;
			uniform float3 _LightsPositions[32];
			uniform float3 _LightsColors[32];

			uniform float4x4 clipToWorld;

			float3 screenToWorld(float2 uv) {
				float depth = tex2D(_CameraDepthTexture, uv).x;

				float4 clipSpacePosition = float4(uv * 2.0 - 1.0, depth, 1.0) * LinearEyeDepth(depth);
				float3 worldPosition = mul(clipToWorld, clipSpacePosition).xyz;
			   
				return worldPosition;
			}

			float4 frag(v2f_img i): COLOR {
				float4 color = tex2D(_Tex0, i.uv);
				float4 raw_normal = tex2D(_Tex1, i.uv);
				float3 pos_world = screenToWorld(i.uv);

				float3 N = normalize(2 * (raw_normal.xyz - 0.5));

				float3 result = float3(0, 0, 0);
				for (int i = 0; i < 32; ++i)
				{
					float3 L = normalize(_LightsPositions[i] - pos_world);
					float dist = distance(_LightsPositions[i], pos_world);

					float contribution = 1.0 / (pow(dist, 2.0) + 0.0001);
					result += max(dot(L, N) * contribution, 0) * normalize(_LightsColors[i]);
				}

				return float4(result * color.xyz, 1.0);
			}
			ENDCG
		}
	}
}