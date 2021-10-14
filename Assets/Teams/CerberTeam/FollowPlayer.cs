using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class FollowPlayer : Action
    {
        public SharedBool followPlayer;

        public override void OnAwake()
        {
            //spaceShip = _spaceShip.Value.gameObject.GetComponent<SpaceShip>();

            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            //spaceShip.Shoot();
            followPlayer.Value = true;

            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}


