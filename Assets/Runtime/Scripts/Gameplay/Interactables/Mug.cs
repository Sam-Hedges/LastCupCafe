using System.Collections.Generic;
using System.Diagnostics;

public class Mug : Item {
    public List<IngredientType> ingredients = new List<IngredientType>();
    public bool IsDirty { get; private set; } = true;

    public void AddIngredient(IngredientType ingredient) {
        if (!ingredients.Contains(ingredient)) {
            ingredients.Add(ingredient);
        }
    }

    public void Clean() {
        IsDirty = false;
    }
}