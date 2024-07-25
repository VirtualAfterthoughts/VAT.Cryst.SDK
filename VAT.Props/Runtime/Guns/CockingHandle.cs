using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction;

namespace VAT.Props
{
    public class CockingHandle : MonoBehaviour
    {
        [SerializeField]
        private Grip[] _grips = new Grip[0];

        [SerializeField]
        private GunBolt _bolt = null;

        private IInteractor _mainInteractor = null;

        private Vector3 _lastLocalPosition;

        private void OnEnable()
        {
            foreach (var grip in _grips)
            {
                grip.OnAttached += OnAttached;
                grip.OnDetached += OnDetached;
            }
        }

        private void OnDisable()
        {
            foreach (var grip in _grips)
            {
                grip.OnAttached -= OnAttached;
                grip.OnDetached -= OnDetached;
            }
        }

        private void OnAttached(IInteractor interactor)
        {
            if (_mainInteractor == null)
            {
                _mainInteractor = interactor;

                _lastLocalPosition = GetLocalPosition(interactor);
            }
        }

        private Vector3 GetLocalPosition(IInteractor interactor)
        {
            var targetData = interactor.GetTargetData();
            var targetInWorld = targetData.rig.Transform(targetData.targetInRig);

            return transform.InverseTransformPoint(targetInWorld.position);
        }

        private void OnDetached(IInteractor interactor)
        {
            if (_mainInteractor == interactor)
            {
                _mainInteractor = null;
                _lastLocalPosition = Vector3.zero;
            }
        }

        private void LateUpdate()
        {
            if (_bolt.IsLocked)
            {
                return;
            }

            if (_mainInteractor == null)
            {
                ApplySpring();
                return;
            }

            var localPosition = GetLocalPosition(_mainInteractor);

            var difference = localPosition - _lastLocalPosition;

            var dot = Vector3.Dot(difference, Vector3.back) / 0.2f;
            float newPercent = Mathf.Clamp01(_bolt.PulledPercent + dot);
            _bolt.UpdateBolt(newPercent);

            _lastLocalPosition = localPosition;
        }

        private void ApplySpring()
        {
            float springPercent = Mathf.MoveTowards(_bolt.PulledPercent, 0f, Time.deltaTime * 40f);
            _bolt.UpdateBolt(springPercent);
        }
    }
}
