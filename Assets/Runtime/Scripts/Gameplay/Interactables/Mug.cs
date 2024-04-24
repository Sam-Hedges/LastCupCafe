using System.Collections.Generic;
using System.Diagnostics;

public class Mug : Item {
    public List<IngredientType> ingredients = new List<IngredientType>();
    
    public GameObjectEventChannelSO iconInitChannel;
    public bool IsDirty { get; private set; } = true;

    private IconGridHandler _handler;

    private new void Awake()
    {
        base.Awake();
        
        iconInitChannel.RaiseEvent(gameObject);
    }

    public void AddIngredient(IngredientType ingredient) {
        if (!ingredients.Contains(ingredient)) {
            ingredients.Add(ingredient);
            _handler.AddIngredientIcon(ingredient);
        }
    }

    public void SetIconGridHandler(IconGridHandler handler)
    {
        _handler = handler;
    }

    public void Clean() {
        IsDirty = false;
    }
}