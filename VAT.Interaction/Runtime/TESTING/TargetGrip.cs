using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class TargetGrip : Grip
    {
        [Header("Target Grip")]
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        [Min(0f)]
        private float _radius = 0f;

        public float Radius { get { return _radius; } set {  _radius = value; } }

        public Transform GetTargetTransform() 
        { 
            return _target; 
        }

        public virtual float GetWorldRadius()
        {
            return transform.lossyScale.Maximum() * _radius;
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
            var target = GetTargetTransform();
            var normal = interactor.GetRigidbody().transform.up;

            return SimpleTransform.Create(target.position + normal * GetWorldRadius(), target.rotation);
        }

        private void OnDrawGizmosSelected()
        {
            Transform target = _target ? _target : transform;

            Gizmos.DrawWireSphere(target.position, GetWorldRadius());
        }
    }
}
