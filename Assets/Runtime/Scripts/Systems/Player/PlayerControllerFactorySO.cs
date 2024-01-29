using UnityEngine;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerControllerFactory", menuName = "Factory/PlayerController Factory")]
public class PlayerControllerFactorySO : FactorySO<PlayerController>
{
	public PlayerController prefab;

	public override PlayerController Create()
	{
		return Instantiate(prefab);
	}
}
