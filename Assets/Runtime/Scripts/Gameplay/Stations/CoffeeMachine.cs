using Mono.CSharp;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachine : Workstation, IMinigame
{
    [SerializeField] private GameObjectEventChannelSO workstationStateUpdateChannel;
    [SerializeField] private GameObject coffeeGroundsPrefab;
    private CoffeeMachineMG _icons;
    private PressureGaugeMG _gauge;

    //TThe Target pressure player should be reaching
    public float pressureTarget;
    //The error value (how much lower/higher the delta can be and still give a correct output)
    public float pressureVariance;

    private bool _canComplete;
    private float savedInput;
    private float delta;
    private float progressDelta;
    private bool charged;
    
    private void Start()
    {
        workstationStateUpdateChannel.RaiseEvent(gameObject);
    }

    public void InitUI(CoffeeMachineMG cm) {
        _icons = cm;
        _gauge = cm.Gauge;
    }

    public override void InitWorkstation() {
        
    }

    public override void OnPlaceItem(Item newItem) {
        
        if (newItem.GetType() == typeof(CoffeeGrounds)) {
            charged = true;
            Destroy(newItem.gameObject);
        }
        
        if (currentlyStoredItem != null) return; 
        
        currentlyStoredItem = newItem;
        
        currentlyStoredItem.transform.position = transform.position;
        currentlyStoredItem.transform.rotation = transform.rotation;
        
        currentlyStoredItem.transform.SetParent(transform);
        currentlyStoredItem.transform.localPosition = Vector3.up;
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
        _gauge.needleTransform.localEulerAngles = new Vector3(0, 0, delta * -240 + 120);

        if (delta >= pressureTarget - pressureVariance && delta <= pressureTarget + pressureVariance)
        {
            _gauge.pressureBar.color = _gauge.goodColor;
            progressDelta += 0.01f;
        }
        else
        {
            _gauge.pressureBar.color = _gauge.badColor;
            progressDelta -= 0.01f;
        }
        
        progressDelta = Mathf.Clamp(progressDelta, 0, 0.25f);
        
        _gauge.progressBar.fillAmount = progressDelta;
    }

    public override void MinigameTrigger(float input) {
        savedInput = input;
    }
}