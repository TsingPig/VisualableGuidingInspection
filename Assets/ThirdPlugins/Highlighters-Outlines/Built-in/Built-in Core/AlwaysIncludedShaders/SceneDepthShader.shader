Shader "Highlighters_BuiltIn/SceneDepthShader"
{
    Properties
    {
    }
    SubShader {
        Pass {
            Name "Scene depth rendering"

            Cull Back
		    ZWrite On
		    ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 position     : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        struct Varyings
        {
            float4 positionCS   : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float4 positionCS = UnityObjectToClipPos(input.position.xyz);

                output.positionCS = positionCS;

                return output;
            }

            float frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return input.positionCS.z; 
            }
            ENDCG

        }
    }
    Fallback Off
}