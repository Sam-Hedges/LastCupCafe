using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RaymarchRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private RaymarchSettings settings;
    [SerializeField] private ComputeShader shader;
    private RaymarchRenderPass raymarchRenderPass;

    public override void Create() {
        if (shader == null) {
            return;
        }
        settings.shapes = new List<BaseShape>(FindObjectsOfType<BaseShape>());
        raymarchRenderPass = new RaymarchRenderPass(shader, settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData) {
        
        if (renderingData.cameraData.cameraType == CameraType.Game) {
            renderer.EnqueuePass(raymarchRenderPass);
        }
    }
}

[Serializable]
public class RaymarchSettings
{
    [Header("General")]
    public ComputeShader raymarchingShader;
    public List<BaseShape> shapes;
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    public Light sunLight;
    public float maxDistance = 500;
    public int maxIterations = 512;

    [Header("Shadows")]
    public bool useSoftShadows = true;
    [Range(1, 128)] public float shadowPenumbra;
    [Range(0, 4)] public float shadowIntensity;
    public Vector2 shadowDistance = new Vector2(0.1f, 20);

    [Header("Ambient Occlusion")] 
    public bool aoEnabled = true;
    [Range(0.01f, 10f)] public float aoStepSize;
    [Range(1, 5)] public int aoIterations;
    [Range(0, 1)] public float aoIntensity;
}