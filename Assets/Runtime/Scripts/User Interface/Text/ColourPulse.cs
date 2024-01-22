using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class ColourPulse : MonoBehaviour
{
    public Color pulseColour = Color.blue;
    public float pulseDuration = 1f;

    private TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        // Start the color pulsing
        DOTween.To(() => textComponent.color, x => textComponent.color = x, pulseColour, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo); // Infinite loops, Yoyo for back and forth
    }
}