using Mono.CSharp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CoffeeMachine : Workstation, IMinigame
{
    [SerializeField] private GameObjectEventChannelSO workstationStateUpdateChannel;
    [SerializeField] private GameObject coffeeGroundsPrefab;

    public CoffeeMachineMG Icons {
        get => Icons;
        set {
            icons = value;
            gauge = icons.Gauge;
        }
    }

    private CoffeeMachineMG icons;
    private PressureGaugeMG gauge;

    //TThe Target pressure player should be reaching
    public float pressureTarget;
    //The error value (how much lower/higher the delta can be and still give a correct output)
    public float pressureVariance;

    private bool canComplete;
    private float savedInput;
    private float delta;
    private float progressDelta;
    private bool charged;
    
    private void Start()
    {
        workstationStateUpdateChannel.RaiseEvent(gameObject);
    }

    public void InitUI(CoffeeMachineMG cm) {
        Icons = cm;
        gauge = cm.Gauge;
    }

    public override void InitWorkstation() {
        
    }

    public override bool OnPlaceItem(GameObject newItem) {
        
        if (newItem.GetComponent<CoffeeGrounds>()) {
            charged = true;
            Destroy(newItem.gameObject);
            return true;
        }
        
        if (currentlyStoredItem) return false; 
        
        currentlyStoredItem = newItem.GetComponent<Item>();
        
        currentlyStoredItem.transform.position = transform.position;
        currentlyStoredItem.transform.rotation = transform.rotation;
        
        currentlyStoredItem.transform.SetParent(transform);
        currentlyStoredItem.transform.localPosition = Vector3.up;
        return true;
    }
    
    public override void MinigameButton()
    {
        if (currentlyStoredItem.TryGetComponent(out Mug mug) && canComplete) {
            mug.AddIngredient(IngredientType.CoffeeGrounds);
        }
        else
        {
            Debug.Log("Failed");
        }
    }
    
    private void FixedUpdate() {
        
        if (!gauge) return;
        
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
        gauge.needleTransform.localEulerAngles = new Vector3(0, 0, delta * -240 + 120);

        if (delta >= pressureTarget - pressureVariance && delta <= pressureTarget + pressureVariance)
        {
            gauge.pressureBar.color = gauge.goodColor;
            progressDelta += 0.01f;
        }
        else
        {
            gauge.pressureBar.color = gauge.badColor;
            progressDelta -= 0.01f;
        }
        
        progressDelta = Mathf.Clamp(progressDelta, 0, 0.25f);
        
        gauge.progressBar.fillAmount = progressDelta;
    }

    public override void MinigameTrigger(float input) {
        savedInput = input;
    }
}