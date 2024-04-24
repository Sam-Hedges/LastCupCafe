using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Order : MonoBehaviour
{
    public OrderManager manager;
    public List<IngredientType> ingredients = new List<IngredientType>();
    [SerializeField] private Timer timer;
    [SerializeField] private IconGridHandler iconGridHandler;

    private void Start()
    {
        timer.OnTimerCompleteAction += DeleteOrder;
        SetRandomIngredients();
    }

    private void SetRandomIngredients()
    {
        int numberOfIngredients = Random.Range(1, 4);

        for (int i = 0; i < numberOfIngredients; i++)
        {
            IngredientType type = GetRandomIngredientType();
            iconGridHandler.AddIngredientIcon(type);
            ingredients.Add(type);
        }

    }
    
    private void DeleteOrder()
    {
        manager.RemoveOrderFromList(this);
    }

    private IngredientType GetRandomIngredientType()
    {
        while (true)
        {
            IngredientType type = (IngredientType)Random.Range(0, 7);
            if (ingredients.Contains(type)) continue;
            switch (type)
            {
                case IngredientType.CaramelSyrup:
                    if (ingredients.Contains(IngredientType.ChocolatePowder)) continue;
                    return type;
                case IngredientType.ChocolatePowder:
                    if (ingredients.Contains(IngredientType.CaramelSyrup)) continue;
                    return type;
                case IngredientType.CoffeeBeans:
                case IngredientType.CoffeeGrounds:
                    continue;
                default:
                    return type;
            }

        }
        
    }
}
