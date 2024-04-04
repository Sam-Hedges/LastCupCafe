using System;
using UnityEngine;

public enum IngredientType {
    Milk,
    CoffeeBeans,
    CoffeeGrounds,
    ChocolatePowder,
    CaramelSyrup,
    Water
}
public class Ingredient : Item {
    
    public IngredientType IngredientType { get; private set; }
    public Sprite Icon;

}