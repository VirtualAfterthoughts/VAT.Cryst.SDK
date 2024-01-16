using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Proportions;

using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Math;

namespace VAT.Avatars.Skeletal
{
    using Unity.Mathematics;
    using VAT.Input;

    public class HumanoidArm : HumanoidBoneGroup
    {
        public bool isLeft = false;

        public DataBone Clavicle => Bones[0];
        public DataBone Scapula => Bones[1];
        public DataBone UpperArm => Bones[2];
        public DataBone Elbow => Bones[3];
        public DataBone Wrist => Bones[4];
        public DataBone Carpal => Bones[5];

        public HumanoidHand Hand => SubGroups[0] as HumanoidHand;

        public override int BoneCount => 6;

        public override int SubGroupCount => 1;

        private HumanoidSpine _spine;

        private HumanoidNeckProportions _neckProportions;
        private HumanoidSpineProportions _spineProportions;
        private HumanoidArmProportions _armProportions;

        private SimpleTransform _target;

        private Vector3 _armVector;
        private Vector3 _armUp;

        private float _twistSmooth;
        private float _twistRelax;
        private float _limitSmooth;

        private float _lowerArmLength;
        private float _upperArmLength;
        private float _armLength;

        private Vector3 _initialUpperArm;

        // Arm
        public AnimationCurve CarpalBend = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(40f, 35f, 0.54f, 0.54f), new Keyframe(100f, 45f));
        public AnimationCurve ElbowRolloff = new AnimationCurve(new Keyframe(-0.03f, 0.075f), new Keyframe(0.1f, 0.1f, 0f, 1.25f), new Keyframe(0.97f, 0.97f, 1.25f, 0f), new Keyframe(1.03f, 0.99f));

        public AnimationCurve ElbowRelaxCurve = new AnimationCurve(new Keyframe(-365f, -20.1f), new Keyframe(-220.3f, -44.4f), new Keyframe(-8.5f, -20.6f), new Keyframe(162.1f, 93.7f), new Keyframe(395f, 111.1f));
        public AnimationCurve ElbowLimitCurve = new AnimationCurve(new Keyframe(-180f, 0f), new Keyframe(-60f, 0f), new Keyframe(-20f, -75f), new Keyframe(70f, 0f), new Keyframe(180f, 0f));

        // Clavicles
        public AnimationCurve ClavicleReachCurve = new AnimationCurve(new Keyframe(-0.7f, -0.7f, 0f, 0f), new Keyframe(-0.5f, -0.1f, 0.36f, 0.36f), new Keyframe(-0.08f, 0.05f, 0.36f, 0f), new Keyframe(0f, 0f, -0.7f, -0.7f), new Keyframe(0.08f, -0.05f, 0f, 0.36f), new Keyframe(0.404f, 0.1f, 0.36f, 0.36f), new Keyframe(0.604f, 0.7f, 0f, 0f));

