using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchRenderPass : ScriptableRenderPass {
    
    private readonly ComputeAsset _computeAsset;
    private readonly ProfilingSampler _profilingSampler;

    private RTHandle _colorBufferTarget;
    private RTHandle _depthBufferTarget;
    private RTHandle _raymarchBufferTarget;
    
    private Camera _camera;
    private List<ComputeBuffer> _computeBuffers;
    
    private string _colourDestinationID;
    private string _depthDestinationID;
    private Material _addMaterial;

    public RaymarchRenderPass(ComputeAsset computeAsset, RenderPassEvent renderPassEvent) {
        _computeAsset = computeAsset;
        _profilingSampler = new ProfilingSampler(nameof(RaymarchRenderPass));
        this.renderPassEvent = renderPassEvent;
    }

    // Called before executing the render pass.
    // Used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        base.OnCameraSetup(cmd, ref renderingData);
        if (_computeAsset == null) return;
        
        _computeAsset.Setup(renderingData);
        
        _camera = renderingData.cameraData.camera;
        
        // Setup _raymarchBufferTarget as a temporary render target
        RenderingUtils.ReAllocateIfNeeded(ref _raymarchBufferTarget, renderingData.cameraData.cameraTargetDescriptor,
            name: "_raymarchBufferTarget");
        
        var desc = renderingData.cameraData.cameraTargetDescriptor;
        
        desc.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref _colorBufferTarget, desc,
                name: "_colorBufferTarget");
        _colorBufferTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        // 32 is the magic number 
        desc.depthBufferBits = 32; 
        RenderingUtils.ReAllocateIfNeeded(ref _depthBufferTarget, desc,
                name: "_depthBufferTarget");
        _depthBufferTarget = renderingData.cameraData.renderer.cameraDepthTargetHandle;

        ConfigureTarget(_colorBufferTarget, _depthBufferTarget);

        ConfigureClear(ClearFlag.All, Color.black);
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        if (_computeAsset == null || _computeAsset.shader == null) { return; }
        if (_addMaterial == null) {
            _addMaterial = new Material(Shader.Find("Hidden/AddShader"));
        }
        
        CommandBuffer cmd = CommandBufferPool.Get();
        ScriptableRenderer renderer = renderingData.cameraData.renderer;
        int kernelHandle = _computeAsset.shader.FindKernel("CSMain");

        using (new ProfilingScope(cmd, _profilingSampler)) {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            // Note : ExecuteCommandBuffer at least once, makes sure the frame
            // debugger displays everything under the correct title.
            
            _computeAsset.Render(cmd, kernelHandle);
            
            // Set the camera depth texture to the global shader variable
            cmd.SetComputeTextureParam(_computeAsset.shader, kernelHandle, "_CameraDepthTexture", _depthBufferTarget);

            // Set the source and destination textures for the raymarching shader
            cmd.SetComputeTextureParam(_computeAsset.shader, kernelHandle, "Source", _colorBufferTarget);
            cmd.SetComputeTextureParam(_computeAsset.shader, kernelHandle, "Destination", _raymarchBufferTarget);

            // Set the size of the thread groups
            var numThreadsX = Mathf.CeilToInt(_camera.pixelWidth / 8f);
            var numThreadsY = Mathf.CeilToInt(_camera.pixelHeight / 8f);

            // Dispatch the compute shader
            cmd.DispatchCompute(_computeAsset.shader, kernelHandle, numThreadsX, numThreadsY, 1);

            _addMaterial.SetFloat("_Sample", 0);
            
            // Copy the render texture to the destination
            Blitter.BlitCameraTexture(cmd, _raymarchBufferTarget, renderer.cameraColorTargetHandle, _addMaterial, 0);

            //Execute the command buffer and release it back to the pool.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }


    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        ReleaseTargets();
    } 

    public void ReleaseTargets() {
        _colorBufferTarget?.Release();
        _depthBufferTarget?.Release();
        _raymarchBufferTarget?.Release(); 
    }
}