using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchRenderFeature : ScriptableRendererFeature
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    public ComputeAsset computeAsset;
    private RaymarchRenderPass _raymarchRenderPass;
    
    public override void Create() {
        if (computeAsset == null) return;
        _raymarchRenderPass = new RaymarchRenderPass(computeAsset, renderPassEvent);
    }
    
    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera (every frame!) 
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (computeAsset == null) return;
        
        // Skip rendering for inspector preview cameras
        if (renderingData.cameraData.isPreviewCamera) return;
        
        _raymarchRenderPass.ConfigureInput(ScriptableRenderPassInput.Depth);
        _raymarchRenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
        
        renderer.EnqueuePass(_raymarchRenderPass);
    }
    
    protected override void Dispose(bool disposing) {
        _raymarchRenderPass?.ReleaseTargets();
    }
}

[Serializable]
public class RaymarchSettings
{
    [Header("General")]
    public ComputeShader shader;
    public List<BaseShape> shapes;
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    public Light light;
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

    [Header("Debug")] 
    public string colourDestinationID = "";
    public string depthDestinationID = "";
}
