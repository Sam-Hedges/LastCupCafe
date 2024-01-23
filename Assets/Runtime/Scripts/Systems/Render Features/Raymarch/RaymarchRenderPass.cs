using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchRenderPass : ScriptableRenderPass
{
    private RaymarchSettings defaultSettings;
    private ComputeShader shader;
    private Camera camera;

    private RenderTextureDescriptor raymarchTextureDescriptor;
    private RTHandle raymarchTextureHandle;
    private RTHandle cameraDepthTextureHandle;

    private RenderTexture renderTextureTarget;
    private List<ComputeBuffer> computeBuffers; // List of buffers to be disposed

    public RaymarchRenderPass(RaymarchSettings defaultSettings)
    {
        this.shader = defaultSettings.shader;
        this.defaultSettings = defaultSettings;
        this.renderPassEvent = defaultSettings.renderPassEvent;

        raymarchTextureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    /// <summary>
    /// Housekeeping to configure the render pass
    /// </summary>
    public override void Configure(CommandBuffer cmd,
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the texture size to be the same as the camera target size.
        raymarchTextureDescriptor.width = cameraTextureDescriptor.width;
        raymarchTextureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref raymarchTextureHandle, raymarchTextureDescriptor);
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        camera = renderingData.cameraData.camera;
        
        // create a render texture to store the raymarched scene
        RenderTexture rt = new RenderTexture(renderingData.cameraData.renderer.cameraDepthTargetHandle)
        {
            enableRandomWrite = true
        };
        rt.Create();
        cameraDepthTextureHandle = RTHandles.Alloc(rt);
    }

    private void UpdateRaymarchSettings()
    {
        shader.SetMatrix("cameraToWorld", camera.cameraToWorldMatrix);
        shader.SetMatrix("cameraInverseProjection", camera.projectionMatrix.inverse);

        shader.SetFloat("maxDistance", defaultSettings.maxDistance);
        shader.SetInt("maxIterations",  defaultSettings.maxIterations);

        shader.SetFloat("shadowIntensity",  defaultSettings.shadowIntensity);
        shader.SetFloat("shadowPenumbra",  defaultSettings.shadowPenumbra);
        shader.SetBool("softShadows",  defaultSettings.useSoftShadows);
        shader.SetVector("shadowDistance",  defaultSettings.shadowDistance);

        shader.SetFloat("aoStepSize",  defaultSettings.aoStepSize);
        shader.SetFloat("aoIntensity",  defaultSettings.aoIntensity);
        shader.SetInt("aoIterations",  defaultSettings.aoIterations);
        shader.SetBool("aoEnabled",  defaultSettings.aoEnabled);
    }

    private void LoadShapes()
    {
        // get all shapes in the scene
        List<BaseShape> shapes = defaultSettings.shapes;

        if (shapes.Count == 0) return; // if there are no shapes, return
        
        // pass the number of shapes in the scene to the shader
        shader.SetInt("shapesCount", shapes.Count);

        // sort the shapes by operation type
        shapes.Sort((a, b) => a.operationType.CompareTo(b.operationType));

        // create a buffer to store the shape data
        ShapeData[] shapeData = new ShapeData[shapes.Count];
        
        // iterate through the shapes and add their data to the buffer
        for (int i = 0; i < shapes.Count; i++)
        {
            var shape = shapes[i];
            shapeData[i] = new ShapeData()
            {
                position = shape.transform.position,
                scale = shape.Scale,
                rotation = shape.transform.eulerAngles,
                blendStrength = shape.blendStrength,
                color = shape.Color,
                data = shape.CreateExtraData(),
                operationType = (int)shape.operationType,
                shapeType = (int)shape.ShapeType
            };
        }

        // create a compute buffer to store the shape data
        ComputeBuffer buffer = new ComputeBuffer(shapes.Count, ShapeData.GetStride());
        buffer.SetData(shapeData);
        
        // pass the buffer to the shader
        shader.SetBuffer(0, "shapes", buffer);

        // add the buffer to the list of buffers to dispose of after rendering
        computeBuffers.Add(buffer);
    }
    
    /// <summary>
    /// Set the light parameters for the raymarching shader to the values of the sun light in the scene
    /// </summary>
    private void LoadLight()
    {
        Vector3 direction = Vector3.down;
        Color color = Color.white;
        float intensity = 1;

        if (defaultSettings.sunLight)
        {
            direction = defaultSettings.sunLight.transform.forward;
            color = defaultSettings.sunLight.color;
            intensity = defaultSettings.sunLight.intensity;
        }

        shader.SetVector("lightDirection", direction);
        shader.SetVector("lightColor", new Vector3(color.r, color.g, color.b));
        shader.SetFloat("lightIntensity", intensity);
    } 
    
    public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        // list of vulkan buffers to dispose of after rendering
        computeBuffers = new List<ComputeBuffer>();
        
        UpdateRaymarchSettings();
        LoadShapes();
        LoadLight();

        // set the camera depth texture to the global shader variable
        shader.SetTexture(0, "_CameraDepthTexture", cameraDepthTextureHandle);
        
        // set the source and destination textures for the raymarching shader
        shader.SetTexture(0, "Source", cameraTargetHandle);
        shader.SetTexture(0, "Destination", raymarchTextureHandle);

        // set the size of the thread groups
        int numThreadsX = Mathf.CeilToInt(camera.pixelWidth / 8f);
        int numThreadsY = Mathf.CeilToInt(camera.pixelHeight / 8f);

        // dispatch the compute shader
        shader.Dispatch(0, numThreadsX, numThreadsY, 1);

        // copy the render texture to the destination
        Blit(cmd, raymarchTextureHandle, cameraTargetHandle, null, 1);

        // dispose of the buffers
        foreach (var buffer in computeBuffers)
        {
            buffer.Dispose();
        }
        
        // Blit from the camera target to the temporary render texture,
        // using the first shader pass.
        
        // Blit(cmd, cameraTargetHandle, raymarchTextureHandle, null, 0);
        
        // Blit from the temporary render texture to the camera target,
        // using the second shader pass.
        
        // Blit(cmd, raymarchTextureHandle, cameraTargetHandle, null, 1);

        //Execute the command buffer and release it back to the pool.
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        if (raymarchTextureHandle != null) raymarchTextureHandle.Release();
    }
    
    /// <summary>
    /// Contains data for a shape
    /// </summary>
    private struct ShapeData
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 rotation;
        public Vector3 color;
        public Vector4 data;
        public int shapeType;
        public int operationType;
        public float blendStrength;

        /// <summary>
        /// Return the size of the struct in bytes
        /// </summary>
        /// <returns>
        /// Stride is the size of one element in the buffer, in bytes. Must be a multiple of 4 and less than 2048,
        /// and match the size of the buffer type in the shader. 
        /// </returns>
        public static int GetStride()
        {
            return sizeof(float) * 17 + sizeof(int) * 2;
        }
    } 
}