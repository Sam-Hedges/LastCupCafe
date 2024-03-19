using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachine : Workstation, IMinigameInteract
{
    [SerializeField] private GameObject coffeeGroundsPrefab;
    private int charges = 0;
    private const int maxCharges = 3;

    public bool CanProcessItem(GameObject item) {
        return item.GetComponent<CoffeeBeans>() != null && charges < maxCharges;
    }

    public Image pressureBar;

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


    public override void MinigameButton(GameObject heldItem)
    {
        if (_canComplete && currentlyStoredItem.name == "Mug")
        {
            currentlyStoredItem.GetComponent<Mug>().AddIngredient("Coffee");
        }
        else
        {
            Debug.Log("Failed");
        }
    }
    
    private void FixedUpdate() {
        if (savedInput > 0) {
            delta += 0.01f;
        }
        else {
            delta -= 0.01f;
        }

        delta = Mathf.Clamp(delta, 0, 1);
        
        PressureOutput(delta);

        if (delta >= pressureTarget - pressureVariance && delta <= pressureTarget + pressureVariance)
        {
            pressureBar.color = goodColor;
            _canComplete = true;
        }
        else
        {
            pressureBar.color = badColor;
            _canComplete = false;
        }
    }

    public override void MinigameTrigger(float input, GameObject heldItem) {
        
        savedInput = input;
    }

    //Used for setting pressure value for UI
    private void PressureOutput(float input)
    {
        float outputValue = input * 0.75f;
        pressureBar.fillAmount = outputValue;
    }
}