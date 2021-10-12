using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class FireShockwave : Action
    {
        public SharedFloat _energy = 1.0f;
        public SharedFloat _shockwaveEnergyCost = 0.2f;
        [SerializeField] private Shockwave shockwavePrefab;
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
            if (_energy.Value < _shockwaveEnergyCost.Value)
                return TaskStatus.Failure;

            Quaternion rotation = Quaternion.Euler(0, 0, _orientation.Value);
            Shockwave spawned = GameObject.Instantiate<Shockwave>(shockwavePrefab, _position.Value, rotation);

            spawned.SetOwner(_owner.Value);
            _energy.Value -= _shockwaveEnergyCost.Value;
            

            // Currently, the HUD script is changing the value of the sliders too : Find how to change sliders value in HUD or energy Value in SpaceShip
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



