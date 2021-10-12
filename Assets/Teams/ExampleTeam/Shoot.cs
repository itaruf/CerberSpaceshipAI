using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class Shoot : Action
    {
        //public GameData data;
        //public SpaceShip spaceship;
        public SharedFloat _energy = 1.0f;
        public SharedFloat _shootEnergyCost = 0.2f;
        [SerializeField] private Bullet bulletPrefab;
        public SharedInt _owner;
        //public SharedTransform _transform;
        public SharedVector2 _position;
        public SharedVector2 _orientation;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (_energy.Value < _shootEnergyCost.Value)
                return TaskStatus.Failure;
            Bullet spawned = GameObject.Instantiate<Bullet>(bulletPrefab, _position.Value, transform.rotation) ;
            spawned.SetOwner(_owner.Value);
            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}

