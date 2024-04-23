public class CoffeeBeans : Ingredient
{
    private new void Awake() {
        base.Awake();
        SetIngredientType(IngredientType.CoffeeBeans);
    }
}
