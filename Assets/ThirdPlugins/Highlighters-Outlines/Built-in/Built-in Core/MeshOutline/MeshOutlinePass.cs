using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Highlighters;

namespace Highlighters_BuiltIn
{
    public class MeshOutlinePass
    {
        public CommandBuffer cmd;
        public RenderTexture renderTexture;
        private RenderTexture depthRenderTexture;

        private List<HighlighterRenderer> renderersToDraw;
        private List<Material> materialsToDraw;
        private HighlighterSettings highlighterSettings;


        public MeshOutlinePass(HighlighterSettings highlighterSettings, int ID, List<HighlighterRenderer> renderers, RenderTexture depthRenderTexture, string eventPrefix) // Camera camera,
        {
            this.highlighterSettings = highlighterSettings;
            this.renderersToDraw = renderers;
            this.depthRenderTexture = depthRenderTexture;

            cmd = new CommandBuffer();
            cmd.name = eventPrefix + "Mesh_Outline_" + ID;

            renderersToDraw = renderers;
            UpdateMaterialsToDraw();

        }


        private void UpdateMaterialsToDraw()
        {
            materialsToDraw = new List<Material>();

            for (int i = 0; i < renderersToDraw.Count; i++)
            {
                var material = new Material(Shader.Find("Highlighters_BuiltIn/MeshOutlineObjects"));
                highlighterSettings.SetMeshOutlineMaterialProperties(material);
                materialsToDraw.Add(material);
            }
        }

        public void ConfigureRenderTexture(int width, int height)
        {
            if (renderTexture != null) RenderTexture.ReleaseTemporary(renderTexture);

            int lwidth = Mathf.FloorToInt(width * highlighterSettings.InfoRenderScale);
            int lheight = Mathf.FloorToInt(height * highlighterSettings.InfoRenderScale);

            renderTexture = RenderTexture.GetTemporary(lwidth, lheight, 16, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.filterMode = FilterMode.Point;
        }

        public void Execute()
        {
            cmd.Clear();
            cmd.BeginSample(cmd.name);

            cmd.SetRenderTarget(renderTexture);
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            if (depthRenderTexture != null) cmd.SetGlobalTexture("_SceneDepthMask", depthRenderTexture);

            if (renderersToDraw.Count == materialsToDraw.Count)
            {
                for (int i = 0; i < renderersToDraw.Count; i++)
                {
                    var item = renderersToDraw[i];

                    if (item.renderer == null || item.renderer.enabled == false) continue;

                    for (int submeshIndex = 0; submeshIndex < item.submeshIndexes.Count; submeshIndex++)
                    {
                        int ShaderPass;
                        if (highlighterSettings.DepthMask == DepthMask.BehindOnly) ShaderPass = 1;
                        else if (highlighterSettings.DepthMask == DepthMask.FrontOnly) ShaderPass = 0;
                        else ShaderPass = 2;

                        cmd.DrawRenderer(item.renderer, materialsToDraw[i], item.submeshIndexes[submeshIndex], ShaderPass);
                    }
                }
            }
            else
            {
                UpdateMaterialsToDraw();
            }

            cmd.EndSample(cmd.name);
        }
    }
}
