using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
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

#if UNITY_EDITOR
        [Header("Visualizer")]
        [SerializeField]
        [Range(-1f, 1f)]
        private float _visualizerSlide = 0f;
#endif

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

        public override SimpleTransform GetTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var grabPoint = point.GetDefaultGrabPoint();

            var distance = ((Vector3)grabPoint.position - target.position);
            var relativeDistance = target.InverseTransformDirection(distance);

            float realHeight = GetWorldHeight() * 0.5f;
            float upOffset = Mathf.Clamp(relativeDistance.y, -realHeight, realHeight);

            var fixedDirection = relativeDistance;
            fixedDirection.y = 0f;
            var direction = target.TransformDirection(fixedDirection.normalized);

            var grabRotation = Quaternion.FromToRotation(-point.GetGrabNormal(), direction) * grabPoint.rotation;
            Vector3 grabUp = grabRotation * Vector3.up;
            Vector3 targetUp = target.up * Mathf.Sign(Vector3.Dot(target.up, grabUp));
            grabRotation = Quaternion.FromToRotation(grabUp, targetUp) * grabRotation;

            return SimpleTransform.Create(target.position + target.up * upOffset + direction * GetWorldRadius(), grabRotation);
        }

        public override SimpleTransform GetDefaultTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();

            var direction = target.right;

            var grabRotation = target.rotation;
            var grabPosition = target.position;

            var grabPoint = point.GetDefaultGrabPoint();
            var normal = -point.GetGrabNormal();

            float dot = Vector3.Dot(grabPoint.right, normal);

            direction *= dot;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                grabPosition += _visualizerSlide * GetWorldHeight() * 0.5f * target.up;
            }
#endif

            return SimpleTransform.Create(grabPosition + direction * GetWorldRadius(), grabRotation);
        }
    }
}
