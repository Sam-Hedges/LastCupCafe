using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/PlayerController Event Channel")]
public class PlayerControllerEventChannelSO : SerializableScriptableObject
{
	public UnityAction<PlayerController> OnEventRaised;
	
	public void RaiseEvent(PlayerController value)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(value);
	}
}
