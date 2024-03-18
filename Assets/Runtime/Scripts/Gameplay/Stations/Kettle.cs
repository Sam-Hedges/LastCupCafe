using UnityEngine;
using UnityEngine.UI;

public class Kettle : Workstation, /*IProduceItem,*/ IMinigameInteract {

    //---ProduceItem has been disabled to allow Minigame interactions - errors are thrown when both ProduceItem and Minigame are present at once
    //---ProduceItem also doesn't appear to be properly implemented? Unsure how it's meant to work
    /*public GameObject ProduceItem() {
        // Simulate filling a mug with water, ideally with a minigame for stopping at the right moment
        return null;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        ProduceItem();
    }*/

    //Scrollbar UI object
    public Scrollbar scrollBar;

    public Color filledCol;
    public Color warningCol;
    public Color emptyCol;

    //Timer functions
    float targetTime = 0.0f;
    public float startTime = 20.0f;

    //If true water can be taken, if false kettle must be re-boiled before water can be taken
    //Should be used alongside ProduceItem I think?
    bool isActive = true;

    void Update()
    {
        ColorBlock cb = scrollBar.colors;

        targetTime -= Time.deltaTime;
        scrollBar.size = (targetTime / startTime);
        if (targetTime > (startTime*0.7f))
        {
            cb.disabledColor = filledCol;
            scrollBar.colors = cb;
        }
        else if (targetTime > (startTime*0.2f))
        {
            cb.disabledColor = warningCol;
            scrollBar.colors = cb;
        }
        else if (targetTime < (startTime * 0.2f))
        {
            cb.disabledColor = emptyCol;
            scrollBar.colors = cb;
        }
        else if (targetTime <= 0.0f)
        {
            isActive = false;
            targetTime = 0.0f;
        }

        if (currentlyStoredItem.name == "Mug" && isActive == true)
        {
            currentlyStoredItem.GetComponent<Mug>().AddIngredient("Water");
        }

    }

    public override void MinigameButton(GameObject heldItem)
    {
        targetTime = startTime;
        Debug.Log("Kettle Active");
        isActive = true;
    }
}