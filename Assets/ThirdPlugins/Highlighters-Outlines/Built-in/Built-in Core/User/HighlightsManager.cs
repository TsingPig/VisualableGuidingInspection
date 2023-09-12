using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

using System.Collections.Generic;
using System;

using Highlighters;

namespace Highlighters_BuiltIn
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class HighlightsManager : MonoBehaviour
    {
        /// <summary>
        /// Contains all highlighters in the scene. 
        /// </summary>
        public Dictionary<int, Highlighter> highlightersInScene = new Dictionary<int, Highlighter>();

        private DepthMaskPass depthMaskPass;
        public bool useDepthMask = false;

        // Overlays
        private Dictionary<int, ObjectsPass> objectsPasses = new Dictionary<int, ObjectsPass>();
        private Dictionary<int, MeshOutlinePass> meshOutlinePasses = new Dictionary<int, MeshOutlinePass>();
        private Dictionary<int, OverlayPass> overlayPasses = new Dictionary<int, OverlayPass>();

        // Blurs
        private Dictionary<int, PerObjectBlurPass> perObjectBlurPasses = new Dictionary<int, PerObjectBlurPass>();

        // Renderers used to draw scene depth buffer
        private List<Renderer> depthRenderers = new List<Renderer>();

        // Public variables
        public LayerMask DepthLayerMask;
        [Range(0.05f, 1f)] public float DepthRenderScale = 1;
        public CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;

        private bool InSceneView = true;

        private Camera mainCamera;
        private Camera sceneCamera;
        private List<Camera> activeCameras = new List<Camera>();

        public static List<(CommandBuffer, CameraEvent)> commandBuffers;

        private string eventsPrefix = "Highlights: ";

        // Makes sure the manager won't try to update data while already doing it
        private bool isWorking = false;

        public static HighlightsManager instance { get; private set; }

        public void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple instances of HighlightsManager detected, destroying one.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Call this method when you add a new renderer to your scene during runtime
        /// that needs to be rendered to the depth mask.
        /// </summary>
        public void RefreshRenderers()
        {
            FindDepthRenderers();
        }

        private void OnEnable()
        {
            CheckIsInSceneView();

            HighlightersReset();

            Highlighter.OnHighlighterValidate += HighlighterDataUpdate;
            Highlighter.OnHighlighterReset += HighlightersReset;

#if UNITY_EDITOR
            EditorSceneManager.sceneSaved += SceneSaved;

            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update += EditorUpdate;
            }
#endif
        }

        private void OnDisable()
        {
            Highlighter.OnHighlighterValidate -= HighlighterDataUpdate;
            Highlighter.OnHighlighterReset -= HighlightersReset;

            RemoveAllCommandBuffers();

#if UNITY_EDITOR
            EditorSceneManager.sceneSaved -= SceneSaved;

            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= EditorUpdate;
            }
#endif
        }

#if UNITY_EDITOR
        private void SceneSaved(Scene scene)
        {
            CheckIsInSceneView();

            FindDepthRenderers();
            HighlightersReset();
        }


        private void EditorUpdate()
        {
            CheckIsInSceneView();


            if (InSceneView)
            {
                if (useDepthMask)
                {
                    depthMaskPass.Execute();
                }
                foreach (var item in objectsPasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in meshOutlinePasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in perObjectBlurPasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in overlayPasses)
                {
                    item.Value.Execute();
                }

            }
        }
