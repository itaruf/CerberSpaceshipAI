using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;

namespace TeamCerber {

	public class CerberController : BaseSpaceShipController
	{
		private BehaviorTree behaviorTree;

		private bool needShoot;
		private bool needMine;
		private bool needShockwave;

		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
			behaviorTree = GetComponent<BehaviorTree>();

			behaviorTree.SetVariableValue("Spaceship", GameManager.Instance.GetSpaceShipForController(this).gameObject);
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
            needShoot = (bool)behaviorTree.GetVariable("Need Shoot").GetValue();
			needMine = (bool)behaviorTree.GetVariable("Need Mine").GetValue();
			needShockwave = (bool)behaviorTree.GetVariable("Need Shockwave").GetValue();

			SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);
			float thrust = 1.0f;
			float targetOrient = spaceship.Orientation + 90.0f;

			bool canHit = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);

			if (needShoot && canHit)
				behaviorTree.SetVariableValue("Need Shoot", false);

			behaviorTree.SetVariableValue("Need Mine", false);
			behaviorTree.SetVariableValue("Need Shockwave", false);

			return new InputData(thrust, targetOrient, needShoot && canHit, needMine, needShockwave);

		}
	}

}
