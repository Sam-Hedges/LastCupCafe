using UnityEngine;
using System.Linq;
using Game.Pool;
using Game.Factory;

[CreateAssetMenu(fileName = "NewPlayerInputPool", menuName = "Pool/PlayerInputHandler Pool")]
public class PlayerInputPoolSO : ComponentPoolSO<PlayerInputHandler>
{
	[SerializeField] private PlayerInputFactorySO factory;

	public override IFactory<PlayerInputHandler> Factory
	{
		get => factory;
		set => factory = value as PlayerInputFactorySO;
	}
}
