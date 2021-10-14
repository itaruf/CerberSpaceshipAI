using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class CheckClosestWP : Action
    {
        public SharedGameObject _spaceShip;
        private SpaceShip spaceShip;
        private GameData gameData;
        public SharedVector2 _closestWpPosition;
        public BehaviorTree behaviorTree;
        public override void OnAwake()
        {
            behaviorTree = GetComponent<BehaviorTree>();
            spaceShip = _spaceShip.Value.gameObject.GetComponent<SpaceShip>();
            gameData = GameManager.Instance.GetGameData();
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            //Debug.Log(spaceShip.gameObject.transform.position);
            foreach (WayPointView WPChild in gameData.WayPoints)
            {
                // On cherche constamment le WP le plus proche qui n'a pas été déjà conquis par le joueur
                if (Vector2.Distance(_closestWpPosition.Value, spaceShip.gameObject.transform.position) > Vector2.Distance(WPChild.Position, spaceShip.gameObject.transform.position) && WPChild.Owner != spaceShip.Owner)
                {
                    _closestWpPosition.Value = WPChild.Position; // On update avec les coord du WP le plus proche
                    /*Debug.Log(_closestWpPosition.Value);
                    Debug.Log(WPChild.Owner);*/
                }
            }
            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}


