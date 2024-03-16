using System.Collections;
using System.Collections.Generic;

using static Unity.Mathematics.math;

using UnityEngine;

using VAT.Avatars.Art;
using VAT.Avatars.Proportions;
using VAT.Avatars.Skeletal;

using VAT.Shared.Data;

namespace VAT.Avatars.Helpers
{
    using Unity.Mathematics;

    public static class HumanoidHelper {
        public static void CalculateLeg(ref HumanoidLegProportions proportions, HumanoidLeg leg, HumanoidLegDescriptor descriptor, Transform root)
        {
            if (descriptor.upperLeg.HasTransform)
            {
                float offset = Vector3.Dot(Vector3.ProjectOnPlane((Vector3)leg.Hip.Parent.position - descriptor.upperLeg.transform.position, leg.Hip.forward), leg.Hip.right);
                offset = Mathf.Abs(offset);
                proportions.hipSeparationOffset = offset;
            }

            if (descriptor.lowerLeg.HasTransform)
            {
                proportions.kneeOffsetZ = Vector3.Dot(Vector3.ProjectOnPlane((Vector3)leg.Hip.position - descriptor.lowerLeg.transform.position, leg.Hip.up), leg.Hip.forward);
            }

            proportions.ankleEllipsoid.height = -Vector3.Dot(Vector3.ProjectOnPlane(root.position - (Vector3)leg.Ankle.position, leg.Ankle.forward), leg.Ankle.up);
        }

        public static void CalculateNeck(ref HumanoidNeckProportions proportions, HumanoidDataSkeleton skeleton, HumanoidArtDescriptor descriptor)
        {
        }

        public static void CalculateSpine(ref HumanoidSpineProportions proportions, HumanoidDataSkeleton skeleton, HumanoidArtDescriptor descriptor) {
            var spineDescriptor = descriptor.spineDescriptor;
            var spine = skeleton.Spine;

            if (spineDescriptor.chest.HasTransform) {
                var chestOffset = (Vector3)spine.T1Vertebra.position - spineDescriptor.chest.transform.position;
                proportions.upperChestEllipsoid.height = Vector3.Dot(Vector3.ProjectOnPlane(chestOffset, spine.T1Vertebra.forward), spine.T1Vertebra.up);
            }

            if (spineDescriptor.hips.HasTransform) {
                var hipsOffset = (Vector3)spine.L1Vertebra.position - spineDescriptor.hips.transform.position;
                proportions.spineEllipsoid.height = Vector3.Dot(Vector3.ProjectOnPlane(hipsOffset, spine.L1Vertebra.forward), spine.L1Vertebra.up);
            }
        }

        public static void CalculateArm(ref HumanoidArmProportions proportions, HumanoidArm arm, HumanoidArmDescriptor descriptor) {
            proportions.upperArmRotation = Quaternion.AngleAxis(90f * (arm.isLeft ? -1f : 1f), Vector3.up);
            
            if (descriptor.upperArm.HasTransform)
            {
                var upperArmOffset = (Vector3)arm.Scapula.position - descriptor.upperArm.transform.position;

                proportions.shoulderBladeEllipsoid.radius.x = -Vector3.Dot(Vector3.ProjectOnPlane(upperArmOffset, arm.UpperArm.up), arm.UpperArm.forward);
            }

            if (descriptor.lowerArm.HasTransform) {
                var lowerArmOffset = (Vector3)arm.UpperArm.position - descriptor.lowerArm.transform.position;

                proportions.upperArmEllipsoid.height = -Vector3.Dot(Vector3.ProjectOnPlane(lowerArmOffset, arm.UpperArm.up), arm.UpperArm.forward);

                var scapulaLowerOffset = (Vector3)arm.Scapula.position - descriptor.lowerArm.transform.position;
                proportions.upperArmOffsetZ = -Vector3.Dot(Vector3.ProjectOnPlane(scapulaLowerOffset, arm.Scapula.up), arm.Scapula.forward);
            }

            Vector3 offset = mul(arm.Clavicle.rotation, proportions.wristOffset);

            if (descriptor.wrist.HasTransform)
            {
                var fromTo = (descriptor.wrist.transform.position + offset - (Vector3)arm.UpperArm.position).normalized;
                var worldRotation = quaternion.LookRotation(fromTo, arm.Scapula.up);
                proportions.upperArmRotation = arm.Scapula.InverseTransformRotation(worldRotation);

                var wristOffset = (Vector3)arm.Elbow.position - descriptor.wrist.transform.position;

                proportions.elbowEllipsoid.height = -Vector3.Dot(Vector3.ProjectOnPlane(wristOffset, arm.Elbow.up), arm.Elbow.forward);
            }
            else if (descriptor.hand.hand.HasTransform) 
            {
                var fromTo = (descriptor.hand.hand.transform.position + offset - (Vector3)arm.UpperArm.position).normalized;
                var worldRotation = quaternion.LookRotation(fromTo, arm.Scapula.up);
                proportions.upperArmRotation = arm.Scapula.InverseTransformRotation(worldRotation);

                var handOffset = (Vector3)arm.Elbow.position - descriptor.hand.hand.transform.position;

                proportions.elbowEllipsoid.height = -Vector3.Dot(Vector3.ProjectOnPlane(handOffset, arm.Elbow.up), arm.Elbow.forward);
            }
            
            CalculateHand(ref proportions.handProportions, arm.Hand, descriptor.hand);
        }

