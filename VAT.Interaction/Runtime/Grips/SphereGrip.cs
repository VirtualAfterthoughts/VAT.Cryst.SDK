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
            var palmHost = point.GetHostTransform();

            var grabPoint = palmHost.Transform(GrabTargetHelper.GetTargetInInteractor(point, pose));
            var direction = ((Vector3)grabPoint.Position - target.position).normalized;

            var grabRotation = Quaternion.FromToRotation(palmHost.TransformDirection(-point.GetNormalInHost()), direction) * grabPoint.Rotation;

            var worldTarget = new SimpleTransform(target.position + direction * GetWorldRadius(), grabRotation);

            var host = GetHostGameObject().transform;
            var hostTransform = new SimpleTransform(host.position, host.rotation);

            return hostTransform.InverseTransform(worldTarget);
        }

        public override SimpleTransform CalculateDefaultTargetInHost(PalmPoint point, HandPoseData pose)
        {
            var target = GetTargetTransform();
            var direction = target.right;

            float dot = point.GetThumbDot();

            direction *= dot;

            var grabRotation = target.rotation;

            var worldTarget = new SimpleTransform(target.position + direction * GetWorldRadius(), grabRotation);

            var host = GetHostGameObject().transform;
            var hostTransform = new SimpleTransform(host.position, host.rotation);

            return hostTransform.InverseTransform(worldTarget);
        }
    }
}