        public AnimationCurve ClavicleYCurve = new AnimationCurve(new Keyframe(-1f, -5.24f, 0f, 0f), new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 5.63f, 0f, 0f));

        public AnimationCurve ClavicleZCurve = new AnimationCurve(new Keyframe(-1.2f, -15f, 0f, 0f), new Keyframe(-0.8f, -5f, 18f, 18f), new Keyframe(0f, 0f, 0f, 0f), new Keyframe(0.8f, 15.7f, 65f, 65f), new Keyframe(1.2f, 45f, 0f, 0f));

        public override void Initiate()
        {
            base.Initiate();

            SubGroups[0] = new HumanoidHand(this);
        }

        public override void WriteProportions(HumanoidProportions proportions) {
            _neckProportions = proportions.neckProportions;
            _spineProportions = proportions.spineProportions;

            if (isLeft)
                _armProportions = proportions.leftArmProportions;
            else
                _armProportions = proportions.rightArmProportions;

            _lowerArmLength = _armProportions.elbowEllipsoid.height;
            _upperArmLength = _armProportions.upperArmEllipsoid.height;
            _armLength = _lowerArmLength + _upperArmLength;

            if (isLeft)
                Hand.Hand.localScale = new Vector3(-1f, 1f, 1f);

            Hand.WriteProportions(_armProportions.handProportions);
        }

        public override void BindPose() {
            float mult = isLeft ? -1f : 1f;

            Clavicle.localPosition = new float3(mult * _armProportions.clavicleSeparation, _spineProportions.upperChestEllipsoid.height * 0.02f, _spineProportions.upperChestEllipsoid.radius.y * 0.85f);
            Clavicle.localRotation = Quaternion.AngleAxis(25f * mult, Vector3.up) * Quaternion.AngleAxis(3f * mult, Vector3.forward);

            Scapula.localPosition = new float3(mult * _armProportions.clavicleEllipsoid.radius.x * 1.5f, 0f, -_spineProportions.upperChestEllipsoid.radius.y * 0.85f);
            Scapula.rotation = Clavicle.Parent.rotation;

            UpperArm.localPosition = new float3(mult * _armProportions.shoulderBladeEllipsoid.radius.x, 0f, _armProportions.shoulderBladeEllipsoid.radius.y * 0.2f);
            UpperArm.localRotation = Quaternion.AngleAxis(90f * mult, Vector3.up);

            Elbow.localPosition = new float3(0f, 0f, _armProportions.upperArmEllipsoid.height);
            Wrist.localPosition = new float3(0f, 0f, _armProportions.elbowEllipsoid.height);

            _initialUpperArm = _spine.T1Vertebra.InverseTransformPoint(UpperArm.position);

            base.BindPose();
        }

        public override void Solve() {
            Handedness handedness = isLeft ? Handedness.LEFT : Handedness.RIGHT;
            _avatarPayload.TryGetHand(handedness, out _target);

            ClavicleSolve();
            ScapulaSolve();
            ArmSolve();
            ElbowRelax();
            ElbowLimit();

            Hand.Solve();
        }

        private void ClavicleSolve() {
            float mult = isLeft ? 1f : -1f;

            // Get the direction from the clavicle to the hand (arm length)
            Vector3 upperArm = _spine.T1Vertebra.TransformPoint(_initialUpperArm);

            Vector3 clav = _target.position - (float3)upperArm;
            clav /= _armLength * 1.5f;

            // Solve the Y axis of clavicles (back and forth)
            float pull = ClavicleReachCurve.Evaluate(Vector3.Dot(_spine.T1Vertebra.forward, clav));
            float angle = ClavicleYCurve.Evaluate(Mathf.Atan(pull)) * Mathf.Rad2Deg;
            angle = angle * mult * 0.05f;
            Clavicle.rotation = Quaternion.AngleAxis(angle, _spine.T1Vertebra.up) * _spine.T1Vertebra.rotation;

            // Solve the Z axis of clavicles (up and down)
            pull = ClavicleReachCurve.Evaluate(Vector3.Dot(_spine.T1Vertebra.up, clav));
            angle = ClavicleYCurve.Evaluate(Mathf.Atan(pull)) * Mathf.Rad2Deg;
            angle = -angle * mult * 0.05f;
            Clavicle.rotation = Quaternion.AngleAxis(angle, _spine.T1Vertebra.forward) * Clavicle.rotation;

            Clavicle.rotation = (Quaternion.AngleAxis(25f * mult, -Clavicle.up) * Quaternion.AngleAxis(3f * mult, -Clavicle.forward)) * Clavicle.rotation;
        }

        private void ScapulaSolve() {
            float scapulaMult = 2f;

            float mult = isLeft ? 1f : -1f;
            Vector3 forward = Quaternion.AngleAxis(25f * mult, Clavicle.up) * Clavicle.forward;
            Vector3 up = Quaternion.AngleAxis(3f * mult, Clavicle.forward) * Clavicle.up;

            float lateralAngle = Vector3.Angle(up, _spine.T1Vertebra.up);
            Vector3 lateralAxis = Vector3.Cross(up, _spine.T1Vertebra.up);
            Quaternion lateralRotation = Quaternion.AngleAxis(-lateralAngle * scapulaMult, lateralAxis);

            float protractAngle = Vector3.Angle(forward, _spine.T1Vertebra.forward);
            Vector3 protractAxis = Vector3.Cross(forward, _spine.T1Vertebra.forward);
            Quaternion protractRotation = Quaternion.AngleAxis(-protractAngle * scapulaMult, protractAxis);

            Scapula.rotation = protractRotation * lateralRotation * _spine.T1Vertebra.rotation;
        }

        private float3 GetDepenetration(LineData line, SimpleTransform transform, Ellipsoid ellipsoid) {
            return ellipsoid.AsInterface().GetDepenetration(transform, line.ClosestPointOnLine(transform.position));
        }

        private void ArmSolve() {
            // First we solve a likely elbow position, then we can rotate the elbow using the wrist twist
            // cos(y) = a^2 + b^2 - c^2 / 2ab
            // Find angle between any two lengths let a be length 1 and b be length 2

            // Remap shoulder position
            var shoulderPosition = UpperArm.position;

            var target = _target.position;
            var line = new LineData(target, target + _target.forward * (_armProportions.handProportions.wristEllipsoid.height + _armProportions.handProportions.knuckleEllipsoid.height));

            var upperChestTransform = _spine.T1Vertebra.Transform;
            upperChestTransform.position += upperChestTransform.up * _spineProportions.upperChestEllipsoid.height * -0.5f;

            var chestTransform = _spine.T7Vertebra.Transform;
            chestTransform.position += chestTransform.up * _spineProportions.chestEllipsoid.height * -0.5f;

            var spineTransform = _spine.L1Vertebra.Transform;
            spineTransform.position += spineTransform.up * _spineProportions.spineEllipsoid.height * -0.5f;

            var closest = BodyRemapping.BlendClosest(target,
                new BodyRemapGroup(_spine.Neck.Skull.Transform, _neckProportions.skullEllipsoid),
                new BodyRemapGroup(_spine.Neck.C1Vertebra.Transform, _neckProportions.upperNeckEllipsoid),
                new BodyRemapGroup(_spine.Neck.C4Vertebra.Transform, _neckProportions.lowerNeckEllipsoid),
                new BodyRemapGroup(upperChestTransform, _spineProportions.upperChestEllipsoid),
                new BodyRemapGroup(chestTransform, _spineProportions.chestEllipsoid),
                new BodyRemapGroup(spineTransform, _spineProportions.spineEllipsoid),
                new BodyRemapGroup(_spine.Sacrum.Transform, _spineProportions.pelvisEllipsoid)
                );

            target += GetDepenetration(line, closest.transform, closest.ellipsoid);

            Vector3 newVector = target - shoulderPosition;

            // Make sure the vector isn't too small as to cause issues
            if (newVector.sqrMagnitude > 0.01f) _armVector = newVector;

            float a = _armVector.magnitude;
            float b = _armProportions.upperArmEllipsoid.height;
            float c = _armProportions.elbowEllipsoid.height;

            float A = Mathf.Acos(((Mathf.Pow(a, 2f) + Mathf.Pow(b, 2f) - Mathf.Pow(c, 2f)) / (2f * a * b)).SinClamp());
            float B = Mathf.Acos(((Mathf.Pow(b, 2f) + Mathf.Pow(c, 2f) - Mathf.Pow(a, 2f)) / (2f * b * c)).SinClamp());

            // Fixes flipping of the elbows along certain axes
            Vector3 crossRelax = Quaternion.AngleAxis(-90f, -Clavicle.right) * Vector3.ProjectOnPlane(_armVector, -Clavicle.right);

            // Now, apply the angles to the shoulder
            float armPerc = (1f - Mathf.Clamp01(a / (b + c) + 0.6f)) * 10f;
            _armUp = Vector3.Lerp(_armUp, Vector3.zero, armPerc);

            UpperArm.rotation = Quaternion.LookRotation(_armVector, -crossRelax + _armUp);
            UpperArm.rotation = Quaternion.AngleAxis(A * Mathf.Rad2Deg, UpperArm.right) * UpperArm.rotation;
            _armUp = Vector3.Lerp(Clavicle.up, UpperArm.up, 0.85f);

            // Simple elbow solve, once we have the upper arm the elbow is pretty easy to solve
            Elbow.localPosition = Vector3.forward * b;

            Elbow.rotation = Quaternion.AngleAxis(180f - B * Mathf.Rad2Deg, -UpperArm.right) * UpperArm.rotation;

            WristSolve();
        }

        private void WristSolve()
        {
            // Rotate the wrist according to the hand. Hand should always match target rotation.
            Wrist.rotation = Quaternion.AngleAxis(-Vector3.Angle(Elbow.forward, _target.forward), Vector3.Cross(Elbow.forward, _target.forward)) * _target.rotation;

            // Bend the carpal from the hand to the wrist like a spine
            float bendAngle = Vector3.Angle(Elbow.forward, _target.forward);
            Vector3 bendAxis = Vector3.Cross(Elbow.forward, _target.forward);
            Carpal.rotation = Quaternion.AngleAxis(-CarpalBend.Evaluate(bendAngle), bendAxis) * _target.rotation;

            Hand.Hand.rotation = _target.rotation;
        }

        private void ElbowRelax()
        {
            float mult = isLeft ? -1f : 1f;

            // Get the angle of the wrist twist
            float newTwist = Vector3.SignedAngle(Quaternion.AngleAxis(Mathf.Clamp(_twistRelax * mult, -15f, 275f) * mult, Elbow.forward) * -Elbow.up, Wrist.up, Elbow.forward) + _twistRelax;

            if (newTwist > 360f)
                newTwist -= 360f;
            else if (newTwist < -360f)
                newTwist += 360f;

            // Smooth the output
            _twistRelax = ElbowRelaxCurve.Evaluate((newTwist - 45f * mult) * mult) * mult;

            _twistSmooth = Mathf.Lerp(_twistSmooth, _twistRelax, Time.deltaTime * 30f);
            UpperArm.rotation = Quaternion.AngleAxis(_twistSmooth, _armVector) * UpperArm.rotation;

            WristSolve();
        }

        private void ElbowLimit()
        {
            float mult = isLeft ? 1f : -1f;

            float num = Vector3.Angle(_spine.L1Vertebra.position - UpperArm.position, UpperArm.forward);
            if (num < 70f)
            {
                Vector3 from = Vector3.ProjectOnPlane(UpperArm.forward, _armVector);
                Vector3 to = Vector3.ProjectOnPlane(_spine.L1Vertebra.position - UpperArm.position, _armVector);
                float time = Vector3.SignedAngle(from, to, _armVector * mult);
                time = ElbowLimitCurve.Evaluate(time);
                time *= 1f - Mathf.Clamp01((num - 50f) / 20f);

                _limitSmooth = Mathf.Lerp(_limitSmooth, time, Time.deltaTime * 12f);
                UpperArm.rotation = Quaternion.AngleAxis(_limitSmooth, _armVector * mult) * UpperArm.rotation;

                WristSolve();
            }
        }

        public override void Attach(DataBoneGroup group) {
            FirstBone.Parent = group.FirstBone;

            _spine = group as HumanoidSpine;
        }
    }
}
