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
        private Vector3 _axis = Vector3.forward;

        [SerializeField]
        private Vector3 _secondaryAxis = Vector3.up;

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

        public Quaternion GetAxisOffset(IInteractor interactor)
        {
            var grabPoint = interactor.GetGrabPoint();
            var normal = interactor.GetRigidbody().transform.up;

            float dot = Vector3.Dot(grabPoint.right, normal);

            var lookRotation = Quaternion.LookRotation(_axis, _secondaryAxis);
            if (dot < 0f)
                lookRotation = Quaternion.Inverse(lookRotation);

            return lookRotation;
        }

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            var target = GetTargetTransform();
            var normal = interactor.GetRigidbody().transform.up;

            var rotation = GetAxisOffset(interactor) * target.rotation;

            return SimpleTransform.Create(target.position + normal * GetWorldRadius(), rotation);
        }

        private void OnValidate()
        {
            _axis.Normalize();
            _secondaryAxis.Normalize();
            Vector3.OrthoNormalize(ref _axis, ref _secondaryAxis);
        }

        private void OnDrawGizmosSelected()
        {
            Transform target = _target ? _target : transform;

            var worldRadius = GetWorldRadius();

            Gizmos.DrawWireSphere(target.position, worldRadius);

            var axis = target.TransformDirection(_axis);
            var secondaryAxis = target.TransformDirection(_secondaryAxis);

            using (var color = TempGizmoColor.Create())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(target.position, target.position + axis * worldRadius * 2f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + secondaryAxis * worldRadius * 2f);
            }
        }
    }
}
