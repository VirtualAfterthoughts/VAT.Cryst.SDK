using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Math;

namespace VAT.Interaction
{
    public class BoxGrip : TargetGrip
    {
        [Header("Box Grip")]
        [SerializeField]
        private Vector3 _center = Vector3.zero;

        [SerializeField]
        private Vector3 _size = Vector3.one;

        public override SimpleTransform GetTargetInWorld(IGrabberPoint grabberPoint)
        {
            var grabPoint = grabberPoint.GetDefaultGrabPoint();
            var targetTransform = GetTargetTransform();
            var localGrabPoint = targetTransform.InverseTransformPoint(grabPoint.position);

            var face = Geometry.ClosestFace(localGrabPoint, _center, _size, Faces.EVERYTHING);
            if (face.HasValue)
            {
                var worldPoint = targetTransform.TransformPoint(face.Value.ClosestPoint(localGrabPoint));
                var worldNormal = targetTransform.TransformDirection(face.Value.normal);

                var grabRotation = Quaternion.FromToRotation(-grabberPoint.GetGrabNormal(), worldNormal) * grabPoint.rotation;

                return SimpleTransform.Create(worldPoint, grabRotation);
            }

            return grabPoint;
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
