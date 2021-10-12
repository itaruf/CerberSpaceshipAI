using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class Shoot : Action
    {
        public SharedFloat _energy = 1.0f;
        public SharedFloat _shootEnergyCost = 0.2f;
        [SerializeField] private Bullet bulletPrefab;
        public SharedInt _owner;
        public SharedVector2 _position;
        public SharedFloat _orientation;
        public SharedGameObject _hud;
        private Hud hud;

        public override void OnAwake()
        {
            hud = _hud.Value.gameObject.GetComponent<Hud>();
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (_energy.Value < _shootEnergyCost.Value)
                return TaskStatus.Failure;

            Quaternion rotation = Quaternion.Euler(0, 0, _orientation.Value);
            Bullet spawned = GameObject.Instantiate<Bullet>(bulletPrefab, _position.Value, rotation) ;

            spawned.SetOwner(_owner.Value);
            _energy.Value -= _shootEnergyCost.Value;

            //if (_owner.Value == 0)
            //    hud.slider1.value = _energy.Value;
            //else if (_owner.Value == 1)
            //    hud.slider2.value = _energy.Value;

            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }

}

