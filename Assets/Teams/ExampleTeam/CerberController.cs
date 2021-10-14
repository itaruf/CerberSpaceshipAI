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
        private WayPointView closestWpView;
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

            behaviorTree.SetVariableValue("Need Mine", false);

            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);

            float thrust = 1.0f;

            //float targetOrient = spaceship.Orientation + 90.0f;


            if (followPlayer)
                targetOrient = -Mathf.Atan2(otherSpaceship.Position.x - spaceship.Position.x, otherSpaceship.Position.y - spaceship.Position.y) * Mathf.Rad2Deg + 90;
            else if (moveToWp)
            {
                float nearestWp = float.MaxValue;

                // Drop une mine quand le vaisseau conquiert un waypoint
                if (Vector2.Distance(closestWpPosition, spaceship.Position) < 0.7f && closestWpView.Owner == spaceship.Owner)
                {
                    behaviorTree.SetVariableValue("Need Mine", true);
                    //Debug.Log(behaviorTree.GetVariable("Need Mine"));
                }

                // Récupérer les coords du waypoint le plus proche
                foreach (WayPointView WPChild in data.WayPoints)
                {
                    float distance = Vector2.Distance(WPChild.Position, spaceship.Position);
                    if (nearestWp > distance && WPChild.Owner != spaceship.Owner)
                    {
                        nearestWp = distance;
                        closestWpPosition = WPChild.Position;
                        closestWpView = WPChild;
                        //Debug.Log(closestWpPosition);
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

            LayerMask mask = LayerMask.GetMask("Asteroid");

            Debug.DrawLine(spaceship.Position, spaceship.Position + spaceship.Velocity.normalized * 10.0f, Color.red); ;
            RaycastHit2D hit2 = Physics2D.Raycast(spaceship.Position, spaceship.Velocity, 20.0f, mask);

            Debug.DrawRay(spaceship.Position, (otherSpaceship.Position - spaceship.Position), Color.green);
            RaycastHit2D hit = Physics2D.Linecast(spaceship.Position, otherSpaceship.Position, mask);

            if (hit2.collider != null)
            {
                if (hit2.collider.CompareTag("Asteroid"))
                {
                    Debug.Log("Asteroid Hit");
                    canHit = false;
                }
            }

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Asteroid"))
                {
                    Debug.Log("Asteroid between");
                    canHit = false;
                }
            }

            if (needShoot && canHit)
                behaviorTree.SetVariableValue("Need Shoot", false);

            behaviorTree.SetVariableValue("Need Shockwave", false);

            return new InputData(thrust, targetOrient, needShoot && canHit, needMine, needShockwave);

        }
    }

}
