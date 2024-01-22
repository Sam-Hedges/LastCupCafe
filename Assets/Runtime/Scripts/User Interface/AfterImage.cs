using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class AfterImage : MonoBehaviour
{
    public Transform container; // Target to follow
    public GameObjectPoolManager pool;
    public float afterImageLifetime = 0.2f; // Lifetime of each afterimage
    public Color fadeToColor = Color.clear; // Color to fade to
    public float spawnIntervalSeconds = 0.2f; // Interval between spawning afterimages

    private Image originalImage;
    private RectTransform rectTransform;
    private Color startColour;
    
    private Coroutine spawnAfterImagesCoroutine;

    
    private void Start() {
        originalImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        startColour = originalImage.color;
    }
    
    public void EnableAfterImages() {
        if (spawnAfterImagesCoroutine == null)
        {
            spawnAfterImagesCoroutine = StartCoroutine(SpawnAfterImages());
        }
    }
    public void DisableAfterImages() {
        if (spawnAfterImagesCoroutine != null)
        {
            StopCoroutine(spawnAfterImagesCoroutine);
        }
    }

    private IEnumerator SpawnAfterImages() {
        while (true)
        {
            yield return new WaitForSeconds(spawnIntervalSeconds);
            CreateAfterImage();
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImageObj = pool.GetGameObject();
        
        RectTransform afterImageRect = afterImageObj.GetComponent<RectTransform>();
        CopyRectTransform(rectTransform, afterImageRect);
        afterImageRect.SetParent(container, false);
        afterImageRect.SetAsFirstSibling(); // Place behind the original image

        Image afterImage = afterImageObj.GetComponent<Image>();
        afterImage.color = startColour;
        afterImage.DOColor(fadeToColor, afterImageLifetime)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => DisableGameObject(afterImageObj));
    }
    
    private void CopyRectTransform(RectTransform sourceRect, RectTransform destinationRect)
    {
        destinationRect.localPosition = sourceRect.localPosition;
        destinationRect.localRotation = sourceRect.localRotation;
        destinationRect.localScale = sourceRect.localScale;
        destinationRect.anchoredPosition = sourceRect.anchoredPosition;
        destinationRect.sizeDelta = sourceRect.sizeDelta;
    }

    private void DisableGameObject(GameObject gameObject)
    {
        pool.DisableGameObject(gameObject);
    }
    
}