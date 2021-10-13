using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class Shoot : Action
    {
        public SharedGameObject _spaceShip;
        private SpaceShip spaceShip;

        public SharedBool needShoot;

        public override void OnAwake()
        {
            //spaceShip = _spaceShip.Value.gameObject.GetComponent<SpaceShip>();

            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            //spaceShip.Shoot();
            needShoot.Value = true;

            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}

