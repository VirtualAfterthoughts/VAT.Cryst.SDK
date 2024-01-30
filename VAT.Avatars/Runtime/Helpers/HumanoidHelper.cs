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
        public static void CalculateSpine(ref HumanoidSpineProportions proportions, HumanoidSpine spine, HumanoidSpineDescriptor descriptor) {
            if (descriptor.chest.HasTransform) {
                proportions.upperChestEllipsoid.height = distance(descriptor.chest.transform.position, spine.T1Vertebra.position);
            }

            if (descriptor.hips.HasTransform) {
                proportions.spineEllipsoid.height = distance(descriptor.hips.transform.position, spine.L1Vertebra.position);
            }
        }

        public static void CalculateArm(ref HumanoidArmProportions proportions, HumanoidArm arm, HumanoidArmDescriptor descriptor) {
            if (descriptor.lowerArm.HasTransform) {
                proportions.upperArmEllipsoid.height = distance(descriptor.lowerArm.transform.position, arm.UpperArm.position);
            }
            
            if (descriptor.wrist.HasTransform)
            {
                proportions.elbowEllipsoid.height = distance(descriptor.wrist.transform.position, arm.Elbow.position);
            }
            else if (descriptor.hand.hand.HasTransform) 
            {
                proportions.elbowEllipsoid.height = distance(descriptor.hand.hand.transform.position, arm.Elbow.position);
            }
            
            CalculateHand(ref proportions.handProportions, arm.Hand, descriptor.hand);
        }

        public static void CalculateHand(ref HandProportions proportions, HumanoidHand hand, HumanoidHandDescriptor descriptor)
        {
            proportions.handedness = descriptor.isLeft ? Input.Handedness.LEFT : Input.Handedness.RIGHT;

            proportions.fingerProportions = new FingerProportions[5];

            proportions.fingerProportions[0] = Internal_CalculateFinger(hand, descriptor.thumb, Vector3.left);
            proportions.fingerProportions[1] = Internal_CalculateFinger(hand, descriptor.index, Vector3.up);
            proportions.fingerProportions[2] = Internal_CalculateFinger(hand, descriptor.middle, Vector3.up);
            proportions.fingerProportions[3] = Internal_CalculateFinger(hand, descriptor.ring, Vector3.up);
            proportions.fingerProportions[4] = Internal_CalculateFinger(hand, descriptor.pinky, Vector3.up);

            proportions.wristEllipsoid.height = distance(hand.Hand.position, descriptor.middle.proximal.transform.position);
            Vector3 fingerDirection = descriptor.middle.distal.transform.position - descriptor.middle.proximal.transform.position;
            proportions.knuckleEllipsoid.height = fingerDirection.magnitude + proportions.fingerProportions[2].distalEllipsoid.height;
        }

        private static FingerProportions Internal_CalculateFinger(HumanoidHand hand, HumanoidFingerDescriptor descriptor, Vector3 up)
        {
            FingerProportions finger = default;

            finger.metaCarpalTransform = SimpleTransform.Default;
            finger.proximalTransform = SimpleTransform.Default;

            Vector3 offset = descriptor.middle.Transform.position - descriptor.proximal.Transform.position;
            up = hand.Hand.Transform.TransformVector(up);

            quaternion direction = Quaternion.LookRotation(offset.normalized, up);
            quaternion worldToLocal = inverse(direction);

            if (descriptor.metaCarpal.HasTransform)
            {
                var metaCarpal = SimpleTransform.Create(descriptor.metaCarpal.Transform.position, direction);
                finger.metaCarpalTransform = hand.Hand.Transform.InverseTransform(metaCarpal);

                finger.proximalTransform.position = metaCarpal.InverseTransformPoint(descriptor.proximal.Transform.position);
            }
            else if (descriptor.proximal.HasTransform)
            {
                finger.metaCarpalTransform.rotation = hand.Hand.Transform.InverseTransformRotation(direction);
                finger.metaCarpalTransform.position = hand.Hand.Transform.InverseTransformPoint(descriptor.proximal.Transform.position);
            }

            finger.phalanxCount = descriptor.distal ? 3 : 2;

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
