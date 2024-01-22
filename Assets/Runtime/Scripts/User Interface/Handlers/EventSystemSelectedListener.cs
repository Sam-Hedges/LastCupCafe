using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Listens to a GameObjectEventChannelSO and sets the selected gameobject on the EventSystem.
/// </summary>
public class EventSystemSelectedListener : MonoBehaviour
{
    [Header("Listening to")]
    [SerializeField] private GameObjectEventChannelSO selectedGameObjectEventChannel;
    private EventSystem eventSystem;
    
    private void Awake() {
        eventSystem = GetComponent<EventSystem>();
    }
    
    private void OnEnable() {
        selectedGameObjectEventChannel.OnEventRaised += SetSelectedGameObject;
    }
    
    private void OnDisable() {
        selectedGameObjectEventChannel.OnEventRaised -= SetSelectedGameObject;
    }

    private void SetSelectedGameObject(GameObject go)
    {
        eventSystem.SetSelectedGameObject(go);
    }
}