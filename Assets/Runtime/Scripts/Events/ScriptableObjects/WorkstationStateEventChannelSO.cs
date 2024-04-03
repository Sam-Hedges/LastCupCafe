using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// This class is used for Events that have one gameobject argument.
/// Example: A game object pick up event event, where the GameObject is the object we are interacting with.
/// </summary>

[CreateAssetMenu(menuName = "Events/Workstation State Event Channel")]
public class WorkstationStateEventChannelSO : SerializableScriptableObject
{
	public UnityAction<Workstation> OnEventRaised;
	
	public void RaiseEvent(Workstation value)
	{
		OnEventRaised?.Invoke(value);
	}
}
