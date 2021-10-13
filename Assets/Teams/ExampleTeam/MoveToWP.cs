using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime;
using TeamCerber;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class MoveToWP : Action
    {
        public SharedGameObject _spaceShip;
        private SpaceShip spaceShip;
        public BehaviorTree behaviorTree;
        private SharedVector2 closestWpPosition;

        public override void OnAwake()
        {
            behaviorTree = GetComponent<BehaviorTree>();
            spaceShip = _spaceShip.Value.gameObject.GetComponent<SpaceShip>();
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            closestWpPosition = (SharedVector2) behaviorTree.GetVariable("Closest Wp Position");
            //Debug.Log(closestWpPosition.Value);

            // On fait bouger le vaisseau vers le wp le plus proche
            if (Vector2.SqrMagnitude(spaceShip.Position - closestWpPosition.Value) < 0.1f)
            {
                return TaskStatus.Success;
            }
   
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}


