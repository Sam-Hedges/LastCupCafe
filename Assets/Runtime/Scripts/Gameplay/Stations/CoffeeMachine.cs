using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachine : Workstation, IMinigame
{
    [SerializeField] private GameObjectEventChannelSO workstationStateUpdateChannel;
    [SerializeField] private GameObject coffeeGroundsPrefab;
    private PressureGaugeMG _gauge;
    
    [SerializeField] private Image pressureBar;
    [SerializeField] private Image progressBar;
    [SerializeField] private RectTransform needleTransform;
    [SerializeField] private RectTransform sweetZoneTransform;

    //Colour values to be used later
    public Color badColor;
    public Color goodColor;

    //TThe Target pressure player should be reaching
    public float pressureTarget;
    //The error value (how much lower/higher the delta can be and still give a correct output)
    public float pressureVariance;

    private bool _canComplete;
    private float savedInput;
    private float delta;
    private float progressDelta;

    private int charges = 0;
    private const int maxCharges = 3;
    
    private void Start()
    {
        workstationStateUpdateChannel.RaiseEvent(gameObject);
    }

    public bool CanProcessItem(GameObject item) {
        return item.GetComponent<CoffeeBeans>() != null && charges < maxCharges;
    }


    public override void MinigameButton()
    {
        if (currentlyStoredItem.TryGetComponent(out Mug mug) && _canComplete) {
            mug.AddIngredient(IngredientType.CoffeeGrounds);
        }
        else
        {
            Debug.Log("Failed");
        }
    }
    
    private void FixedUpdate() {
        
        switch (savedInput) {
            case 0:
                delta -= 0.01f;
                break;
            case 1:
                delta += 0.01f;
                break;
            default:
                // Lerp to smooth out jittery trigger input
                delta = Mathf.Lerp(delta, savedInput, Time.fixedDeltaTime * 10);
                break;
        }

        // Clamp delta between 0 and 1
        delta = Mathf.Clamp(delta, 0, 1);
        
        // Update pressure bar
        needleTransform.localEulerAngles = new Vector3(0, 0, delta * -240 + 120);

        if (delta >= pressureTarget - pressureVariance && delta <= pressureTarget + pressureVariance)
        {
            pressureBar.color = goodColor;
            progressDelta += 0.01f;
        }
        else
        {
            pressureBar.color = badColor;
            progressDelta -= 0.01f;
        }
        
        progressDelta = Mathf.Clamp(progressDelta, 0, 0.25f);
        
        progressBar.fillAmount = progressDelta;
    }

    public override void MinigameTrigger(float input) {
        savedInput = input;
    }
}