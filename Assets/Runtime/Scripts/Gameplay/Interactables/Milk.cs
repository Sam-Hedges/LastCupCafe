public class Milk : Ingredient
{
    private new void Awake() {
        base.Awake();
        SetIngredientType(IngredientType.Milk);
    }
}
