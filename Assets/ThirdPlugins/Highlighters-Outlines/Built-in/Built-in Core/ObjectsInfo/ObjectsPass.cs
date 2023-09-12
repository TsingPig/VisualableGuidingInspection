using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Highlighters;

namespace Highlighters_BuiltIn
{
    public class ObjectsPass
    {
        public CommandBuffer cmd;
        public RenderTexture renderTexture;
        public RenderTexture depthRenderTexture;

        private List<HighlighterRenderer> renderersToDraw;
        private List<Material> materialsToDraw;
        private List<int> materialsPassIndexes;
        private HighlighterSettings highlighterSettings;
        private Camera camera;

        /// <summary>
        /// Vector4(MinX, MinY, MaxX, MaxY) in viewport space
        /// </summary>
        private Vector4 renderingBounds;

        public ObjectsPass(HighlighterSettings highlighterSettings, int ID, List<HighlighterRenderer> renderers, Camera camera, RenderTexture depthRenderTexture, string eventPrefix)
        {
            this.highlighterSettings = highlighterSettings;
            this.renderersToDraw = renderers;
            this.camera = camera;
            this.depthRenderTexture = depthRenderTexture;

            cmd = new CommandBuffer();
            cmd.name = eventPrefix + "Objects_Info_" + ID;

            UpdateMaterialsToDraw();
        }
        private void UpdateMaterialsToDraw()
        {
            materialsToDraw = new List<Material>();
            materialsPassIndexes = new List<int>();

            bool useDepth = true;
            if (highlighterSettings.DepthMask == DepthMask.Disable) useDepth = false;

            foreach (var item in renderersToDraw)
            {
                if (item.useCutout)
                {
                    var materialCutout = new Material(Shader.Find("Highlighters_BuiltIn/ObjectsInfo"));
                    materialCutout.SetTexture("_MainTex", item.GetClipTexture());
                    materialCutout.SetFloat("_Cutoff", item.clippingThreshold);
                    materialCutout.SetInt("useDepth", useDepth ? 1 : 0);
                    materialsToDraw.Add(materialCutout);
                    materialsPassIndexes.Add(((int)item.cullMode));

                }
                else
                {
                    var material = new Material(Shader.Find("Highlighters_BuiltIn/ObjectsInfo"));
                    material.SetInt("useDepth", useDepth ? 1 : 0);
                    materialsToDraw.Add(material);
                    //materialsPassIndexes.Add(((int)item.cullMode));
                    materialsPassIndexes.Add(((int)item.cullMode));
                }
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
            cmd.ClearRenderTarget(true, true, Color.black);
            if (depthRenderTexture != null) cmd.SetGlobalTexture("_SceneDepthMask", depthRenderTexture);

            if (renderersToDraw.Count == materialsToDraw.Count)
            {
                renderingBounds = new Vector4(10, 10, -10, -10);
                Vector3 rendererCenter = Vector3.zero;

                for (int i = 0; i < renderersToDraw.Count; i++)
                {
                    var item = renderersToDraw[i];
                    if (item.renderer == null || item.renderer.enabled == false) continue;

                    for (int submeshIndex = 0; submeshIndex < item.submeshIndexes.Count; submeshIndex++)
                    {
                        cmd.DrawRenderer(item.renderer, materialsToDraw[i], item.submeshIndexes[submeshIndex], materialsPassIndexes[i]);
                    }

                    var bounds = item.renderer.bounds;
                    var center = bounds.center;
                    var extents = bounds.extents;
                    rendererCenter = center;

                    if (highlighterSettings.RenderingBoundsDistanceFix)
                    {
                        if (RenderingBounds.CloseEnughToRenderFullScreen(camera, center, highlighterSettings.RenderingBoundsMaxDistanceFix, highlighterSettings.RenderingBoundsMinDistanceFix))
                        {
                            renderingBounds = new Vector4(0, 0, 1, 1);
                            highlighterSettings.SetRenderBoundsValues(renderingBounds);
                            continue;
                        }
                    }

                    renderingBounds = RenderingBounds.CalculateBounds(camera, extents, center, renderingBounds, highlighterSettings.RenderingBoundsSizeIncrease);

                }

                float cameraObjectDist = (rendererCenter - camera.transform.position).magnitude;
                highlighterSettings.CameraObjectDistace = cameraObjectDist;

                //if (useRenderBounds)
                {
                    highlighterSettings.SetRenderBoundsValues(renderingBounds);
                }
                //else
                {
                    //renderingBounds = new Vector4(-10000, -10000, 10000, 10000);
                    //highlighterSettings.SetRenderBoundsValues(renderingBounds);
                }
            }
            else
            {
                UpdateMaterialsToDraw();
            }

            //Graphics.ExecuteCommandBuffer(cmd);
            cmd.EndSample(cmd.name);

        }
    }
}
