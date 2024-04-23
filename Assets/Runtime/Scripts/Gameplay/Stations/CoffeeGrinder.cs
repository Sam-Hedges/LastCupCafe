using QFSW.QC.Actions;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
public class CoffeeGrinder : Workstation, IProcessItem, IMinigame, IProduceItem
{
    [SerializeField] private GameObject coffeeGroundsPrefab;
    public int charges = 0;
    private const int maxCharges = 3;
    private float grindProgress;
    public bool isFull;

    //UI Variables
    [SerializeField] private GameObject handle;
    [SerializeField] private Image progressBar;
    private Vector2 zeroAngle;
    private float UIAngle;
    [SerializeField] private Color filledCol;
    [SerializeField] private Color warningCol;
    [SerializeField] private Color emptyCol;

    void Start()
    {
        zeroAngle = new Vector2(0.0f, 1.0f);
    }

    public bool CanProcessItem(Item item) {
        return item.GetComponent<CoffeeBeans>() != null && charges < maxCharges;
    }

    public void ProcessItem(Item item) {
        if (!CanProcessItem(item)) return;
        else if (charges < maxCharges)
        {
            Destroy(item.gameObject);
            charges = maxCharges;
        }
    }

    //FindDegree modified and fixed from https://discussions.unity.com/t/how-to-get-0-360-degree-from-two-points/146651

    public static float FindDegree(float x, float y)
    {
        float value = Mathf.Atan2(x, y) / math.PI * 180f;
        if (value < 0) value += 360f;

        return value;
    }

    private Vector2 storedValue;
    public override void MinigameStick(Vector2 input)
    {
        UIAngle = -FindDegree(input.x, input.y);
        handle.transform.localRotation = Quaternion.Euler(0, 0, UIAngle);
        if (!currentlyStoredItem) return;
        if (!currentlyStoredItem.GetComponent<CoffeeBeans>()) return;
        if (storedValue != input && (input.x + input.y >= 0.9f || input.x - input.y <= -0.9f))
        {
            storedValue = input;
            grindProgress += 0.005f;
            progressBar.fillAmount = (grindProgress / 4.0f);
            Debug.Log(grindProgress);
        }
        if (grindProgress >= 1.0f && currentlyStoredItem != null)
        {
            grindProgress = 0.0f;
            ProcessItem(currentlyStoredItem);
            isFull = true;
        }
        ColourUpdate();
    }

    float UIFillAmount;
    Item IProduceItem.ProduceItem()
    {
        charges--;
        UIFillAmount = charges / maxCharges;
        Debug.Log(UIFillAmount);
        progressBar.fillAmount = UIFillAmount*0.25f;
        if(charges <= 0)
        {
            isFull = false;
        }
        ColourUpdate();
        return Instantiate(coffeeGroundsPrefab, transform.position + Vector3.up, Quaternion.identity).GetComponent<Item>();
    }

    private void ColourUpdate()
    {
        if (progressBar.fillAmount >= (1.0f*0.25f))
        {
            progressBar.color = filledCol;
        }
        else if (progressBar.fillAmount >= (0.7f * 0.25f) && UIFillAmount <= (1.0f * 0.25f))
        {
            progressBar.color = warningCol;
        }
        else if (progressBar.fillAmount < (0.5f * 0.25f))
        {
            progressBar.color = emptyCol;
        }
    }
}