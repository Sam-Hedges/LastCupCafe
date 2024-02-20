using System;
using UnityEngine;

public enum IngredientType {
    Mug,
    Milk,
    CoffeeBeans,
    CoffeeGrounds,
    ChocolatePowder,
    CaramelSyrup,
}
public class Ingredient : Item {
    
    public IngredientType IngredientType { get; private set; }
    
}