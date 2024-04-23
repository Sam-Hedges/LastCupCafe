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
public abstract class Ingredient : Item {
    
    public GameObjectEventChannelSO iconInitChannel;
    public IngredientType IngredientType { get; private set; }

    private new void Awake()
    {
        base.Awake();
    }
    public void Start() {
        iconInitChannel.RaiseEvent(gameObject);
    }

    internal void SetIngredientType(IngredientType type) {
        IngredientType = type;
    }
    
}