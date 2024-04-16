using QFSW.QC.Actions;
using UnityEngine;
public class CoffeeGrinder : Workstation, IProcessItem, IMinigame, IProduceItem
{
    [SerializeField] private GameObject coffeeGroundsPrefab;
    public int charges = 0;
    private const int maxCharges = 3;
    private float grindProgress;
    public bool isFull;

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

    Vector2 storedValue;
    public override void MinigameStick(Vector2 input)
    {
        if (!currentlyStoredItem.GetComponent<CoffeeBeans>()) return;

        if (storedValue != input && (input.x + input.y >= 0.9f || input.x - input.y <= -0.9f))
        {
            storedValue = input;
            grindProgress += 0.005f;
            Debug.Log(grindProgress);
        }
        if (grindProgress >= 1.0f && currentlyStoredItem != null)
        {
            grindProgress = 0.0f;
            ProcessItem(currentlyStoredItem);
            isFull = true;
        }
    }

    Item IProduceItem.ProduceItem()
    {
        charges--;
        if(charges <= 0)
        {
            isFull = false;
        }
        return Instantiate(coffeeGroundsPrefab, transform.position + Vector3.up, Quaternion.identity).GetComponent<Item>();
    }
}