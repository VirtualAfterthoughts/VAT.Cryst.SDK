using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Input;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public static class GrabTargetHelper
    {
        public static Quaternion GetRotationOffset(PalmPoint point, HandPoseData pose)
        {
            var handedness = point.GetHandedness();

            var offset = pose.rotationOffset.normalized;

            if (handedness == Handedness.RIGHT)
            {
                offset.ToAngleAxis(out var angle, out var axis);
                axis.z = -axis.z;
                axis.y = -axis.y;

                offset = Quaternion.AngleAxis(angle, axis);
            }

            return offset;
        }

        public static SimpleTransform GetTargetInInteractor(PalmPoint point, HandPoseData pose)
        {
            var offset = GetRotationOffset(point, pose);

            var local = point.GetHostTransform().InverseTransform(point.GetPoint(pose.centerOfPressure));

            local.rotation *= offset;
            return local;
        }

        public static SimpleTransform GetTargetInHost(IGrippable grip, PalmPoint point)
        {
            var host = grip.GetHostGameObject().transform;
            return SimpleTransform.Create(host.position, host.rotation).InverseTransform(grip.GetTargetInWorld(point, grip.GetDefaultPose()));
        }
    }
}
