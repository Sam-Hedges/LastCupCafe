using UnityEngine;

/// <summary>
/// Broadcasts the selected gameobject to the selectedGameObjectEventChannel.
/// </summary>
public class EventSystemSelectedBroadcaster : MonoBehaviour
{
    [Header("Broadcasting on")]
    [SerializeField] private GameObjectEventChannelSO selectedGameObjectEventChannel;
    public void SetSelectedGameObject(GameObject go)
    {
        selectedGameObjectEventChannel.RaiseEvent(go);
    }
}