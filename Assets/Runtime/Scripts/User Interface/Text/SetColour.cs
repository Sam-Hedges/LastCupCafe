using UnityEngine;

public class SetColour : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private TextColorSO colorSettings;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
        if (colorSettings == null)
        {
            Debug.LogWarning("ColorSettings is not assigned. Using default colors.", this);
            colorSettings = ScriptableObject.CreateInstance<TextColorSO>();
        }
    }

    public void SetTMPColourToDefault()
    {
        textMeshProUGUI.color = colorSettings.defaultColour;
    }
    
    public void SetTMPColourToSelected()
    {
        textMeshProUGUI.color = colorSettings.selectedColour;
    }
}