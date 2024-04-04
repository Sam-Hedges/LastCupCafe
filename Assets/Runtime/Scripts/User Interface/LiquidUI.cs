using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] [RequireComponent(typeof(Image))]
public class LiquidUI : MonoBehaviour
{
    #region Fields
    
    private Material material;
    private Image imageRenderer;

    [Header("--- Bar Script Parameters ---")]

    [Tooltip("Do the transition effect automatically when the fill amount reach 1")]
    public bool automaticTransitionEffect;

    [Tooltip("Amount of the bar shown, (f.ex: 0.5 is 50%), the bar go to this value more or less quickly depending of the smoothness.")]
    [Range(0, 1)]
    public float targetFillAmount;

    [Tooltip("Defines how fast the bar will go to its target fill amount value")]
    public float smoothness;

    public Color targetForegroundColor;

    private float _currentFillAmount;
    private Color _currentForegroundColor;

    [Header("--- Shader Parameters ---")]

#if UNITY_EDITOR
    [Tooltip("Only compile in editor, tick to apply all the changes to the material")]
    public bool EDITORUpdateMaterial = true;
#endif

    [Header("UVs")]

    [Tooltip("Spherize the UV, useful with a circle imageRenderer")]
    public bool spherize = false;

    [Tooltip("Bar rotation")]
    public Rotation rotation;
    public enum Rotation
    {
        Right,
        Left,
        Up,
        Down
    }

    [Header("Inside Noise")]
    [Tooltip("The scale of the noise inside of the bar")]
    [Range(1, 200)]
    public float insideNoiseScale = 25;

    [Tooltip("Defines how visible is the noise inside the bar")]
    [Range(0, 1)]
    public float insideNoiseIntensity = 0.25f;

    [Tooltip("Defines how detailed is the noise inside the bar")]
    [Range(1, 255)]
    public float insideNoiseColorVariation = 50;

    [Header("Border")]
    [Tooltip("The scale of the noise applied to the border, set to 0 for a straight line")]
    [Range(0, 50)]
    public float borderNoiseScale = 3;

    [Tooltip("The amount of distortion applied to the border, set to 0 for a straight line")]
    [Range(0, 0.3f)]
    public float borderDistortionAmount = 0.1f;

    [Tooltip("Defines how reactive the border light is to the fill amount changes. (f.ex: 100 makes the bar lights up to small value variation)")]
    public float borderLightReactivity = 10;
    
    private bool onTransition;

    [System.NonSerialized]
    public Color originalForegroundColor;
    [System.NonSerialized]
    public Color originalBackgroundColor;
    
    #endregion

    #region Start & Update

    private void Awake()
    {
        imageRenderer = GetComponent<Image>();
        material = imageRenderer.material;

        originalForegroundColor = GetForegroundColor();
        originalBackgroundColor = GetBackgroundColor();

        _currentForegroundColor = GetForegroundColor();


        if (!(material.shader.name == "SamHedges/LiquidUI" || material.shader.name == "SamHedges/LiquidUI_Opal"))
        {
            Debug.Log(material.shader.name);
            Debug.LogError("The shader of the material is not the LiquidUI shader");
        }
    }

    public Color GetBackgroundColor()
    {
        return material.GetColor("_BackgroundColour");
    }

    public Color GetForegroundColor()
    {
        return material.GetColor("_Colour");
    }

    private void SetForegoundColor(Color color)
    {
        material.SetColor("_Colour", color);
    }

    private void SetBackgroundColor(Color color)
    {
        material.SetColor("_BackgroundColour", color);
    }

    public void ResetColors()
    {
        targetForegroundColor = originalForegroundColor;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (EDITORUpdateMaterial)
        {
            UpdateMaterial();
        }
#endif

        if (automaticTransitionEffect)
        {
            if (_currentFillAmount >= 0.99f && !onTransition)
            {
                StartTransition();
            }
        }

        float fillAmountDif = Mathf.Abs(_currentFillAmount - targetFillAmount);

        material.SetFloat("_MovingAmount", fillAmountDif * borderLightReactivity);

        if (!onTransition)
            _currentFillAmount = Mathf.Lerp(_currentFillAmount, targetFillAmount, Time.deltaTime * smoothness);

        _currentForegroundColor = Color.Lerp(_currentForegroundColor, targetForegroundColor, Time.deltaTime * smoothness);

        material.SetFloat("_Progress", _currentFillAmount);

        SetForegoundColor(_currentForegroundColor);
    }

    #endregion

    #region Methods
    public void StartTransition()
    {
        onTransition = true;
        _currentFillAmount = 1;

        StopAllCoroutines();
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            material.SetFloat("_DissolveTransition", t);
            yield return null;
        }

        _currentFillAmount = 0;
        targetFillAmount = 0;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            material.SetFloat("_DissolveAmount", t);
            yield return null;
        }

        material.SetFloat("_DissolveAmount", 0);
        material.SetFloat("_DissolveTransition", 0);

        onTransition = false;
    }

    private void UpdateMaterial()
    {
        switch (rotation)
        {
            case Rotation.Down:
                material.SetFloat("_Rotation", 270);
                break;
            case Rotation.Up:
                material.SetFloat("_Rotation", 90);
                break;
            case Rotation.Left:
                material.SetFloat("_Rotation", 180);
                break;
            case Rotation.Right:
                material.SetFloat("_Rotation", 0);
                break;
        }
        
        material.SetFloat("_BorderNoiseScale", borderNoiseScale);
        material.SetFloat("_InsideNoiseScale", insideNoiseScale);
        material.SetFloat("_Spherize", spherize ? 1 : 0);
        material.SetFloat("_InsideNoiseIntensity", insideNoiseIntensity);
        material.SetFloat("_BorderDistortionAmount", borderDistortionAmount);
        material.SetFloat("_InsideNoiseRoundFactor", insideNoiseColorVariation);
    }

    private void OnApplicationQuit()
    {
        // If the transition occurs and the game is stopped, reset the transition effect.

        material.SetFloat("_DissolveAmount", 0);
        material.SetFloat("_DissolveTransition", 0);

        SetForegoundColor(originalForegroundColor);
    }
    
    private int PropertyToID(Material material, string propertyName) { 
        Shader shader = material.shader;
        return shader.GetPropertyNameId(shader.FindPropertyIndex(propertyName));
    }
    
    #endregion

}
