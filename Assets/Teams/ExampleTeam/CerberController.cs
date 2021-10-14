using System;
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
        private float lastMineShot = -3.5f;

        private float mineSecurityDistance = .8f;
        private float asteroidSecurityDistance = 1.2f;
        private float shockwaveDistance = 1.5f;

        private bool needAvoid;
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
            float thrust = 1.0f;

            //booléens
            needShoot = (bool)behaviorTree.GetVariable("Need Shoot").GetValue();
            needMine = (bool)behaviorTree.GetVariable("Need Mine").GetValue();
            needShockwave = (bool)behaviorTree.GetVariable("Need Shockwave").GetValue();
            followPlayer = (bool)behaviorTree.GetVariable("Follow Player").GetValue();
            moveToWp = (bool)behaviorTree.GetVariable("Move to Wp").GetValue();

            SpaceShipView otherSpaceship = data.GetSpaceShipForOwner(1 - spaceship.Owner);

            CheckShockwaveDistance(spaceship, otherSpaceship);

            bool canHitEnemy = AimingHelpers.CanHit(spaceship, otherSpaceship.Position, otherSpaceship.Velocity, 0.15f);
            LayerMask mask = LayerMask.GetMask("Asteroid");
            RaycastHit2D hit2 = Physics2D.Raycast(spaceship.Position, spaceship.Velocity, 20.0f, mask);
            RaycastHit2D hit = Physics2D.Linecast(spaceship.Position, otherSpaceship.Position, mask);
            CheckHitEnemy(spaceship, otherSpaceship, canHitEnemy, hit, hit2);

            CheckHitMine(spaceship, data, out var canHitMine);
            CheckDropMine(spaceship);

            //behaviorTree.SetVariableValue("Need Mine", false);
            behaviorTree.SetVariableValue("Need Shockwave", false);

            MoveToWp(spaceship, data, hit2);

            return new InputData(thrust, targetOrient, (needShoot && canHitEnemy) || canHitMine, needMine, needShockwave);
        }

        private void CheckDropMine(SpaceShipView spaceship)
        {
            needMine = Vector2.Distance(closestWpPosition, spaceship.Position) < 0.7f &&
                             closestWpView.Owner == spaceship.Owner && Time.time - lastMineShot > 3.5f;

            if (needMine)
            {
                lastMineShot = Time.time;
            }
            behaviorTree.SetVariableValue("Need Mine", needMine);
        }

        private void CheckHitMine(SpaceShipView spaceship, GameData data, out bool canHitMine)
        {
            canHitMine = false;
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
        }

        private void CheckHitEnemy(SpaceShipView spaceship, SpaceShipView otherSpaceship, bool canHitEnemy, RaycastHit2D hit, RaycastHit2D hit2)
        {

            //if (hit2.collider != null)
            //{
            //    if (hit2.collider.CompareTag("Asteroid"))
            //    {
            //        canHitEnemy = false;
            //    }
            //}
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Asteroid"))
                {
                    canHitEnemy = false;
                }
            }
            if (needShoot && canHitEnemy)
            {
                behaviorTree.SetVariableValue("Need Shoot", false);
            }
        }

        private void CheckShockwaveDistance(SpaceShipView spaceship, SpaceShipView otherSpaceship)
        {
            if (Vector2.Distance(spaceship.Position, otherSpaceship.Position) < shockwaveDistance)
            {
                needShockwave = true;
            }
        }

        void MoveToWp(SpaceShipView spaceship, GameData data, RaycastHit2D hitVelocity)
        {
            Debug.DrawRay(spaceship.Position, (Quaternion.Euler(0, 0, targetOrient) * Vector3.right).normalized * asteroidSecurityDistance, Color.red);
            float nearestWp = float.MaxValue;

            foreach (WayPointView WPChild in data.WayPoints)
            {
                float distance = Vector2.Distance(WPChild.Position, spaceship.Position);

                if (nearestWp > distance && WPChild.Owner != spaceship.Owner)
                {
                    nearestWp = distance;
                    closestWpPosition = WPChild.Position;
                    closestWpView = WPChild;

                    moveToWp = false;
                }
            }

            if (needAvoid) needAvoid = false;
            //float closestAstDist = Single.PositiveInfinity;
            foreach (AsteroidView ast in data.Asteroids)
            {
                float dist = Vector2.Distance(spaceship.Position, ast.Position);
                bool isInShipRange = dist < ast.Radius + spaceship.Radius + asteroidSecurityDistance;
                bool willHit = hitVelocity.collider != null && hitVelocity.collider.CompareTag("Asteroid");
                if (isInShipRange && willHit) 
                {
                    needAvoid = true;
                    avoidedAstPos = ast.Position;
                    //closestAstDist = dist;
                    Debug.DrawLine(spaceship.Position, avoidedAstPos, Color.blue);
                }
            }

            if (!needAvoid)
            {
                float angle = Vector2.SignedAngle(spaceship.Velocity, closestWpPosition - spaceship.Position);
                angle = angle * 1.5f;
                angle = Mathf.Clamp(angle, -175, 175);

                targetOrient = Vector2.SignedAngle(Vector2.right, spaceship.Velocity) + angle;
            }
            else
            {
                targetOrient = Vector2.SignedAngle(Vector2.right, spaceship.Position - avoidedAstPos);
            }
        }
    }
}
