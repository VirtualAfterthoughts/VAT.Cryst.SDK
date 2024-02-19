using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class TargetGrip : Grip
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

        private void Awake()
        {
            if (_target == null)
            {
                _target = transform;
            }
        }

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            var normal = interactor.GetRigidbody().transform.up;
            return SimpleTransform.Create(_target.position + normal * GetRadius(), _target.rotation);
        }

        private void OnDrawGizmosSelected()
        {
            Transform target = _target ? _target : transform;

            Gizmos.DrawWireSphere(target.position, GetRadius());
        }
    }
}
