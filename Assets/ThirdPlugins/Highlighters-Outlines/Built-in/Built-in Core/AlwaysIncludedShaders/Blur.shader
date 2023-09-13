Shader "Highlighters_BuiltIn/Blur"
{
	 CGINCLUDE
        #include "UnityCG.cginc"

		sampler2D _ObjectsInfo;
		float2 _ObjectsInfo_TexelSize;

		sampler2D _BlurredObjects;
		float2 _BlurredObjects_TexelSize;

		// General

		int _DepthMask;
		float4 BoundsMinMax;

		// Outer Glow
		#pragma shader_feature _BOXBLUR
		#pragma shader_feature _GAUSSIANBLUR

		float4 _ColorFront;
		float4 _ColorBack;
		int _UseSingleOuterGlow;
		int _OutlineVisibleBeforeObject;
		
		float _BlurIntensity;
		int _BlurIterations;

		// Gaussian Blur
		float _GaussSamples[50];

		// Box Blur
		float _BoxBlurSize;

		// Rendering bounds
		float4 _RenderingBounds;

		struct Attributes
		{
			float4 positionOS : POSITION;
			float2 uv : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		
		struct Varyings
		{
			float4 positionCS : SV_POSITION;
			float3 positionVS : TEXCOORD1;
			float2 uv : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
		};

		Varyings VertexSimple(Attributes input)
		{
			Varyings output = (Varyings)0;

			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            float4 positionCS = UnityObjectToClipPos(input.positionOS.xyz);
            output.positionCS = positionCS;

			output.positionVS = ComputeScreenPos(positionCS);;

			output.uv = input.uv;

			return output;
		}

		// Gaussian Blur

		float2 CalcIntensityN0(float2 uv, float2 offset, int k)
		{
			return tex2D(_ObjectsInfo, uv + k * offset).rg * _GaussSamples[k];
		}

		float2 CalcIntensityN1(float2 uv, float2 offset, int k)
		{
			return tex2D(_ObjectsInfo, uv - k * offset).rg * _GaussSamples[k];
		}

		float2 CalcIntensityGaussian(float2 uv, float2 offset)
		{
			float2 intensity = 0;

			[unroll(50)]
			for (int k = 1; k <= _BlurIterations; ++k)
			{
				intensity += CalcIntensityN0(uv, offset, k);
				intensity += CalcIntensityN1(uv, offset, k);
			}

			intensity += CalcIntensityN0(uv, offset, 0);
			return intensity;
		}

		float2 CalcIntensityN0Blurred(float2 uv, float2 offset, int k)
		{
			return tex2D(_BlurredObjects, uv + k * offset).rg * _GaussSamples[k];
		}

		float2 CalcIntensityN1Blurred(float2 uv, float2 offset, int k)
		{
			return tex2D(_BlurredObjects, uv - k * offset).rg * _GaussSamples[k];
		}

		float2 CalcIntensityBlurredGaussian(float2 uv, float2 offset)
		{
			float2 intensity = 0;

			[unroll(50)]
			for (int k = 1; k <= _BlurIterations; ++k)
			{
				intensity += CalcIntensityN0Blurred(uv, offset, k);
				intensity += CalcIntensityN1Blurred(uv, offset, k);
			}

			intensity += CalcIntensityN0Blurred(uv, offset, 0);
			return intensity;
		}

		// ---------------

		// Box Blur

		float2 CalcIntensityBox(float2 uv)
		{
			float2 intensity = 0;

			[unroll(50)]
			for(float index=0;index<_BlurIterations;index++){
				float2 uvBlur = uv + float2(0, (index/(_BlurIterations-1) - 0.5) * _BoxBlurSize);
				intensity += tex2D(_ObjectsInfo,uvBlur).rg;
			}
			intensity = intensity / 10;
			return intensity;
		}

		float2 CalcIntensityBlurredBox(float2 uv)
		{
			float invAspect = _ScreenParams.y / _ScreenParams.x;
			
			float2 intensity = 0;

			[unroll(50)]
			for(float index = 0; index < _BlurIterations; index++){
				float2 uvBlur = uv + float2((index/(_BlurIterations-1) - 0.5) * _BoxBlurSize * invAspect, 0);
				intensity += tex2D(_BlurredObjects,uvBlur).rg;
			}
			intensity = intensity / 10;
			return intensity;
		}

		// ---------------
		// Blur Main Functions

		float2 ObjectsIntesity(float2 uv)
		{
			float2 intensity = float2(0,0);

			#ifdef _BOXBLUR
			intensity = CalcIntensityBox(uv);

			#elif _GAUSSIANBLUR
			intensity =  CalcIntensityGaussian(uv, float2(_BlurredObjects_TexelSize.x,0));
			#endif

			return intensity;

		}

		float2 BlurredIntesity(float2 uv)
		{
			float2 intensity = float2(0,0);

			#ifdef _BOXBLUR
			intensity = CalcIntensityBlurredBox(uv);

			#elif _GAUSSIANBLUR
			intensity =  CalcIntensityBlurredGaussian(uv, float2(0, _BlurredObjects_TexelSize.y));
			#endif

			intensity  *= _BlurIntensity;
			intensity = saturate(intensity);
			return intensity;

		}

		// ---------------

		bool ShouldRender(float3 positionVS)
		{
			//return true;

			if(_RenderingBounds.x < positionVS.x && _RenderingBounds.y < positionVS.y && _RenderingBounds.z > positionVS.x && _RenderingBounds.w > positionVS.y)
			{
				return true;
			}

			return false;
		}

		// ---------------

		float2 FragmentH(Varyings i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			if(!ShouldRender(i.positionVS)) return float2(0,0);

			float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
			
			//return ObjectsIntesity(uv);
			return ObjectsIntesity(uv);
			
		}

		float4 FragmentVAlpha(Varyings i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			if(!ShouldRender(i.positionVS)) return float4(0,0,0,0);

			float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
			float2 intensity = BlurredIntesity(uv);

			float4 mask = tex2D(_ObjectsInfo, uv);

			//_DepthMask -- Behind | Front | Both | Disable 

            if(_DepthMask == 3) // No Depth Mask
			{
				if(mask.r > 0) 
				{
					return float4(0,0,0,0);
				}				
				else return float4(_ColorFront.rgb, _ColorFront.a* saturate(intensity.g + intensity.r));
			}
			
			else if(_DepthMask == 0) // Behind Draw
			{
				if(!_OutlineVisibleBeforeObject)
				{
					if (mask.r > 0 || mask.g > 0 )
					{
						return float4(0,0,0,0);
					}
				}
				if (mask.r > 0)
				{
					return float4(0,0,0,0);
				}
				return float4(_ColorFront.rgb, _ColorFront.a* (intensity.r));
			}
			
			else if(_DepthMask == 1) // Front Draw
			{
				if(!_OutlineVisibleBeforeObject)
				{
					if (mask.r > 0 || mask.g > 0 )
					{
						return float4(0,0,0,0);
					}
				}

				if (mask.g > 0)
				{
					return float4(0,0,0,0);
				}
				return float4(_ColorFront.rgb, _ColorFront.a* (intensity.g));
			}

			else if(_DepthMask == 2) // Both Draw
			{
				if (mask.r > 0 || mask.g > 0)
				{
					return float4(0,0,0,0);
				}

				// This will only be used for _UseSingleOuterGlow = true | see FragmentVAlphaBoth

				//if(_UseSingleOuterGlow)
				{
					return float4(_ColorFront.rgb, _ColorFront.a* saturate(intensity.g + intensity.r));
				}
			}
           
			return float4(0,0,0,0);
		}

		float2 FragmentVAlphaBoth(Varyings i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			if(!ShouldRender(i.positionVS)) return float2(0,0);

			float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
			float2 intensity = BlurredIntesity(uv);

			return intensity;
		}

		float4 FragmentVAdditive(Varyings i) : SV_Target
		{
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			if(!ShouldRender(i.positionVS)) return float4(0,0,0,0);

			float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
			float2 intensity = BlurredIntesity(uv);
			float4 mask = tex2D(_ObjectsInfo, uv);

			// weird debug stuff
			//float2 v = ObjectsIntesity(uv);
			//return mask;
			//float2 v = SAMPLE_TEXTURE2D(_BlurredObjects, sampler_BlurredObjects,uv);
			//return float4(v.r,v.g,0,1);

			//_DepthMask -- Behind | Front | Both | Disable 

            if(_DepthMask == 3) // No Depth Mask
			{
				if(mask.r > 0) 
				{
					return float4(0,0,0,0);
				}				
				return _ColorFront * (intensity.g + intensity.r);
			}
			
			else if(_DepthMask == 0) // Behind Draw
			{
				if(!_OutlineVisibleBeforeObject)
				{
					if (mask.r > 0 || mask.g > 0 )
					{
						return float4(0,0,0,0);
					}
				}
				if (mask.r > 0)
				{
					return float4(0,0,0,0);
				}
				return _ColorFront * intensity.r;
			}
			
			else if(_DepthMask == 1) // Front Draw
			{
				if(!_OutlineVisibleBeforeObject)
				{
					if (mask.r > 0 || mask.g > 0 )
					{
						return float4(0,0,0,0);
					}
				}

				if (mask.g > 0)
				{
					return float4(0,0,0,0);
				}
				return _ColorFront * intensity.g;
			}

			else if(_DepthMask == 2) // Both Draw
			{
				if (mask.r > 0 || mask.g > 0)
				{
					return float4(0,0,0,0);
				}
				if(_UseSingleOuterGlow)
				{
					return _ColorFront * (intensity.g + intensity.r);
				}
				return _ColorFront * intensity.g + _ColorBack * intensity.r;
			}
           
			return float4(0,0,0,0);
		}

	ENDCG

	SubShader
	{
		ZWrite Off
		Lighting Off


		// Pass 0
		Pass
		{
			Name "HPass"

			CGPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragmentH

            #include "UnityCG.cginc"
            ENDCG
		}

		// Pass 1
		Pass
		{
			Name "VPassAlpha"
			Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency

			CGPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragmentVAlpha

            #include "UnityCG.cginc"
            ENDCG

		}

		// Pass 2
		Pass
		{
			Name "VPassAdditive"
			Blend One One // Additive

			CGPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragmentVAdditive

            #include "UnityCG.cginc"
            ENDCG

		}

		// Pass 3
		Pass
		{
			Name "VPassAlphaBoth"

			//Blend One One // Additive
			//Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragmentVAlphaBoth

            #include "UnityCG.cginc"
            ENDCG
		}

		// this is not used for anything currently
		// Pass 4
		Pass
		{
			Name "VPassCombined"

			Blend One One // Additive

			CGPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragmentVAdditive

            #include "UnityCG.cginc"
            ENDCG
		}
	}

}