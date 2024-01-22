using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class GenericGrip : Grip
    {
        private Collider _testCollider;

        protected override void Awake()
        {
            base.Awake();

            _testCollider = GetComponent<Collider>();
        }

        public override SimpleTransform GetTargetInWorld(IGrabber grabber)
        {
            var grabPoint = grabber.GetGrabPoint();
            var point = _testCollider.ClosestPoint(grabPoint.position);

            var normal = -(point - (Vector3)grabPoint.position);
            normal.Normalize();

            var direction = (point - (Vector3)grabPoint.position);
            Ray ray = new(grabPoint.position, direction.normalized);
            if (_testCollider.Raycast(ray, out var hitInfo, float.PositiveInfinity))
            {
                normal = hitInfo.normal;
                point = hitInfo.point;
            }

            var forward = (Vector3)grabPoint.forward;
            var up = (Vector3)grabPoint.up;
            Vector3.OrthoNormalize(ref normal, ref forward, ref up);

            return SimpleTransform.Create(point, Quaternion.LookRotation(forward, up));
        }
    }
}
