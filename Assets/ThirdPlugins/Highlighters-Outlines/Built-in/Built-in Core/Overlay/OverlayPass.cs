using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Highlighters;

namespace Highlighters_BuiltIn
{
    public class OverlayPass
    {
        public CommandBuffer cmd;
        private readonly Material material;
        private RenderTexture objectsInfo;
        private RenderTexture meshOutlineObjects;

        public OverlayPass(HighlighterSettings highlighterSettings, int ID, string eventPrefix) // , Camera camera
        {
            material = new Material(Shader.Find("Highlighters_BuiltIn/Overlay"));
            //material = CoreUtils.CreateEngineMaterial()
            highlighterSettings.SetOverlayMaterialProperties(material);

            cmd = new CommandBuffer();
            cmd.name = eventPrefix + "Overlay_" + ID;
        }

        public void SetupObjectsTarget(RenderTexture objectsInfo)
        {
            this.objectsInfo = objectsInfo;
        }

        public void SetupMeshOutlineTarget(RenderTexture meshOutlineObjects)
        {
            this.meshOutlineObjects = meshOutlineObjects;
        }

        public void Execute()
        {
            cmd.Clear();
            cmd.BeginSample(cmd.name);

            cmd.SetGlobalTexture("_ObjectsInfo", objectsInfo);
            cmd.SetGlobalTexture("_MeshOutlineObjects", meshOutlineObjects);

            var screenImage = Shader.PropertyToID("_ScreenImage");
            cmd.GetTemporaryRT(screenImage, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

            cmd.SetRenderTarget(screenImage);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, screenImage);

            cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            cmd.Blit(screenImage, BuiltinRenderTextureType.CameraTarget, material);

            cmd.ReleaseTemporaryRT(screenImage);
            cmd.EndSample(cmd.name);
        }
    }
}
