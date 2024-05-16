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

        public override SimpleTransform CalculateTargetInHost(PalmPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var grabPoint = point.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(point, pose));
            var direction = ((Vector3)grabPoint.position - target.position).normalized;

            var grabRotation = Quaternion.FromToRotation(-point.GetNormal(), direction) * grabPoint.rotation;

            var worldTarget = SimpleTransform.Create(target.position + direction * GetWorldRadius(), grabRotation);

            var host = GetHostGameObject().transform;
            var hostTransform = SimpleTransform.Create(host.position, host.rotation);

            return hostTransform.InverseTransform(worldTarget);
        }

        public override SimpleTransform CalculateDefaultTargetInHost(PalmPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var direction = target.right;

            float dot = point.GetThumbDot();

            direction *= dot;

            var grabRotation = target.rotation;

            var worldTarget = SimpleTransform.Create(target.position + direction * GetWorldRadius(), grabRotation);

            var host = GetHostGameObject().transform;
            var hostTransform = SimpleTransform.Create(host.position, host.rotation);

            return hostTransform.InverseTransform(worldTarget);
        }
    }
}
