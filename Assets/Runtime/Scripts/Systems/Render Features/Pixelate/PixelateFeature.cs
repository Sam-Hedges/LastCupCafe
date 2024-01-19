using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelateSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int screenHeight = 144;
    }

    [SerializeField] internal PixelateSettings settings;
    [SerializeField] internal Shader shader;
    private Material material;
    private PixelatePass pixelateRenderPass = null;

    public override void Create() {
        if (shader == null) 
            shader = Shader.Find("Hidden/Pixelate");    
        
        material = new Material(shader);
        pixelateRenderPass = new PixelatePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(pixelateRenderPass);
    }
    
    protected override void Dispose(bool disposing)
    { 
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Destroy(material);
        }
        else
        {
            DestroyImmediate(material);
        }
#else
            Destroy(material);
#endif
    }
}