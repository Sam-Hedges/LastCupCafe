using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class RaymarchingCamera : MonoBehaviour
{
    [SerializeField]
    private ComputeShader raymarchingShader;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private float maxDistance = 500;

    [SerializeField]
    private int maxIterations = 512;

    [Header("Shadows")]

    [SerializeField]
    private bool useSoftShadows = true;

    [SerializeField]
    [Range(1, 128)]
    private float shadowPenumbra;

    [SerializeField]
    [Range(0, 4)]
    private float shadowIntensity;

    [SerializeField]
    private Vector2 shadowDistance = new Vector2(0.1f, 20);

    [Header("Ambient Occlusion")]

    [SerializeField]
    private bool aoEnabled = true;

    [SerializeField]
    [Range(0.01f, 10f)]
    private float aoStepSize;

    [SerializeField]
    [Range(1, 5)]
    private int aoIterations;

    [SerializeField]
    [Range(0,1)]
    private float aoIntensity;

    private Camera m_cam;
    public Camera Camera
    {
        get
        {
            if (!m_cam)
                m_cam = GetComponent<Camera>();
            return m_cam;
        }
    }

    /// <summary>
    /// Target texture for raymarching
    /// </summary>
    private RenderTexture renderTextureTarget;

    /// <summary>
    /// buffers to dispose of after rendering
    /// </summary>
    private List<ComputeBuffer> buffers;

    // called after the camera renders to modify the final image
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // if no shader is assigned, just skip this effect
        if (!raymarchingShader)
        {
            // copy the source texture to the destination
            Graphics.Blit(source, destination);
            return;
        }

        // list of vulkan buffers to dispose of after rendering
        buffers = new List<ComputeBuffer>();

        CreateTexture();
        LoadShapes();
        SetParameters();
        LoadLight();

        // set the camera depth texture to the global shader variable
        raymarchingShader.SetTexture(0, "_CameraDepthTexture", Shader.GetGlobalTexture("_CameraDepthTexture"));
        
        // set the source and destination textures for the raymarching shader
        raymarchingShader.SetTexture(0, "Source", source);
        raymarchingShader.SetTexture(0, "Destination", renderTextureTarget);

        // set the size of the thread groups
        int numThreadsX = Mathf.CeilToInt(Camera.pixelWidth / 8f);
        int numThreadsY = Mathf.CeilToInt(Camera.pixelHeight / 8f);

        // dispatch the compute shader
        raymarchingShader.Dispatch(0, numThreadsX, numThreadsY, 1);

        // copy the render texture to the destination
        Graphics.Blit(renderTextureTarget, destination);

        // dispose of the buffers
        foreach (var buffer in buffers)
        {
            buffer.Dispose();
        }
    }

    /// <summary>
    /// Create texture for raymarching
    /// </summary>
    private void CreateTexture() {
        // if the render texture already exists and is the correct size, return
        if (renderTextureTarget != null && renderTextureTarget.width == Camera.pixelWidth &&
            renderTextureTarget.height == Camera.pixelHeight) return;
        
        // otherwise, release the texture and create a new one
        if (renderTextureTarget)
            renderTextureTarget.Release();
        renderTextureTarget = new RenderTexture(Camera.pixelWidth, Camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        renderTextureTarget.enableRandomWrite = true; // allow compute shader to write to this texture
        renderTextureTarget.Create();
    }

    /// <summary>
    /// Set the parameters for the raymarching shader
    /// </summary>
    private void SetParameters()
    {
        raymarchingShader.SetMatrix("cameraToWorld", Camera.cameraToWorldMatrix);
        raymarchingShader.SetMatrix("cameraInverseProjection", Camera.projectionMatrix.inverse);

        raymarchingShader.SetFloat("maxDistance", maxDistance);
        raymarchingShader.SetInt("maxIterations", maxIterations);

        raymarchingShader.SetFloat("shadowIntensity", shadowIntensity);
        raymarchingShader.SetFloat("shadowPenumbra", shadowPenumbra);
        raymarchingShader.SetBool("softShadows", useSoftShadows);
        raymarchingShader.SetVector("shadowDistance", shadowDistance);

        raymarchingShader.SetFloat("aoStepSize", aoStepSize);
        raymarchingShader.SetFloat("aoIntensity", aoIntensity);
        raymarchingShader.SetInt("aoIterations", aoIterations);
        raymarchingShader.SetBool("aoEnabled", aoEnabled);
    }

    /// <summary>
    /// 
    /// </summary>
    private void LoadShapes()
    {
        // get all shapes in the scene
        List<BaseShape> shapes = new List<BaseShape>(FindObjectsOfType<BaseShape>());

        if (shapes.Count == 0) return; // if there are no shapes, return
        
        // pass the number of shapes in the scene to the shader
        raymarchingShader.SetInt("shapesCount", shapes.Count);

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
        raymarchingShader.SetBuffer(0, "shapes", buffer);

        // add the buffer to the list of buffers to dispose of after rendering
        buffers.Add(buffer);
    }

    /// <summary>
    /// Set the light parameters for the raymarching shader to the values of the sun light in the scene
    /// </summary>
    private void LoadLight()
    {
        Vector3 direction = Vector3.down;
        Color color = Color.white;
        float intensity = 1;

        if (sunLight)
        {
            direction = sunLight.transform.forward;
            color = sunLight.color;
            intensity = sunLight.intensity;
        }

        raymarchingShader.SetVector("lightDirection", direction);
        raymarchingShader.SetVector("lightColor", new Vector3(color.r, color.g, color.b));
        raymarchingShader.SetFloat("lightIntensity", intensity);
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