#endif
        private void CheckIsInSceneView()
        {
            if (Application.isPlaying)
            {
                InSceneView = false;
            }
            else
            {
                InSceneView = true;
            }
        }

        private void Start()
        {
            InSceneView = false;

            ResetCommandBuffersList();

            FindDepthRenderers();
            HighlightersReset();
        }

        private void OnValidate()
        {
            //CheckIsInSceneView();

            RemoveAllCommandBuffers();
            //GetCameras();

            FindDepthRenderers();
            HighlightersReset();
        }

        /// <summary>
        /// Finds main camera and scene camera.
        /// </summary>
        private void GetCameras()
        {
#if UNITY_EDITOR
            foreach (var item in UnityEditor.SceneView.GetAllSceneCameras())
            {
                if (item.name == "SceneCamera")
                {
                    sceneCamera = item;
                }
            }
#endif

            mainCamera = GetComponent<Camera>();
        }

        /// <summary>
        /// Updates activeCameras list based on InSceneView value of property.
        /// </summary>
        private void UpdateCameras()
        {
            activeCameras.Clear();

            if (InSceneView && sceneCamera != null)
            {
                activeCameras.Add(sceneCamera);
            }
            else if (mainCamera != null)
            {
                activeCameras.Add(mainCamera);
            }
            else
            {
                Debug.LogError("There is no camera to perform highlights on!");
            }
        }

        /// <summary>
        /// Makes sure commandBuffers list isn't null;
        /// </summary>
        private void ResetCommandBuffersList()
        {
            if (commandBuffers == null)
            {
                commandBuffers = new List<(CommandBuffer, CameraEvent)>();
            }
        }

        /// <summary>
        /// Finds all depth renderers for depth buffer in scene.
        /// </summary>
        private void FindDepthRenderers()
        {
            depthRenderers.Clear();

            var renderersArray = FindObjectsOfType<Renderer>(false);

            foreach (var renderer in renderersArray)
            {
                if (DepthLayerMask == (DepthLayerMask | (1 << renderer.gameObject.layer)))
                {
                    depthRenderers.Add(renderer);
                }
            }
        }

        /// <summary>
        /// Adds command buffer to all cameras in activeCameras list.
        /// </summary>
        /// <param name="cmd">Command buffer to add.</param>
        private void AddCommandBuffer(CommandBuffer cmd)
        {
            foreach (var cam in activeCameras)
            {
                cam.AddCommandBuffer(cameraEvent, cmd);
            }
            commandBuffers.Add((cmd, cameraEvent));
        }

        /// <summary>
        /// Removes command buffer from all cameras in activeCameras list.
        /// </summary>
        /// <param name="cmd">Command buffer to remove.</param>
        private void RemoveCommandBuffer(CommandBuffer cmd)
        {
            foreach (var cam in activeCameras)
            {
                cam.RemoveCommandBuffer(cameraEvent, cmd);
            }
            commandBuffers.Remove((cmd, cameraEvent));
        }

        /// <summary>
        /// Removes all command buffers from all cameras in activeCameras list previously added by the manager.
        /// </summary>
        private void RemoveAllCommandBuffers()
        {
            if (activeCameras == null)
            {
                if (commandBuffers != null) commandBuffers.Clear();
                return;
            }

            foreach (var cam in activeCameras)
            {
                if (cam == null) continue;
                var cmds = cam.GetCommandBuffers(cameraEvent);

                foreach (var cmd in cmds)
                {
                    if (cmd.name.StartsWith(eventsPrefix))
                    {
                        cam.RemoveCommandBuffer(cameraEvent, cmd);
                    }
                }

                if (commandBuffers != null)
                {
                    foreach (var cmd in commandBuffers)
                    {
                        cam.RemoveCommandBuffer(cmd.Item2, cmd.Item1);
                    }
                }
            }

            if (commandBuffers != null) commandBuffers.Clear();
        }

        /// <summary>
        /// Updates data in higlights manager when settings of highlighter are changed. Currently only in editor. 
        /// </summary>
        /// <param name="ID"></param>
        private void HighlighterDataUpdate(int ID)
        {
            if (!highlightersInScene.ContainsKey(ID) || ID == -1 || ID > highlightersInScene.Count)
            {
                HighlightersReset();
                return;
            }

            ReleaseRenderTextures(ID);

            var highlighter = highlightersInScene[ID];

            var highlighterSettings = highlighter.Settings;
            var renderers = highlighter.Renderers;

            //if (activeCameras.Count == 0) GetCameras();
            //UpdateCameras();

            foreach (var cam in activeCameras)
            {
                if (overlayPasses.ContainsKey(ID)) RemoveCommandBuffer(overlayPasses[ID].cmd);
                if (meshOutlinePasses.ContainsKey(ID)) RemoveCommandBuffer(meshOutlinePasses[ID].cmd);
                RemoveCommandBuffer(objectsPasses[ID].cmd);
                if (perObjectBlurPasses.ContainsKey(ID)) RemoveCommandBuffer(perObjectBlurPasses[ID].cmd);

                //if (highlighter.Settings.DepthMask != DepthMask.Disable)
                //{
                //    renderSceneDepth = true;
                //}
                //else
                //{
                //    renderSceneDepth = false;
                //}

                RenderTexture depthRenderTexture = null;

                if (useDepthMask)
                {
                    depthRenderTexture = depthMaskPass.renderTexture;
                }

                objectsPasses[ID] = new ObjectsPass(highlighterSettings, ID, renderers, cam, depthRenderTexture, eventsPrefix);
                objectsPasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                AddCommandBuffer(objectsPasses[ID].cmd);

                if (highlighterSettings.UseMeshOutline)
                {
                    meshOutlinePasses[ID] = new MeshOutlinePass(highlighterSettings, ID, renderers, depthRenderTexture, eventsPrefix);
                    meshOutlinePasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                    AddCommandBuffer(meshOutlinePasses[ID].cmd);
                }

                if (highlighterSettings.UseOuterGlow)
                {
                    perObjectBlurPasses[ID] = new PerObjectBlurPass(highlighterSettings, ID, eventsPrefix);
                    perObjectBlurPasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                    perObjectBlurPasses[ID].SetupObjectsTarget(objectsPasses[ID].renderTexture);
                    AddCommandBuffer(perObjectBlurPasses[ID].cmd);
                }

                if (highlighterSettings.IsAnyOverlayUsed())
                {
                    overlayPasses[ID] = new OverlayPass(highlighterSettings, ID, eventsPrefix);
                    overlayPasses[ID].SetupObjectsTarget(objectsPasses[ID].renderTexture);
                    AddCommandBuffer(overlayPasses[ID].cmd);
                }

                if (highlighterSettings.UseMeshOutline)
                {
                    overlayPasses[ID].SetupMeshOutlineTarget(meshOutlinePasses[ID].renderTexture);
                }

            }
        }

        /// <summary>
        /// Fills highlightersInScene with highlighters from the scene and creates Id for each of the highlighters.
        /// </summary>
        private void GetAllSceneHighlighters()
        {
            highlightersInScene = new Dictionary<int, Highlighter>();

            var list = new List<Highlighter>(FindObjectsOfType<Highlighter>());

            foreach (var item in list)
            {
                item.Setup();
            }

            //renderSceneDepth = false;

            foreach (var item in list)
            {
                if (item.enabled)
                {
                    if (item.ID != -1 && !highlightersInScene.ContainsKey(item.ID))
                    {
                        highlightersInScene.Add(item.ID, item);
                        //if (item.Settings.DepthMask != DepthMask.Disable) renderSceneDepth = true;
                    }
                    else
                    {
                        // If Ids overlap, reset highlighters
                        item.Setup();
                        HighlightersReset();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Called when highlighters need a reset.
        /// </summary>
        public void HighlightersReset()
        {
            // Activation multiple highlighters at the same time makes multiple HighlightersReset() funcitons work at the same time which results in dictionaries not being cleared here.
            if (isWorking) return;
            isWorking = true;

            CheckIsInSceneView();
            GetCameras();
            UpdateCameras();

            ReleaseAllRenderTextures();

            foreach (var cam in activeCameras)
            {
                ResetCommandBuffersList();
                RemoveAllCommandBuffers();

                // This sets renderSceneDepth value
                GetAllSceneHighlighters();

                RenderTexture depthRenderTexture = null;

                if (useDepthMask)
                {
                    // Scene Depth Mask
                    depthMaskPass = new DepthMaskPass(depthRenderers, eventsPrefix);
                    depthMaskPass.ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight, DepthRenderScale);
                    AddCommandBuffer(depthMaskPass.cmd);
                    depthRenderTexture = depthMaskPass.renderTexture;
                }


                // Overlay
                objectsPasses.Clear();
                meshOutlinePasses.Clear();
                overlayPasses.Clear();

                // Blur
                perObjectBlurPasses.Clear();

                foreach (var item in highlightersInScene)
                {
                    var highlighter = item.Value;
                    var ID = highlighter.ID;
                    var highlighterSettings = highlighter.Settings;
                    var renderers = highlighter.Renderers;

                    objectsPasses.Add(ID, new ObjectsPass(highlighterSettings, ID, renderers, cam, depthRenderTexture, eventsPrefix));
                    objectsPasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                    AddCommandBuffer(objectsPasses[ID].cmd);

                    if (highlighterSettings.UseMeshOutline)
                    {
                        meshOutlinePasses.Add(ID, new MeshOutlinePass(highlighterSettings, ID, renderers, depthRenderTexture, eventsPrefix));
                        meshOutlinePasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                        AddCommandBuffer(meshOutlinePasses[ID].cmd);
                    }

                    if (highlighterSettings.UseOuterGlow)
                    {
                        perObjectBlurPasses.Add(ID, new PerObjectBlurPass(highlighterSettings, ID, eventsPrefix));
                        perObjectBlurPasses[ID].ConfigureRenderTexture(cam.pixelWidth, cam.pixelHeight);
                        perObjectBlurPasses[ID].SetupObjectsTarget(objectsPasses[ID].renderTexture);
                        AddCommandBuffer(perObjectBlurPasses[ID].cmd);
                    }

                    if (highlighterSettings.IsAnyOverlayUsed())
                    {
                        overlayPasses.Add(ID, new OverlayPass(highlighterSettings, ID, eventsPrefix));
                        overlayPasses[ID].SetupObjectsTarget(objectsPasses[ID].renderTexture);
                        AddCommandBuffer(overlayPasses[ID].cmd);
                    }

                    if (highlighterSettings.UseMeshOutline)
                    {
                        overlayPasses[ID].SetupMeshOutlineTarget(meshOutlinePasses[ID].renderTexture);
                    }
                }
            }

            isWorking = false;
        }

        /// <summary>
        /// Releases all temporary render textures.
        /// </summary>
        private void ReleaseAllRenderTextures()
        {
            if (depthMaskPass != null)
            {
                if (depthMaskPass.renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(depthMaskPass.renderTexture);
                }
            }

            foreach (var item in objectsPasses)
            {
                if (item.Value.renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(item.Value.renderTexture);
                }
            }

            foreach (var item in meshOutlinePasses)
            {
                if (item.Value.renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(item.Value.renderTexture);
                }
            }

            foreach (var item in perObjectBlurPasses)
            {
                if (item.Value.blurredObjects != null)
                {
                    RenderTexture.ReleaseTemporary(item.Value.blurredObjects);
                }

                if (item.Value.blurredObjectsBoth != null)
                {
                    RenderTexture.ReleaseTemporary(item.Value.blurredObjectsBoth);
                }
            }


        }

        /// <summary>
        /// Releases temporary render textures from specific highlghter.
        /// </summary>
        /// <param name="ID"> ID of the highlighter.</param>
        private void ReleaseRenderTextures(int ID)
        {
            if (objectsPasses.ContainsKey(ID))
            {
                if (objectsPasses[ID].renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(objectsPasses[ID].renderTexture);
                }
            }

            if (meshOutlinePasses.ContainsKey(ID))
            {
                if (meshOutlinePasses[ID].renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(meshOutlinePasses[ID].renderTexture);
                }
            }

            if (perObjectBlurPasses.ContainsKey(ID))
            {
                if (perObjectBlurPasses[ID].blurredObjects != null)
                {
                    RenderTexture.ReleaseTemporary(perObjectBlurPasses[ID].blurredObjects);
                }

                if (perObjectBlurPasses[ID].blurredObjectsBoth != null)
                {
                    RenderTexture.ReleaseTemporary(perObjectBlurPasses[ID].blurredObjectsBoth);
                }
            }

        }

        private void OnPreRender()
        {
            if (!InSceneView)
            {
                if (useDepthMask)
                {
                    depthMaskPass.Execute();
                }

                foreach (var item in objectsPasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in meshOutlinePasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in perObjectBlurPasses)
                {
                    item.Value.Execute();
                }

                foreach (var item in overlayPasses)
                {
                    item.Value.Execute();
                }

            }
        }

    }
}

