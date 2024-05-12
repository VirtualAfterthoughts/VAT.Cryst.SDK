using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class SphereGrip : TargetGrip
    {
        protected override IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new SphereGripJoint(GetTargetTransform(), GetWorldRadius());
        }

        private void Reset()
        {
            if (gameObject.TryGetComponent<SphereCollider>(out var sphereCollider))
            {
                Radius = sphereCollider.radius;
            }
            else
            {
                Radius = 0.5f;
            }
        }

        public override SimpleTransform GetTargetInWorld(PalmPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var grabPoint = point.GetHostTransform().Transform(GetTargetInInteractor(point, pose));
            var direction = ((Vector3)grabPoint.position - target.position).normalized;

            var grabRotation = Quaternion.FromToRotation(-point.GetNormal(), direction) * grabPoint.rotation;

            return SimpleTransform.Create(target.position + direction * GetWorldRadius(), grabRotation);
        }

        public override SimpleTransform GetDefaultTargetInWorld(PalmPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var direction = target.right;

            float dot = point.GetThumbDot();

            direction *= dot;

            var grabRotation = target.rotation;

            return SimpleTransform.Create(target.position + direction * GetWorldRadius(), grabRotation);
        }
    }
}
