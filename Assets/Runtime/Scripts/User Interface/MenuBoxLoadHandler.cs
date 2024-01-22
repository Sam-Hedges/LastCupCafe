using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

[RequireComponent(typeof(AfterImage))]
[RequireComponent(typeof(RectTransform))]
public class MenuBoxLoadHandler : MonoBehaviour
{
    [SerializeField] private RectTransform menuRectTransform; // The RectTransform to animate
    [SerializeField] private GameObject menuFields; // The child object containing menu fields
    [SerializeField] private float animationDurationSeconds = 0.5f; // Duration of the animation
    [SerializeField] private Vector2 minSize = Vector2.one;
    [SerializeField] private float delaySeconds = 2f; // Delay before the animation starts

    private Vector3 originalSizeDelta; // To store the original scale
    private AfterImage afterImage;

    private void Start()
    {
        ToggleMenuFields();

        menuRectTransform = GetComponent<RectTransform>();
        afterImage = GetComponent<AfterImage>();
        
        afterImage.EnableAfterImages();
        
        // Store the original size and set the initial size to zero
        originalSizeDelta = menuRectTransform.sizeDelta;
        menuRectTransform.sizeDelta = minSize;

        // Start the animation
        DOVirtual.DelayedCall(delaySeconds, AnimateMenu);
    }

    private void AnimateMenu() {
        
        // Animate the size from (0, 0) to the original size
        menuRectTransform.DOSizeDelta(originalSizeDelta, animationDurationSeconds)
            .SetEase(Ease.OutCubic)
            .OnComplete(FinishedAnimating);
    }
    
    private void FinishedAnimating() {
        // Disable the afterimages
        afterImage.DisableAfterImages();
        ToggleMenuFields();
    }

    private void ToggleMenuFields()
    {
        if (menuFields != null)
        {
            menuFields.SetActive(!menuFields.activeSelf);
        }
    }
}