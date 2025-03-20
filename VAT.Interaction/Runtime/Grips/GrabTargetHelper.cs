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

            var local = point.GetPressureCenterInHost(pose);

            local.Rotation *= offset;
            return local;
        }

        public static SimpleTransform GetTargetInWorld(IGrippable grip, IInteractor interactor)
        {
            var host = grip.GetHostGameObject().transform;
            return new SimpleTransform(host.position, host.rotation).Transform(grip.GetTargetInHost(interactor));
        }

        public static SimpleTransform CalculateTargetInWorld(IGrippable grip, PalmPoint point, HandPoseData pose)
        {
            var host = grip.GetHostGameObject().transform;
            return new SimpleTransform(host.position, host.rotation).Transform(grip.CalculateTargetInHost(point, pose));
        }

        public static SimpleTransform CalculateDefaultTargetInWorld(IGrippable grip, PalmPoint point, HandPoseData pose)
        {
            var host = grip.GetHostGameObject().transform;
            return new SimpleTransform(host.position, host.rotation).Transform(grip.CalculateDefaultTargetInHost(point, pose));
        }
    }
}
