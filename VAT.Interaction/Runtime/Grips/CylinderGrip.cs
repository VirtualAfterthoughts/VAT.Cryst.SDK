using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class CylinderGrip : TargetGrip
    {
        [Header("Cylinder Grip")]
        [SerializeField]
        [Min(0f)]
        private float _height = 2f;

        public override float GetWorldRadius()
        {
            return Radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        }

        public float GetWorldHeight()
        {
            return _height * transform.lossyScale.y;
        }

        protected override IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new CylinderGripJoint(GetTargetTransform(), GetWorldRadius(), GetWorldHeight());
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
            {
                Radius = capsuleCollider.radius;
                _height = capsuleCollider.height;
            }
            else
            {
                Radius = 0.5f;
                _height = 2f;
            }
        }

        public override SimpleTransform GetTargetInWorld(IGrabberPoint grabberPoint)
        {
            var target = GetTargetTransform();
            var grabPoint = grabberPoint.GetDefaultGrabPoint();

            var distance = ((Vector3)grabPoint.position - target.position);
            var relativeDistance = target.InverseTransformDirection(distance);

            float realHeight = GetWorldHeight() * 0.5f;
            float upOffset = Mathf.Clamp(relativeDistance.y, -realHeight, realHeight);

            var fixedDirection = relativeDistance;
            fixedDirection.y = 0f;
            var direction = target.TransformDirection(fixedDirection.normalized);

            var grabRotation = Quaternion.FromToRotation(-grabberPoint.GetGrabNormal(), direction) * grabPoint.rotation;

            return SimpleTransform.Create(target.position + target.up * upOffset, grabRotation);
        }
    }
}
