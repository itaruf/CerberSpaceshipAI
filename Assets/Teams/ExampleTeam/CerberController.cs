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
        private bool needAvoid;
        private bool followPlayer;
        private bool moveToWp;

        private Vector2 closestWpPosition;
        private float mineSecurityDistance = .5f;
        private float asteroidSecurityDistance = 1.5f;
        private bool isAvoiding = false;
        private Vector2 avoidedAstPos;

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
            needAvoid = (bool)behaviorTree.GetVariable("Need Avoid").GetValue();
            followPlayer = (bool)behaviorTree.GetVariable("Follow Player").GetValue();
            moveToWp = (bool)behaviorTree.GetVariable("Move to Wp").GetValue();

            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);

            float thrust = 1.0f;

            //float targetOrient = spaceship.Orientation + 90.0f;

            if (followPlayer)
            {
                targetOrient = -Mathf.Atan2(otherSpaceship.Position.x - spaceship.Position.x, otherSpaceship.Position.y - spaceship.Position.y) * Mathf.Rad2Deg + 90;
            }
            else if (moveToWp)
            {
                MoveToWp(spaceship, data);
            }
            else if (!followPlayer && !moveToWp)
            {
                //targetOrient = spaceship.Orientation;
            }

            bool canHitEnemy = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);

            if (needShoot && canHitEnemy)
            {
                behaviorTree.SetVariableValue("Need Shoot", false);
            }

            bool canHitMine = false;
            foreach (MineView mine in data.Mines)
            {
                bool isHittable = AimingHelpers.CanHit(spaceship, mine.Position, 1f);
                bool isCloseToWp = Vector2.Distance(mine.Position, closestWpPosition) < mine.BulletHitRadius + mineSecurityDistance;
                if (isCloseToWp && isHittable)
                {
                    canHitMine = true;
                    break;
                }
            }

            behaviorTree.SetVariableValue("Need Mine", false);
            behaviorTree.SetVariableValue("Need Shockwave", false);

            return new InputData(thrust, targetOrient, (needShoot && canHitEnemy) || canHitMine, needMine, needShockwave);
        }

        void MoveToWp(SpaceShipView spaceship, GameData data)
        {
            Debug.DrawRay(spaceship.Position, (Quaternion.Euler(0, 0, targetOrient) * Vector3.right).normalized * asteroidSecurityDistance, Color.red);
            Debug.Log(targetOrient);
            float nearestWp = float.MaxValue;

            foreach (WayPointView WPChild in data.WayPoints)
            {
                float distance = Vector2.Distance(WPChild.Position, spaceship.Position);

                if (nearestWp > distance && WPChild.Owner != spaceship.Owner)
                {
                    nearestWp = distance;
                    closestWpPosition = WPChild.Position;

                    moveToWp = false;
                }
            }

            if (needAvoid) needAvoid = false;
            foreach (AsteroidView ast in data.Asteroids)
            {
                if (Vector2.Distance(spaceship.Position, ast.Position) <
                    ast.Radius + spaceship.Radius + asteroidSecurityDistance)
                {
                    needAvoid = true;
                    avoidedAstPos = ast.Position;
                }
            }
            if (!needAvoid) isAvoiding = false;

            if (needAvoid && !isAvoiding)
            {
                Debug.Log("vesqui");
                isAvoiding = true;
            }
            if (!isAvoiding)
            {
                float angle = Vector2.SignedAngle(spaceship.Velocity, closestWpPosition - spaceship.Position);
                angle = angle * 1.5f;
                angle = Mathf.Clamp(angle, -170, 170);

                targetOrient = Vector2.SignedAngle(Vector2.right, spaceship.Velocity) + angle;
            }
            else
            {
                targetOrient = Vector2.SignedAngle(Vector2.right, spaceship.Position - avoidedAstPos);
            }
        }
    }
}
