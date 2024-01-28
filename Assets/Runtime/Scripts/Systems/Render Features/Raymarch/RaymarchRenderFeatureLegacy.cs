using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class RaymarchRenderFeatureLegacy : ScriptableRendererFeature
{
    [SerializeField] private RaymarchSettings settings;
    private RaymarchRenderPassLegacy _raymarchRenderPassLegacy;

    public override void Create()
    {
        if (settings.shader == null)
        {
            return;
        }

        settings.shapes = new List<BaseShape>(FindObjectsOfType<BaseShape>());
        settings.light = RenderSettings.sun;
        _raymarchRenderPassLegacy = new RaymarchRenderPassLegacy(settings);

        // Set the render pass event
        _raymarchRenderPassLegacy.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_raymarchRenderPassLegacy == null || settings == null)
        {
            return;
        }

        _raymarchRenderPassLegacy.Setup(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        renderer.EnqueuePass(_raymarchRenderPassLegacy);
    }
}

