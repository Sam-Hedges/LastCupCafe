using UnityEngine;
public class CoffeeMachine : Workstation, IMinigameInteract
{
    [SerializeField] private GameObject coffeeGroundsPrefab;
    private int charges = 0;
    private const int maxCharges = 3;

    public bool CanProcessItem(GameObject item) {
        return item.GetComponent<CoffeeBeans>() != null && charges < maxCharges;
    }

    public void Minigame(bool active, GameObject heldItem)
    {

    }
}