using UnityEngine;
using UnityEngine.UI;

public class SetValue : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textMeshProUGUI;
    
    private void Awake()
    {
        textMeshProUGUI = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void SetTMPSliderValue(Slider slider)
    {
        textMeshProUGUI.text = slider.value.ToString();
    }
    
}