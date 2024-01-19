using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public class PixelatePass : ScriptableRenderPass
{
    private PixelateFeature.PixelateSettings settings;
    private RTHandle colorBuffer, pixelBuffer;
    private Material material;
    private int pixelScreenHeight, pixelScreenWidth;

    public PixelatePass(PixelateFeature.PixelateSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        if (material == null) material = CoreUtils.CreateEngineMaterial("Hidden/Pixelate");
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        pixelScreenHeight = settings.screenHeight;
        pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);
        material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));
        descriptor.height = pixelScreenHeight;
        descriptor.width = pixelScreenWidth;
        descriptor.depthBufferBits = 0;  // Separate depth handling
        RenderingUtils.ReAllocateIfNeeded(ref pixelBuffer, descriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelBuffer");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(name: "Pixelize Pass");
        Blit(cmd, colorBuffer, pixelBuffer, material);
        Blit(cmd, pixelBuffer, colorBuffer);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        if (pixelBuffer != null) pixelBuffer.Release();
    }
}