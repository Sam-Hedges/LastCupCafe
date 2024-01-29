using UnityEngine;
using System.Linq;
using Game.Pool;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerControllerPool", menuName = "Pool/PlayerController Pool")]
public class PlayerControllerPoolSO : ComponentPoolSO<PlayerController>
{
	[SerializeField] private PlayerControllerFactorySO factory;

	public override IFactory<PlayerController> Factory
	{
		get => factory;
		set => factory = value as PlayerControllerFactorySO;
	}
}
