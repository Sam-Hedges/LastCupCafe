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

    //The error value (how much lower/higher the input can be and still give a correct output)
    public float pressureVariance;

    bool canComplete;


    public override void MinigameButton(GameObject heldItem)
    {
        if (canComplete == true && currentlyStoredItem.name == "Mug")
        {
            currentlyStoredItem.GetComponent <Mug>().AddIngredient("Coffee");
        }
        else
        {
            Debug.Log("Failed");
        }
    }

    public override void MinigameTrigger(float input, GameObject heldItem)
    {
        pressureOutput(input);

        if (input >= (pressureTarget-pressureVariance) && input <=(pressureTarget+pressureVariance))
        {
            pressureBar.color = goodColor;
            canComplete = true;
        }
        else
        {
            pressureBar.color = badColor;
            canComplete = false;
        }
    }

    //Used for setting pressure value for UI
    private void pressureOutput(float input)
    {
        float outputValue = (input * 0.75f);
        pressureBar.fillAmount = outputValue;
    }
}