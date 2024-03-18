using System.Collections.Generic;
using System.Diagnostics;

public class Mug : Item {
    public List<string> ingredients = new List<string>();
    public bool IsDirty { get; private set; } = true;

    public void AddIngredient(string ingredient) {
        if (!ingredients.Contains(ingredient)) {
            ingredients.Add(ingredient);
        }
    }

    public bool HasIngredient(string ingredient) {
        return ingredients.Contains(ingredient);
    }

    public void Clean() {
        IsDirty = false;
    }
}