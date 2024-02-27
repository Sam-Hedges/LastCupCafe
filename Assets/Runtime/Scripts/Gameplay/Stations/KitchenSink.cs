using UnityEngine;
public class KitchenSink : Workstation, IProcessItem, IMinigameInteract {
    public bool CanProcessItem(GameObject item) {
        // Check if the item is a dirty mug
        return item.GetComponent<Mug>() != null && item.GetComponent<Mug>().IsDirty;
    }

    public void ProcessItem(GameObject item) {
        if (!CanProcessItem(item)) return;
        // Implement the cleaning minigame here. For now, we'll just clean the mug directly.
        item.GetComponent<Mug>().Clean();
    }

    public override void OnInteract() {
        base.OnInteract();
        if (currentlyStoredItem != null) {
            ProcessItem(currentlyStoredItem);
        }
    }
}