        public static void CalculateHand(ref HandProportions proportions, HumanoidHand hand, HumanoidHandDescriptor descriptor)
        {
            proportions.handedness = descriptor.isLeft ? Input.Handedness.LEFT : Input.Handedness.RIGHT;

            proportions.thumbProportions = new FingerProportions[1];

            proportions.thumbProportions[0] = Internal_CalculateFinger(hand, descriptor.thumb, Vector3.left);

            proportions.fingerProportions = new FingerProportions[4];

            proportions.fingerProportions[0] = Internal_CalculateFinger(hand, descriptor.index, Vector3.up);
            proportions.fingerProportions[1] = Internal_CalculateFinger(hand, descriptor.middle, Vector3.up);
            proportions.fingerProportions[2] = Internal_CalculateFinger(hand, descriptor.ring, Vector3.up);
            proportions.fingerProportions[3] = Internal_CalculateFinger(hand, descriptor.pinky, Vector3.up);

            var knuckleOffset = (Vector3)hand.Hand.position - descriptor.middle.proximal.transform.position;
            proportions.wristEllipsoid.height = -Vector3.Dot(Vector3.ProjectOnPlane(knuckleOffset, hand.Hand.up), hand.Hand.forward);
            Vector3 fingerDirection = descriptor.middle.distal.transform.position - descriptor.middle.proximal.transform.position;
            proportions.knuckleEllipsoid.height = fingerDirection.magnitude + proportions.fingerProportions[2].distalEllipsoid.height;
        }

        private static FingerProportions Internal_CalculateFinger(HumanoidHand hand, HumanoidFingerDescriptor descriptor, Vector3 up)
        {
            FingerProportions finger = default;

            finger.metaCarpalTransform = SimpleTransform.Default;
            finger.proximalTransform = SimpleTransform.Default;
            finger.middleTransform = SimpleTransform.Default;

            Vector3 offset = descriptor.middle.Transform.position - descriptor.proximal.Transform.position;
            up = hand.Hand.Transform.TransformVector(up);

            quaternion direction = Quaternion.LookRotation(offset.normalized, up);
            quaternion worldToLocal = inverse(direction);

            Vector3 endOffset = descriptor.distal.Transform.position - descriptor.middle.Transform.position;
            quaternion endDirection = Quaternion.LookRotation(endOffset.normalized, up);

            if (descriptor.metaCarpal.HasTransform)
            {
                var metaCarpal = SimpleTransform.Create(descriptor.metaCarpal.Transform.position, direction);
                finger.metaCarpalTransform = hand.Hand.Transform.InverseTransform(metaCarpal);

                finger.proximalTransform.position = metaCarpal.InverseTransformPoint(descriptor.proximal.Transform.position);
            }
            else if (descriptor.proximal.HasTransform)
            {
                var metaCarpal = SimpleTransform.Create(descriptor.proximal.Transform.position, direction);

                finger.metaCarpalTransform = hand.Hand.Transform.InverseTransform(metaCarpal);

                finger.middleTransform.rotation = metaCarpal.InverseTransformRotation(endDirection);
            }

            finger.phalanxCount = descriptor.distal.HasTransform ? 3 : 2;

            if (descriptor.middle.HasTransform)
            {
                float dist = abs(mul(worldToLocal, descriptor.middle.Transform.position - descriptor.proximal.Transform.position).z);
                finger.proximalEllipsoid.height = dist;

                if (descriptor.distal.HasTransform)
                {
                    finger.middleEllipsoid.height = abs(mul(worldToLocal, descriptor.distal.Transform.position - descriptor.middle.Transform.position).z);
                    finger.distalEllipsoid.height = abs(mul(worldToLocal, descriptor.distal.Transform.position - descriptor.middle.Transform.position).z);
                }
                else
                {
                    finger.middleEllipsoid.height = dist * 0.8f;
                    finger.distalEllipsoid.height = dist * 0f;
                }
            }

            return finger;
        }
    }
}
