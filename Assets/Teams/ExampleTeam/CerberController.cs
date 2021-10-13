using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;

namespace TeamCerber
{

    public class CerberController : BaseSpaceShipController
    {
        private BehaviorTree behaviorTree;

        private bool needShoot;
        private bool needMine;
        private bool needShockwave;
        private bool followPlayer;
        private bool moveToWp;

        private Vector2 closestWpPosition;

        float targetOrient;
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
            followPlayer = (bool)behaviorTree.GetVariable("Follow Player").GetValue();
            moveToWp = (bool)behaviorTree.GetVariable("Move to Wp").GetValue();

            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);

            float thrust = 1.0f;

            //float targetOrient = spaceship.Orientation + 90.0f;

            
            if (followPlayer)
                targetOrient = -Mathf.Atan2(otherSpaceship.Position.x - spaceship.Position.x, otherSpaceship.Position.y - spaceship.Position.y) * Mathf.Rad2Deg + 90;
            else if (moveToWp)
            {
                float nearestWp = float.MaxValue;

                foreach (WayPointView WPChild in data.WayPoints)
                {
                    float distance = Vector2.Distance(WPChild.Position, spaceship.Position);
                    if (nearestWp > distance && WPChild.Owner != spaceship.Owner)
                    {
                        nearestWp = distance;
                        closestWpPosition = WPChild.Position;
                        Debug.Log(closestWpPosition);
                    }
                }

                float angle = Vector2.SignedAngle(spaceship.Velocity, closestWpPosition - spaceship.Position);
                angle = angle * 1.5f;
                float angle2 = Vector2.SignedAngle(Vector2.right, spaceship.Velocity) + angle;

                targetOrient = Vector2.SignedAngle(Vector2.right, spaceship.Velocity) + angle;
            }
            else if (!followPlayer && !moveToWp)
                targetOrient = spaceship.Orientation;


            bool canHit = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);

            if (needShoot && canHit)
                behaviorTree.SetVariableValue("Need Shoot", false);

            behaviorTree.SetVariableValue("Need Mine", false);
            behaviorTree.SetVariableValue("Need Shockwave", false);

            return new InputData(thrust, targetOrient, needShoot && canHit, needMine, needShockwave);

        }
    }

}
