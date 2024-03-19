using UnityEngine;
using UnityEngine.UI; // Needed for working with UI Image

public class UISpinner : MonoBehaviour
{
    [SerializeField] private float rotateSpeedMin = -150f;
    [SerializeField] private float rotateSpeedMax = -600f;
    private float currentRotateSpeed;

    [SerializeField] private float fillAmountMin = 0.125f;
    [SerializeField] private float fillAmountMax = 0.625f;
    private bool isIncreasing = true;

    [SerializeField] private float growShrinkSpeed = 2f; // Speed of fill amount change

    private RectTransform _rectComponent;
    private Image _loadingIndicator;

    private void Start()
    {
        _loadingIndicator = GetComponent<Image>();
        _rectComponent = GetComponent<RectTransform>();
        currentRotateSpeed = rotateSpeedMin;
    }

    private void Update()
    {
        // Rotate the spinner
        _rectComponent.Rotate(0f, 0f, currentRotateSpeed * Time.deltaTime);

        // Adjust fill amount and speed
        if (isIncreasing)
        {
            _loadingIndicator.fillAmount += growShrinkSpeed * Time.deltaTime;
            if (_loadingIndicator.fillAmount >= fillAmountMax)
            {
                isIncreasing = false;
                currentRotateSpeed = rotateSpeedMax; // Speed up when at max fill
            }
        }
        else
        {
            _loadingIndicator.fillAmount -= growShrinkSpeed * Time.deltaTime;
            if (_loadingIndicator.fillAmount <= fillAmountMin)
            {
                isIncreasing = true;
                currentRotateSpeed = rotateSpeedMin; // Slow down when shrinking
            }
        }
    }
}