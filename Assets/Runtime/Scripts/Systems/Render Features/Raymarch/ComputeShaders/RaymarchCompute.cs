using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Compute/Raymarch Compute")]
public class RaymarchCompute : ComputeAsset {
    
    [Header("General")] 
    public List<BaseShape> shapes;
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

    private List<ComputeBuffer> _computeBuffers;
    private Camera _camera;

    private void InitRaymarchShaderTags(CommandBuffer cmd) {
        cmd.SetComputeMatrixParam(shader, "cameraToWorld", _camera.cameraToWorldMatrix);
        cmd.SetComputeMatrixParam(shader, "cameraInverseProjection", _camera.projectionMatrix.inverse);

        cmd.SetComputeFloatParam(shader, "maxDistance", maxDistance);
        cmd.SetComputeIntParam(shader, "maxIterations", maxIterations);

        cmd.SetComputeFloatParam(shader, "shadowIntensity", shadowIntensity);
        cmd.SetComputeFloatParam(shader, "shadowPenumbra", shadowPenumbra);
        cmd.SetComputeIntParam(shader, "softShadows", useSoftShadows ? 1 : 0);
        cmd.SetComputeVectorParam(shader, "shadowDistance", shadowDistance);
        
        cmd.SetComputeFloatParam(shader, "aoStepSize", aoStepSize);
        cmd.SetComputeFloatParam(shader, "aoIntensity", aoIntensity);
        cmd.SetComputeIntParam(shader, "aoIterations", aoIterations);
        cmd.SetComputeIntParam(shader, "aoEnabled", aoEnabled ? 1 : 0);
    }

    public override void Setup(RenderingData renderingData) {

        shapes = new List<BaseShape>(FindObjectsOfType<BaseShape>());
        light = RenderSettings.sun;
        _camera = renderingData.cameraData.camera;
    }

    public override void Render(CommandBuffer commandBuffer, int kernelHandle) {
        Cleanup();

        _computeBuffers = new List<ComputeBuffer>();
        
        InitRaymarchShaderTags(commandBuffer);
        LoadShapes(commandBuffer);
        LoadLight(commandBuffer);
    }

    public override void Cleanup() {
        foreach (var buffer in _computeBuffers) {
            buffer?.Dispose();
        }
        _computeBuffers.Clear();
    }

    private void LoadShapes(CommandBuffer cmd) {
        // get all shapes in the scene
        List<BaseShape> tempShapesList = shapes;

        if (tempShapesList.Count == 0) return; // if there are no shapes, return

        // pass the number of shapes in the scene to the shader
        cmd.SetComputeIntParam(shader, "shapesCount", tempShapesList.Count);

        // sort the shapes by operation type
        tempShapesList.Sort((a, b) => a.operationType.CompareTo(b.operationType));

        // create a buffer to store the shape data
        ShapeData[] shapeData = new ShapeData[tempShapesList.Count];

        // iterate through the shapes and add their data to the buffer
        for (int i = 0; i < tempShapesList.Count; i++) {
            var shape = tempShapesList[i];
            var transform = shape.transform;
            
            shapeData[i] = new ShapeData() {
                position = transform.position,
                scale = shape.Scale,
                rotation = transform.eulerAngles,
                blendStrength = shape.blendStrength,
                color = shape.Color,
                data = shape.CreateExtraData(),
                operationType = (int)shape.operationType,
                shapeType = (int)shape.ShapeType
            };
        }

        // create a compute buffer to store the shape data
        ComputeBuffer buffer = new ComputeBuffer(tempShapesList.Count, ShapeData.GetStride());
        buffer.SetData(shapeData);

        // pass the buffer to the shader
        cmd.SetComputeBufferParam(shader, 0, "shapes", buffer);

        // add the buffer to the list of buffers to dispose of after rendering
        _computeBuffers.Add(buffer);
    }

    /// <summary>
    /// Set the light parameters for the raymarching shader to the values of the sun light in the scene
    /// </summary>
    private void LoadLight(CommandBuffer cmd) {
        Vector3 direction = Vector3.down;
        Color color = Color.white;
        float intensity = 1;

        if (light) {
            direction = light.transform.forward;
            color = light.color;
            intensity = light.intensity;
        }

        cmd.SetComputeVectorParam(shader, "lightDirection", direction);
        cmd.SetComputeVectorParam(shader, "lightColor", new Vector3(color.r, color.g, color.b));
        cmd.SetComputeFloatParam(shader, "lightIntensity", intensity);
    }

    /// <summary>
    /// Contains data for a shape
    /// </summary>
    private struct ShapeData {
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
        public static int GetStride() {
            return sizeof(float) * 17 + sizeof(int) * 2;
        }
    }
}