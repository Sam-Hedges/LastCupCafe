public class Chocolate : Ingredient
{
    private new void Awake() {
        base.Awake();
        SetIngredientType(IngredientType.ChocolatePowder);
    }
}
