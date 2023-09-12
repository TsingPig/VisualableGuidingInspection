using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Highlighters;

namespace Highlighters_BuiltIn
{
    public class DepthMaskPass
    {
        private Material material;
        private List<Renderer> renderers;
        public RenderTexture renderTexture;
        public CommandBuffer cmd;

        public DepthMaskPass(List<Renderer> renderers, string eventPrefix) // ,Camera camera
        {
            this.renderers = renderers;

            material = new Material(Shader.Find("Highlighters_BuiltIn/SceneDepthShader"));

            cmd = new CommandBuffer();
            cmd.name = eventPrefix + "Scene Depth";
        }


        public void ConfigureRenderTexture(int width, int height, float scale)
        {
            if (renderTexture != null) RenderTexture.ReleaseTemporary(renderTexture);

            int lwidth = Mathf.FloorToInt(width * scale);
            int lheight = Mathf.FloorToInt(height * scale);

            renderTexture = RenderTexture.GetTemporary(lwidth, lheight, 16, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            renderTexture.filterMode = FilterMode.Point;
        }


        public void Execute()
        {
            cmd.Clear();

            cmd.SetRenderTarget(renderTexture);
            cmd.ClearRenderTarget(true, true, Color.black);

            foreach (var renderer in renderers)
            {
                for (int submeshIndex = 0; submeshIndex < renderer.sharedMaterials.Length; submeshIndex++)
                {
                    cmd.DrawRenderer(renderer, material, submeshIndex, 0);
                }
            }

        }
    }
}