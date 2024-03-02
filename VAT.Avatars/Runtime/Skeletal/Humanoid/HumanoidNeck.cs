using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Avatars.Proportions;
using VAT.Avatars.REWORK;
using VAT.Input;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Skeletal
{
    public class HumanoidNeck : HumanoidBoneGroup, IHumanNeck
    {
        public DataBone EyeCenter => Bones[0];
        public DataBone Skull => Bones[1];
        public DataBone C1Vertebra => Bones[2];
        public DataBone C4Vertebra => Bones[3];

        public override int BoneCount => 4;

        IBone IHumanNeck.C4Vertebra => C4Vertebra;

        IBone IHumanNeck.C1Vertebra => C1Vertebra;

        IBone IHumanNeck.Skull => Skull;

        private HumanoidGeneralProportions _generalProportions;
        private HumanoidNeckProportions _neckProportions;

        private float _armLength = 1f;

        public AnimationCurve CervicalUpOffset = new(new(0f, 1f), new(0.4f, 1f), new(1f, 0.1f));
        public AnimationCurve CervicalTilt = new(new(0f, 0f, 1f, 1f), new(40f, 45f, 0.54f, 0.54f), new(100f, 60f));

        public quaternion chestRotation;

        public float3 feetCenter;

        public override void Initiate() {
            base.Initiate();
        }

        public override void WriteProportions(HumanoidProportions proportions) {
            _generalProportions = proportions.generalProportions;
            _neckProportions = proportions.neckProportions;

            var arm = proportions.leftArmProportions;
            _armLength = arm.upperArmEllipsoid.height + arm.elbowEllipsoid.height;
        }

        public override void BindPose() {
            base.BindPose();

            Ellipsoid skull = _neckProportions.skullEllipsoid;
            Ellipsoid c1 = _neckProportions.upperNeckEllipsoid;

            Skull.localPosition = new(0f, _neckProportions.skullYOffset, -skull.radius.y);
            C1Vertebra.localPosition = new(0f, -skull.height * 0.4f, -_neckProportions.upperNeckOffsetZ);
            C4Vertebra.localPosition = new(0f, -c1.height, -_neckProportions.lowerNeckOffsetZ);
        }

        public override void Solve()
        {
            SimpleTransform root = _avatarPayload.GetRoot();
            _avatarPayload.TryGetHead(out var head);

            EyeCenter.rotation = head.rotation;
            EyeCenter.position = head.position;

            float height = _generalProportions.height;

            float bendAngle = Vector3.Angle(root.up, Skull.up);
            Vector3 bendAxis = Vector3.Cross(root.up, Skull.up);
            var cervicalRotation = Quaternion.AngleAxis(-bendAngle, bendAxis);

            float neckHeight = Mathf.Clamp01(Vector3.Dot(root.up, C1Vertebra.position - root.position) / (height * 0.9f));
            float cervicalHeight = CervicalUpOffset.Evaluate(neckHeight);

            Vector3 tiltVector = Quaternion.AngleAxis(cervicalHeight * 90f, cervicalRotation * Skull.right) * root.up;
            float tiltAngle = Vector3.Angle(tiltVector, Skull.up);
            Vector3 tiltAxis = Vector3.Cross(tiltVector, Skull.up);
            chestRotation = Quaternion.AngleAxis(-CervicalTilt.Evaluate(tiltAngle), tiltAxis) * Skull.rotation;

            // Leaning
            var chestUp = math.mul(chestRotation, math.up());
            var chestForward = math.mul(chestRotation, math.forward());

            var fromTo = Quaternion.FromToRotation(Skull.forward, chestForward) * Skull.rotation;
            float skullChestAngle = Vector3.Angle(fromTo * Vector3.up, chestUp);

            var offsetRotation = Quaternion.FromToRotation(math.mul(chestRotation, math.up()), math.normalize(Skull.position - feetCenter)) * chestRotation;
            float lerp = (1f - cervicalHeight) - (skullChestAngle / 90f);
            chestRotation = Quaternion.Lerp(chestRotation, offsetRotation, lerp);

            ChestPull();

            C1Vertebra.rotation = Quaternion.Lerp(Skull.rotation, chestRotation, 0.5f);
            C4Vertebra.rotation = Quaternion.Lerp(Skull.rotation, chestRotation, 0.7f);
        }

        public void ChestPull()
        {
            _avatarPayload.TryGetArm(Handedness.LEFT, out var leftArm);
            _avatarPayload.TryGetArm(Handedness.RIGHT, out var rightArm);

            leftArm.TryGetHand(out var leftHand);
            rightArm.TryGetHand(out var rightHand);

            float leftYPull = SolveChestYPull(leftHand.Transform);
            float rightYPull = SolveChestYPull(rightHand.Transform);

            float yPull = leftYPull - rightYPull;

            float leftZPull = SolveChestZPull(leftHand.Transform);
            float rightZPull = SolveChestZPull(rightHand.Transform);

            float zPull = rightZPull - leftZPull;

            var yOffset = Quaternion.AngleAxis(25f * yPull, math.mul(chestRotation, Vector3.up));
            var zOffset = Quaternion.AngleAxis(2.5f * zPull, math.mul(chestRotation, Vector3.forward));
            chestRotation = yOffset * zOffset * chestRotation;
        }

        private float SolveChestYPull(SimpleTransform hand)
        {
            var pull = hand.position - C4Vertebra.position;
            pull /= _armLength;

            var forward = math.mul(chestRotation, Vector3.forward);
            var up = math.mul(chestRotation, Vector3.up);

            var yPlane = Vector3.ProjectOnPlane(pull, up);
            float yDot = Vector3.Dot(yPlane, forward);

            return Mathf.Clamp(yDot, -1f, 1f);
        }

        private float SolveChestZPull(SimpleTransform hand)
        {
            var pull = hand.position - C4Vertebra.position;
            pull /= _armLength;

            var forward = math.mul(chestRotation, Vector3.forward);
            var up = math.mul(chestRotation, Vector3.up);

            var zPlane = Vector3.ProjectOnPlane(pull, forward);
            float zDot = Vector3.Dot(zPlane, up);

            return Mathf.Clamp(zDot, -1f, 1f);
        }
    }
}
