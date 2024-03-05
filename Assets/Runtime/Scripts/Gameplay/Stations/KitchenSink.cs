using UnityEngine;
public class KitchenSink : Workstation, IProcessItem, IMinigameInteract
{
    GameObject minigameCanvas;

    public bool CanProcessItem(GameObject item)
    {
        // Check if the item is a dirty mug
        return item.GetComponent<Mug>() != null && item.GetComponent<Mug>().IsDirty;
    }

    public void ProcessItem(GameObject item)
    {
        if (!CanProcessItem(item)) return;
        // Implement the cleaning minigame here. For now, we'll just clean the mug directly.
        item.GetComponent<Mug>().Clean();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        if (currentlyStoredItem != null)
        {
            ProcessItem(currentlyStoredItem);
        }
    }

    public void Minigame(bool active, GameObject heldItem)
    {
        minigameCanvas = this.gameObject.transform.GetChild(0).gameObject;
        if (active == true)
        {
            minigameCanvas.SetActive(true);
            Debug.Log("Activated Station");

        }
        else
        {
            minigameCanvas.SetActive(false);
            Debug.Log("Left Station");
        }

    }
}