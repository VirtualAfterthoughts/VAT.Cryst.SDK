using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Input;
using VAT.Shared.Data;
using VAT.Shared.Math;

namespace VAT.Interaction
{
    public class BoxGrip : Grip
    {
        [Header("Box Grip")]
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        private Vector3 _center = Vector3.zero;

        [SerializeField]
        private Vector3 _size = Vector3.one;

        private void Start()
        {
            if (_target == null)
            {
                _target = transform;
            }
        }

        public Transform GetTargetTransform()
        {
            if (_target == null)
            {
                _target = transform;
            }

            return _target;
        }

        public override SimpleTransform GetTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            var grabPoint = point.GetDefaultGrabPoint();
            var targetTransform = GetTargetTransform();
            var localGrabPoint = targetTransform.InverseTransformPoint(grabPoint.position);

            var face = Geometry.ClosestFace(localGrabPoint, _center, _size, Faces.EVERYTHING);
            if (face.HasValue)
            {
                var worldPoint = targetTransform.TransformPoint(face.Value.ClosestPoint(localGrabPoint));
                var worldNormal = targetTransform.TransformDirection(face.Value.normal);

                var grabRotation = Quaternion.FromToRotation(-point.GetGrabNormal(), worldNormal) * grabPoint.rotation;

                return SimpleTransform.Create(worldPoint, grabRotation);
            }

            return grabPoint;
        }

        public override SimpleTransform GetDefaultTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            var targetTransform = GetTargetTransform();

            var faces = Geometry.GetFaceInformation(_center, _size, Faces.PositiveX);
            var face = faces[0];

            var worldPoint = targetTransform.TransformPoint(face.origin);
            var worldNormal = targetTransform.TransformDirection(face.normal);

            var grabRotation = Quaternion.FromToRotation(targetTransform.right, worldNormal) * targetTransform.rotation;

            return SimpleTransform.Create(worldPoint, grabRotation);
        }

        private void OnDrawGizmosSelected()
        {
            Transform target = GetTargetTransform();
            target = target ? target : transform;
            Gizmos.matrix = target.localToWorldMatrix;

            Gizmos.DrawWireCube(_center, _size);
        }
    }
}
