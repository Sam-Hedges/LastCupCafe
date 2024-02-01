using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// This class is used for Events that have no arguments (Example: Exit game event)
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannelSO : SerializableScriptableObject {
    public UnityAction OnEventRaised;

    public void RaiseEvent() {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }

    public void RaiseEvent(PlayerInput obj) {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }
}