using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

[RequireComponent(typeof(RectTransform))]
public class MenuAfterImageHandler : MonoBehaviour
{
    [SerializeField] private Ease animationEase = Ease.OutExpo;
    
    [Header("After Image Settings")]
    [SerializeField] private Transform afterImageContainer;
    [SerializeField] private GameObjectPoolManager afterImagePool;
    [SerializeField] private float afterImageLifetime = 0.2f;
    [SerializeField] private Color fadeToColor = Color.clear;
    [SerializeField] private float spawnIntervalSeconds = 0.2f;

    [Header("Menu Animation Settings")]
    [SerializeField] private GameObject menuFields;
    [SerializeField] private float animationDurationSeconds = 0.5f;
    [SerializeField] private Vector2 minSize = Vector2.one;
    [SerializeField] private float delaySeconds = 0.5f;

    private RectTransform rectTransform;
    private Image originalImage;
    private Color startColour;
    private Vector2 originalSizeDelta;
    private Coroutine spawnAfterImagesCoroutine;
    private float elapsedTimeSinceAnimationStart = 0f; // Track the elapsed time since the animation started
    
    public void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalImage = GetComponent<Image>();
        startColour = originalImage.color;
        originalSizeDelta = rectTransform.sizeDelta;
        rectTransform.sizeDelta = minSize;
        
        ToggleMenuFields();
        
        if (delaySeconds > 0)
        {
            DOVirtual.DelayedCall(delaySeconds, StartAnimations);
        }
        else
        {
            StartAnimations();
        }
    }
    
    public void OnDisable()
    {
        return;
    }

    private void StartAnimations()
    {
        EnableAfterImages();
        AnimateMenu();
    }

    private void AnimateMenu()
    {
        rectTransform.DOSizeDelta(originalSizeDelta, animationDurationSeconds)
            .SetEase(animationEase)
            .OnComplete(() => 
            {
                DisableAfterImages();
                ToggleMenuFields();
            });
    }

    private void ToggleMenuFields()
    {
        if (menuFields != null)
        {
            menuFields.SetActive(!menuFields.activeSelf);
        }
    }

    private void EnableAfterImages()
    {
        elapsedTimeSinceAnimationStart = 0f; // Reset the elapsed time when starting
        if (spawnAfterImagesCoroutine == null)
        {
            spawnAfterImagesCoroutine = StartCoroutine(SpawnAfterImages());
        }
    }

    private void DisableAfterImages()
    {
        if (spawnAfterImagesCoroutine != null)
        {
            StopCoroutine(spawnAfterImagesCoroutine);
            spawnAfterImagesCoroutine = null;
        }
    }

    private IEnumerator SpawnAfterImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnIntervalSeconds);
            CreateAfterImage();
            elapsedTimeSinceAnimationStart += spawnIntervalSeconds; // Increment time
        }
    }


    private void CreateAfterImage()
    {
        GameObject afterImageObj = afterImagePool.GetGameObject();

        RectTransform afterImageRect = afterImageObj.GetComponent<RectTransform>();
        CopyRectTransform(rectTransform, afterImageRect);
        afterImageRect.SetParent(afterImageContainer, false);
        afterImageRect.SetAsFirstSibling();

        Image afterImage = afterImageObj.GetComponent<Image>();
        //float alpha = Mathf.Clamp01(elapsedTimeSinceAnimationStart / animationDurationSeconds + 0.2f); // Calculate alpha
        //afterImage.color = new Color(startColour.r, startColour.g, startColour.b, alpha); // Set alpha
        afterImage.DOColor(fadeToColor, afterImageLifetime)
            .SetEase(animationEase)
            .OnComplete(() => DisableAfterImage(afterImageObj, afterImage));
    }
    
    private void DisableAfterImage(GameObject afterImageObj, Image afterImage)
    {
        afterImage.color = startColour;
        afterImagePool.DisableGameObject(afterImageObj);
    }

    private void CopyRectTransform(RectTransform sourceRect, RectTransform destinationRect)
    {
        destinationRect.localPosition = sourceRect.localPosition;
        destinationRect.localRotation = sourceRect.localRotation;
        destinationRect.localScale = sourceRect.localScale;
        destinationRect.anchoredPosition = sourceRect.anchoredPosition;
        destinationRect.sizeDelta = sourceRect.sizeDelta;
    }
}
