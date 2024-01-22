using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelizeFeature : ScriptableRendererFeature {
    
    [SerializeField] internal PixelizeSettings settings;
    [SerializeField] internal Shader shader;
    private Material material;
    private PixelizePass pixelizeRenderPass;

    public override void Create() {
        if (shader == null) 
            shader = Shader.Find("Hidden/Pixelate");    
        
        material = new Material(shader);
        pixelizeRenderPass = new PixelizePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(pixelizeRenderPass);
    }
}

[System.Serializable]
public class PixelizeSettings {
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public int screenHeight = 144;
}