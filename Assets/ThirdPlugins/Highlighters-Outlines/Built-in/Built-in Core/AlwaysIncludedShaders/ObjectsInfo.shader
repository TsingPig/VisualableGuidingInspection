Shader "Highlighters_BuiltIn/ObjectsInfo"
{

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _SceneDepthMask;
        sampler2D _MainTex;
        float _Cutoff;
        int useDepth ;

		struct Attributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
                float3 normal        : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        struct Varyings
        {
            float4 positionCS   : SV_POSITION;
            float2 uv           : TEXCOORD0;
            float4 screenPos    : TEXCOORD1;
            float3 worldPos     : TEXCOORD2;
            float3 normal        : NORMAL;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = input.texcoord;

                float4 positionCS = UnityObjectToClipPos(input.position.xyz);

                output.positionCS = positionCS;
                output.screenPos = ComputeScreenPos(positionCS);

                float3 positionWS = mul (unity_ObjectToWorld, input.position).xyz;
                //float3 positionWS = UnityObjectToWorld( input.position.xyz );
				output.worldPos = positionWS;

                float3 ase_worldNormal = UnityObjectToWorldNormal(input.normal);
                output.normal = ase_worldNormal;

                return output;
            }

		float4 frag(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

			    //Behind R channel
			    //Front G channel

                // TODO add something to alpha channel, thnik about depth (clip space position) to use in ordering 
                // or just delete the alpha channel

                float3 WorldPosition = input.worldPos;
                float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = input.normal.xyz;

                float Power = 1.4f;
                float rimBeforePower = 1.0 - saturate(dot(ase_worldNormal, ase_worldViewDir));

                float textureAlpha =  tex2D(_MainTex, input.uv).a;
                
                clip(textureAlpha - _Cutoff);
                
                if(useDepth)
                {
                    float2 uvs = float2(input.screenPos.xy / input.screenPos.w);

                    //#if UNITY_UV_STARTS_AT_TOP
                    //uvs.y = 1 - uvs.y;
                    //#endif

                    float4 mask = tex2D(_SceneDepthMask, input.screenPos.xy / input.screenPos.w);

                    #ifdef UNITY_REVERSED_Z 
				    if (mask.r > input.positionCS.z)
			            {
			                return float4(1,0,rimBeforePower,1); // Back
			            }
                    #else
				    if (mask.r <= input.positionCS.z)
			            {
			                return float4(1,0,rimBeforePower,1); // Back
			            }
                    #endif
                    
                    return float4(0,1,rimBeforePower,1); // Front

                }
			    return float4(1,0,rimBeforePower,1); // Back
                
             }

      ENDCG
	SubShader
	{
		ZWrite On
		ZTest LEqual
		Lighting Off

		Pass
		{
			Name "Off"
		    Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            ENDCG
		}

        Pass
		{
			Name "Front"
		    Cull Front


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            ENDCG
		}

        Pass
		{
			Name "Back"
		    Cull Back

	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            ENDCG
		}
	}
}
