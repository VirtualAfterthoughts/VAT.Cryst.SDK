using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class SphereGrip : Grip
    {
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        [Min(0f)]
        private float _radius = 0f;

        public float GetRadius()
        {
            return transform.localScale.Maximum() * _radius;
        }

        protected override IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new SphereGripJoint(_target, GetRadius());
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<SphereCollider>(out var sphereCollider))
            {
                _radius = sphereCollider.radius;
            }
            else
            {
                _radius = 0.5f;
            }
        }

        private void Awake()
        {
            if (_target == null)
            {
                _target = transform;
            }
        }

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            var grabPoint = interactor.GetGrabPoint();
            var direction = ((Vector3)grabPoint.position - _target.position).normalized;

            var grabRotation = Quaternion.FromToRotation(interactor.GetRigidbody().transform.up, direction) * grabPoint.rotation;

            return SimpleTransform.Create(_target.position + direction * GetRadius(), grabRotation);
        }

        private void OnDrawGizmosSelected()
        {
            Transform target = _target ? _target : transform;

            Gizmos.DrawWireSphere(target.position, GetRadius());
        }
    }
}
