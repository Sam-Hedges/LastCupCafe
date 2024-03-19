using UnityEngine;

/// <summary>
/// Broadcasts the selected gameobject to the selectedGameObjectAnchor.
/// </summary>
public class UserInterfaceSelectedElementBroadcaster : MonoBehaviour
{
    [Header("Game Object")]
    [SerializeField] private GameObject firstSelectedGameObject;
    
    [Header("Broadcasting on")]
    [SerializeField] private GameObjectAnchor selectedGameObjectAnchor;
    
    public void Awake()
    {
        selectedGameObjectAnchor.Provide(firstSelectedGameObject);
    }
    
    public void ProvideUserInterfaceElement(GameObject go)
    {
        selectedGameObjectAnchor.Provide(go);
    }
}