using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;

namespace TeamCerber {

	public class ExampleController : BaseSpaceShipController
	{
		private BehaviorTree behaviorTree;
		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
			behaviorTree = GetComponent<BehaviorTree>();
			Hud hud = FindObjectOfType<Hud>();
			behaviorTree.SetVariableValue("Hud", hud.gameObject);
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			behaviorTree.SetVariableValue("Position", spaceship.Position);
			behaviorTree.SetVariableValue("Orientation", spaceship.Orientation);

			SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);
			float thrust = 1.0f;
			float targetOrient = spaceship.Orientation + 90.0f;
			bool needShoot = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);
			return new InputData(thrust, targetOrient, needShoot, false, false);
		}
	}

}
