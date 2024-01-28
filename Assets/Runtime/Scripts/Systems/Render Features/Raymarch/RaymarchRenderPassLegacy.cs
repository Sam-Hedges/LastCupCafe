using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchRenderPassLegacy : ScriptableRenderPass
{
    private RaymarchSettings settings;
    private ComputeShader shader;
    private Camera camera;
    private RTHandle colorBufferHandle;
    private RTHandle depthBufferHandle;
    private List<ComputeBuffer> computeBuffers;

    public RaymarchRenderPassLegacy(RaymarchSettings settings)
    {
        this.settings = settings;
        this.shader = settings.shader;
    }

    public void Setup(RTHandle colorBuffer, RTHandle depthBuffer)
    {
        this.colorBufferHandle = colorBuffer;
        this.depthBufferHandle = depthBuffer;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        camera = renderingData.cameraData.camera;
        camera.depthTextureMode = DepthTextureMode.Depth;

        computeBuffers = new List<ComputeBuffer>();
    }
    
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        foreach (var buffer in computeBuffers)
        {
            buffer.Dispose();
        }

        computeBuffers.Clear();
    } 

    private void UpdateRaymarchSettings()
    {
        shader.SetMatrix("cameraToWorld", camera.cameraToWorldMatrix);
        shader.SetMatrix("cameraInverseProjection", camera.projectionMatrix.inverse);

        shader.SetFloat("maxDistance", settings.maxDistance);
        shader.SetInt("maxIterations",  settings.maxIterations);

        shader.SetFloat("shadowIntensity",  settings.shadowIntensity);
        shader.SetFloat("shadowPenumbra",  settings.shadowPenumbra);
        shader.SetBool("softShadows",  settings.useSoftShadows);
        shader.SetVector("shadowDistance",  settings.shadowDistance);

        shader.SetFloat("aoStepSize",  settings.aoStepSize);
        shader.SetFloat("aoIntensity",  settings.aoIntensity);
        shader.SetInt("aoIterations",  settings.aoIterations);
        shader.SetBool("aoEnabled",  settings.aoEnabled);
    }

    private void LoadShapes()
    {
        // get all shapes in the scene
        List<BaseShape> shapes = settings.shapes;

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

        if (settings.light)
        {
            direction = settings.light.transform.forward;
            color = settings.light.color;
            intensity = settings.light.intensity;
        }

        shader.SetVector("lightDirection", direction);
        shader.SetVector("lightColor", new Vector3(color.r, color.g, color.b));
        shader.SetFloat("lightIntensity", intensity);
    } 
    
    public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        // Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        // New blank destination buffer to render into
        RTHandle destBufferHandle = RTHandles.Alloc(0);

        // List of vulkan buffers to dispose of after rendering
        computeBuffers = new List<ComputeBuffer>();
        
        UpdateRaymarchSettings();
        LoadShapes();
        LoadLight();

        // Set the camera depth texture to the global shader variable
        shader.SetTexture(0, "_CameraDepthTexture", depthBufferHandle);
        
        // Set the source and destination textures for the raymarching shader
        shader.SetTexture(0, "Source", colorBufferHandle);
        shader.SetTexture(0, "Destination", destBufferHandle);

        // Set the size of the thread groups
        int numThreadsX = Mathf.CeilToInt(camera.pixelWidth / 8f);
        int numThreadsY = Mathf.CeilToInt(camera.pixelHeight / 8f);

        // Dispatch the compute shader
        shader.Dispatch(0, numThreadsX, numThreadsY, 1);

        // Copy the render texture to the destination
        Blitter.BlitCameraTexture(cmd, destBufferHandle, colorBufferHandle, null, 1 );

        // Dispose of the buffers
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