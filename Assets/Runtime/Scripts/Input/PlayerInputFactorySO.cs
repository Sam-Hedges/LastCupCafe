using UnityEngine;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerInputFactory", menuName = "Factory/PlayerInputHandler Factory")]
public class PlayerInputFactorySO : FactorySO<PlayerInputHandler>
{
	public PlayerInputHandler prefab;

	public override PlayerInputHandler Create()
	{
		return Instantiate(prefab);
	}
}
