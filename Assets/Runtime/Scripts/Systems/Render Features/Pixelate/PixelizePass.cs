using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizePass : ScriptableRenderPass
{
    private PixelizeFeature.CustomPassSettings settings;
    private static readonly int HalfBlockSize = Shader.PropertyToID("_HalfBlockSize");
    private static readonly int BlockSize = Shader.PropertyToID("_BlockSize");
    private static readonly int BlockCount = Shader.PropertyToID("_BlockCount");

    private RenderTargetIdentifier colourBuffer, pixelBuffer;
    private RTHandle colourBufferHandle, pixelBufferHandle;
    
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    private Material material;
    private int pixelScreenHeight, pixelScreenWidth;

    public PixelizePass(PixelizeFeature.CustomPassSettings settings) {
        this.settings = settings;
        renderPassEvent = settings.renderPassEvent;
        if (material == null) material = CoreUtils.CreateEngineMaterial("Hidden/Pixelate");
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        colourBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        
        pixelScreenHeight = settings.screenHeight;
        pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);
        
        colourBufferHandle = RTHandles.Alloc(colourBuffer); 
        pixelBufferHandle = RTHandles.Alloc(pixelScreenWidth, pixelScreenHeight, filterMode: FilterMode.Point);

        material.SetVector(BlockCount, new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector(BlockSize, new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector(HalfBlockSize, new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));
        
        descriptor.height = pixelScreenHeight;
        descriptor.width = pixelScreenWidth;
        
        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        CommandBuffer cmd = CommandBufferPool.Get("Pixelize Pass");

        // Set up and draw full-screen mesh
        cmd.SetRenderTarget(pixelBufferHandle);
        cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, -1);
        cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    public override void OnCameraCleanup(CommandBuffer cmd) {
        if (cmd == null) throw new ArgumentNullException(nameof(cmd));

        // Release RTHandles
        colourBufferHandle.Release();
        pixelBufferHandle.Release();
    }
}