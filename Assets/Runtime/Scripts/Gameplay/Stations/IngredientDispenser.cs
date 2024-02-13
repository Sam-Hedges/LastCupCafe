using UnityEngine;
public class IngredientDispenser : Workstation, IProduceItem {
    [SerializeField] private GameObject ingredientPrefab; // Set this in the inspector to the appropriate item

    public GameObject ProduceItem() {
        return Instantiate(ingredientPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}