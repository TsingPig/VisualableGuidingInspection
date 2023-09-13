using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

using Highlighters;

namespace Highlighters_BuiltIn
{
    public class PerObjectBlurPass
    {
        public CommandBuffer cmd;
        private RenderTexture objectsInfo;

        private readonly Material blurMaterial;
        private readonly Material alphaBlitMaterial;

        public RenderTexture blurredObjects;
        public RenderTexture blurredObjectsBoth;

        private HighlighterSettings highlighterSettings;

        #region GaussianBlur

        private int MaxWidth = 50;
        private float[] gaussSamples;

        private float[] GetGaussSamples(int width, float[] samples)
        {
            var stdDev = width * 0.5f;

            if (samples is null)
            {
                samples = new float[MaxWidth];
            }

            for (var i = 0; i < width; i++)
            {
                samples[i] = Gauss(i, stdDev);
            }

            return samples;
        }
        private float Gauss(float x, float stdDev)
        {
            var stdDev2 = stdDev * stdDev * 2;
            var a = 1 / Mathf.Sqrt(Mathf.PI * stdDev2);
            var gauss = a * Mathf.Pow((float)Math.E, -x * x / stdDev2);

            return gauss;
        }
        #endregion

        public PerObjectBlurPass(HighlighterSettings blurOutlineSettings, int ID, string eventPrefix)
        {
            this.highlighterSettings = blurOutlineSettings;

            blurMaterial = new Material(Shader.Find("Highlighters_BuiltIn/Blur"));
            blurOutlineSettings.SetBlurMaterialProperties(blurMaterial);
            blurMaterial.EnableKeyword("_Variation_" + ID.ToString());

            gaussSamples = GetGaussSamples(50, gaussSamples);
            blurMaterial.SetFloatArray("_GaussSamples", gaussSamples);

            alphaBlitMaterial = new Material(Shader.Find("Highlighters_BuiltIn/AlphaBlit"));
            alphaBlitMaterial.EnableKeyword("_Variation_" + ID.ToString());
            blurOutlineSettings.SetAlphaBlitMaterialProperties(alphaBlitMaterial);


            cmd = new CommandBuffer();
            cmd.name = eventPrefix + "Per_Object_Blur_" + ID;
        }

        public void ConfigureRenderTexture(int width, int height)
        {
            if (blurredObjects != null) RenderTexture.ReleaseTemporary(blurredObjects);
            if (blurredObjectsBoth != null) RenderTexture.ReleaseTemporary(blurredObjectsBoth);

            int lwidth = Mathf.FloorToInt(width * highlighterSettings.BlurRenderScale);
            int lheight = Mathf.FloorToInt(height * highlighterSettings.BlurRenderScale);

            blurredObjects = RenderTexture.GetTemporary(lwidth, lheight, 16, RenderTextureFormat.RG16, RenderTextureReadWrite.Linear);
            blurredObjects.filterMode = FilterMode.Point;
            blurredObjectsBoth = RenderTexture.GetTemporary(lwidth, lheight, 16, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
            blurredObjectsBoth.filterMode = FilterMode.Point;
        }

        public void SetupObjectsTarget(RenderTexture objectsInfo)
        {
            this.objectsInfo = objectsInfo;
        }

        public void Execute()
        {
            cmd.Clear();
            cmd.BeginSample(cmd.name);

            cmd.SetGlobalTexture("_ObjectsInfo", objectsInfo);
            cmd.SetGlobalTexture("_BlurredObjects", blurredObjects);
            cmd.SetGlobalTexture("_BlurredObjectsBoth", blurredObjectsBoth);

            var screenImage = Shader.PropertyToID("_ScreenImage");
            cmd.GetTemporaryRT(screenImage, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);

            cmd.SetRenderTarget(screenImage);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, screenImage);

            // Horizontal blur
            cmd.SetRenderTarget(blurredObjects);
            cmd.Blit(screenImage, blurredObjects, blurMaterial, 0);

            if (highlighterSettings.BlendingType == HighlighterSettings.BlendType.Alpha)
            {
                // Use additional buffer for right alpha blending
                if (highlighterSettings.DepthMask == DepthMask.Both && !highlighterSettings.UseSingleOuterGlow)
                {
                    // Use VPassAlphaBoth shader Pass
                    cmd.SetRenderTarget(blurredObjectsBoth);
                    cmd.Blit(blurredObjects, blurredObjectsBoth, blurMaterial, 3);

                    cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    cmd.Blit(blurredObjectsBoth, BuiltinRenderTextureType.CameraTarget, alphaBlitMaterial, 0); // Front Blit
                    cmd.Blit(blurredObjectsBoth, BuiltinRenderTextureType.CameraTarget, alphaBlitMaterial, 1); // Back Blit
                }
                else
                {
                    cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    cmd.Blit(blurredObjects, BuiltinRenderTextureType.CameraTarget, blurMaterial, 1);
                }
            }
            else // Don't do anything fancy when BlendType is additive 
            {
                cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                cmd.Blit(blurredObjects, BuiltinRenderTextureType.CameraTarget, blurMaterial, 2);
            }

            cmd.ReleaseTemporaryRT(screenImage);
            cmd.EndSample(cmd.name);
        }
    }
}