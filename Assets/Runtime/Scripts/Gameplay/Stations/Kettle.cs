using UnityEngine;
public class Kettle : Workstation, IProduceItem, IMinigameInteract {
    public GameObject ProduceItem() {
        // Simulate filling a mug with water, ideally with a minigame for stopping at the right moment
        return null;
    }

    public override void OnInteract() {
        base.OnInteract();
        ProduceItem();
    }

    public void Minigame(bool active, GameObject heldItem)
    {

    }
}