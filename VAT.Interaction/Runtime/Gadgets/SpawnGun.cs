using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;
using VAT.Pooling;

namespace VAT.Interaction
{
    public class SpawnGun : MonoBehaviour
    {
        public Transform firePoint;
        public Grip triggerGrip;

        private IInteractor _mainInteractor = null;

        private void OnEnable()
        {
            triggerGrip.OnAttached += OnTriggerGripAttached;
            triggerGrip.OnDetached += OnTriggerGripDetached;
        }

        private void OnDisable()
        {
            triggerGrip.OnAttached -= OnTriggerGripAttached;
            triggerGrip.OnDetached -= OnTriggerGripDetached;
        }

        private void OnTriggerGripAttached(IInteractor interactor)
        {
            if (_mainInteractor == null)
            {
                _mainInteractor = interactor;
            }
        }

        private void OnTriggerGripDetached(IInteractor interactor)
        {
            if (_mainInteractor == interactor)
            {
                _mainInteractor = null;
            }
        }

        private bool _wasPressingTrigger = false;

        private void Update()
        {
            if (_mainInteractor != null)
            {
                var controller = _mainInteractor.GetHandOrNull().GetInputControllerOrNull();
                var trigger = controller.GetTriggerOrNull();
                var axis = trigger.GetAxis();

                bool pressing = axis > 0.7f;

                if (pressing && !_wasPressingTrigger)
                {
                    Fire();
                }

                _wasPressingTrigger = pressing;
            }
        }

        private void Fire()
        {
            if (Physics.Raycast(firePoint.position, firePoint.forward, out var hitInfo, 10f, ~0, QueryTriggerInteraction.Ignore))
            {
                var spawnable = new Spawnable(SpawnUI.SelectedSpawnable);

                AssetSpawner.Register(spawnable);

                AssetSpawner.Spawn(new AssetSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    position = hitInfo.point,
                });
            }
        }
    }
}
