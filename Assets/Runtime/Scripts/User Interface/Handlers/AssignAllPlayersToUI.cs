using UnityEngine;
using UnityEngine.InputSystem.UI;

public class AssignAllPlayersToUI : MonoBehaviour {

    [Header("Canvas")]
    [Tooltip("The canvas that the player cards will be parented to")]
    [SerializeField]
    private Canvas canvas;
    
    [Header("Listening on Channels")]
    [Tooltip("Receives a reference of an Input Controller when it is spawned")]
    [SerializeField]
    private GameObjectEventChannelSO inputControllerInstancedChannel;

    private void OnEnable() {
        inputControllerInstancedChannel.OnEventRaised += InputControllerInstanced;
    }

    private void OnDisable() {
        inputControllerInstancedChannel.OnEventRaised -= InputControllerInstanced;
    }

    private void InputControllerInstanced(GameObject go) {
        MultiplayerEventSystem eventSystem = go.GetComponent<MultiplayerEventSystem>();
        eventSystem.playerRoot = canvas.gameObject;

        // Set first selected to the first button found in the canvas
        eventSystem.firstSelectedGameObject = canvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
    }
}
