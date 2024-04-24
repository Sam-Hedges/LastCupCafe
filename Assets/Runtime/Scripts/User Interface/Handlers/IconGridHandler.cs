using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconGridHandler : MonoBehaviour
{
    [SerializeField] private GameObject baseIconPrefab;

    [Header("Ingredient UI Prefabs")] [SerializeField]
    private GameObject milkIcon;

    [SerializeField] private GameObject coffeeBeansIcon;
    [SerializeField] private GameObject coffeeGroundsIcon;
    [SerializeField] private GameObject chocolateIcon;
    [SerializeField] private GameObject caramelIcon;
    [SerializeField] private GameObject waterIcon;


    public void AddIngredientIcon(IngredientType ingredientType)
    {
        var baseIcon = Instantiate(baseIconPrefab, transform, false);
        
        switch (ingredientType)
        {
            case IngredientType.Milk:
                Instantiate(milkIcon, baseIcon.transform, false);
                return;
            case IngredientType.Espresso:
            case IngredientType.CoffeeBeans:
                Instantiate(coffeeBeansIcon, baseIcon.transform, false);
                return;
            case IngredientType.CoffeeGrounds:
                Instantiate(coffeeGroundsIcon, baseIcon.transform, false);
                return;
            case IngredientType.ChocolatePowder:
                Instantiate(chocolateIcon, baseIcon.transform, false);
                return;
            case IngredientType.CaramelSyrup:
                Instantiate(caramelIcon, baseIcon.transform, false);
                return;
            case IngredientType.Water:
                Instantiate(waterIcon, baseIcon.transform, false);
                return;
        }
    }
}