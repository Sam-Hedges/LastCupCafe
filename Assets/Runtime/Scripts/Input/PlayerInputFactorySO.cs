using UnityEngine;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerInputFactory", menuName = "Factory/InputController Factory")]
public class PlayerInputFactorySO : FactorySO<InputController>
{
	public InputController prefab;

	public override InputController Create()
	{
		return Instantiate(prefab);
	}
}
