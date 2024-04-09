using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Mug : Item {
    public List<IngredientType> ingredients = new List<IngredientType>();
    public bool IsDirty { get; private set; } = true;

    public List<IngredientType> DrinkType1 = new List<IngredientType>();

    public void AddIngredient(IngredientType ingredient) {
        if (!ingredients.Contains(ingredient)) {
            ingredients.Add(ingredient);
        }
        AsignDrinkID();
    }

    public void Clean() {
        IsDirty = false;
    }

    //checks the ingredient list and asigns an ID if it matches a predetermined list
    public void AsignDrinkID()
    {
        if (ingredients.SequenceEqual(DrinkType1))
        {
            UnityEngine.Debug.Log("HELLO");
        }
    }
}