Shader "Custom/Star" 
{
	SubShader
	{
		// Ensure proper depth and alpha fade
		ZWrite Off
		ZTest Off
		Blend One One

		Tags { "Queue"="Background" }
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			// Matching star struct to that built in C#
			struct Star
			{
				float3 position;
				float magnitude;
				float4 hue;
			};

			struct v2f
			{
				float4 worldPos : SV_POSITION;
				float2 coord : TEXCOORD0;
				float magnitude : TEXCOORD1;
				float4 hue : TEXCOORD2;
			};

			// Compute buffer specified in C#
			StructuredBuffer<Star> starData;
			float starScale;
			float sharpness;
			float magnitudeScalar;

			v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
			{
				v2f o;

				// Retrieve the current star
				Star star = starData[instanceID];

				// Compute relative star position
				float4 worldPos = mul(UNITY_MATRIX_VP, float4(_WorldSpaceCameraPos.xyz + star.position, 1));
				worldPos += float4(v.vertex.x, -v.vertex.y * (_ScreenParams.x / _ScreenParams.y), 0, 0) * starScale * lerp(0.5, 1.5, star.magnitude);

				// Assign output parameters
				o.coord = v.vertex.xy;
				o.magnitude = star.magnitude;
				o.worldPos = worldPos;
				o.hue = star.hue;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// Create circular falloff
				float radialDist = saturate(1 - length(i.coord));
				float falloff = pow(min(1, radialDist * sharpness), 3);

				// Add in the stars hue colour and magnitude
				float3 finalColor = saturate(lerp(1, i.hue, 1 - falloff) * falloff) * i.magnitude * magnitudeScalar;
				return float4(finalColor, i.magnitude);
			}
			ENDCG
		}
	}
}