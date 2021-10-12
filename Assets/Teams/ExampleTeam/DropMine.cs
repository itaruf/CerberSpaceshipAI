using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    public class DropMine : Action
    {
        public SharedFloat _energy = 1.0f;
        public SharedFloat _mineEnergyCost = 0.2f;
        [SerializeField] private GameObject minePrefab;
        public SharedVector2 _position;
        public SharedGameObject _hud;
        private Hud hud;

        public override void OnAwake()
        {
            hud = _hud.Value.gameObject.GetComponent<Hud>();
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (_energy.Value < _mineEnergyCost.Value)
                return TaskStatus.Failure;

            GameObject.Instantiate(minePrefab, _position.Value, Quaternion.identity);
            _energy.Value -= _mineEnergyCost.Value;

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


