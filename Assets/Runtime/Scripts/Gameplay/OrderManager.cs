using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO registerOrderChannel;
    [SerializeField] private GameObjectEventChannelSO queryOrderFulfillmentChannel;
    [SerializeField] private IntEventChannelSO orderFulfilledChannel;
    [SerializeField] private GameObject orderPrefab;

    private List<Order> _orders;

    private void Awake()
    {
        registerOrderChannel.OnEventRaised += RegisterOrder;
        queryOrderFulfillmentChannel.OnEventRaised += QueryFulfillment;
        _orders = new List<Order>();
    }

    public void RemoveOrderFromList(Order order)
    {
        _orders.Remove(order);
        Destroy(order.gameObject);
    }

    private void RegisterOrder()
    {
        var order = Instantiate(orderPrefab, transform, false).GetComponent<Order>();
        order.manager = this;
        _orders.Add(order);
    }

    private void QueryFulfillment(GameObject go)
    {
        List<IngredientType> ingredients = go.GetComponent<Mug>().ingredients;
        // Check each order to find a match
        foreach (Order order in _orders)
        {
            if (AreIngredientsEqual(ingredients, order.ingredients))
            {
                // Fulfill the order
                orderFulfilledChannel.RaiseEvent(ingredients.Count);
                Debug.Log("Order fulfilled!");
                RemoveOrderFromList(order);
                break; // Exit the loop after fulfilling the order
            }
        }
    }

    private bool AreIngredientsEqual(List<IngredientType> ingredients1, List<IngredientType> ingredients2)
    {
        // Convert lists to dictionaries to count occurrences of each ingredient
        var ingredientCount1 = ingredients1.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var ingredientCount2 = ingredients2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        // Check if dictionaries have the same keys with the same counts
        return ingredientCount1.Count == ingredientCount2.Count && !ingredientCount1.Except(ingredientCount2).Any();
    }
}