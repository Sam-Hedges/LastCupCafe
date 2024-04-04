using UnityEngine;
public class CoffeeGrinder : Workstation, IProcessItem, IMinigame
{
    [SerializeField] private GameObject coffeeGroundsPrefab;
    private int charges = 0;
    private const int maxCharges = 3;

    public bool CanProcessItem(Item item) {
        return item.GetComponent<CoffeeBeans>() != null && charges < maxCharges;
    }

    public void ProcessItem(Item item) {
        if (!CanProcessItem(item)) return;
        Destroy(item);
        charges++;
        // Assume there's a minigame mechanism here
    }

    // Method to retrieve coffee grounds
    public Item RetrieveGrounds() {
        if (charges > 0) {
            charges--;
            return Instantiate(coffeeGroundsPrefab, transform.position + Vector3.up, Quaternion.identity).GetComponent<Item>();
        }
        return null;
    }

    public override void OnInteract() {
        base.OnInteract();
        // Assuming the interaction retrieves grounds if available
        if (currentlyStoredItem == null && charges > 0) {
            OnPlaceItem(RetrieveGrounds());
        }
    }

    public void Minigame(bool active, GameObject heldItem)
    {

    }
}