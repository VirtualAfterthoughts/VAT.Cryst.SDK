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

            var forward = (Vector3)grabPoint.forward;
            Vector3.OrthoNormalize(ref normal, ref forward);

            return SimpleTransform.Create(point, Quaternion.LookRotation(forward, normal));
        }
    }
}
