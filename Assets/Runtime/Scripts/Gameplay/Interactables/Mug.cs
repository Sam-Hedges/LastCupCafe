using System.Collections.Generic;

public class Mug : Item {
    private List<string> ingredients = new List<string>();
    public bool IsDirty { get; private set; } = false;

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