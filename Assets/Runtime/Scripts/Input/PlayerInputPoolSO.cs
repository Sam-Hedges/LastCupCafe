using UnityEngine;
using System.Linq;
using Game.Pool;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerInputPool", menuName = "Pool/InputController Pool")]
public class PlayerInputPoolSO : ComponentPoolSO<InputController>
{
	[SerializeField] private PlayerInputFactorySO factory;

	public override IFactory<InputController> Factory
	{
		get => factory;
		set => factory = value as PlayerInputFactorySO;
	}
}
