using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Proportions;

namespace VAT.Avatars.Vitals
{
    public class HumanoidArmVitals : HumanoidBoneGroupVitals
    {
        public override int BoneCount => 4;

        private readonly bool _isLeft;

        private HumanoidSpineProportions _spineProportions;
        private HumanoidArmProportions _proportions;
        private HumanoidPhysArm _arm;

        public float ShoulderMass => _boneMasses[0];
        public float UpperArmMass => _boneMasses[1];
        public float ElbowMass => _boneMasses[2];
        public float HandMass => _boneMasses[3];

        public HumanoidArmVitals(bool isLeft) {
            _isLeft = isLeft;
        }

        public override void ConfigureJoints() {
            _arm.Clavicle.ConfigureJoint();
            _arm.Scapula.ConfigureJoint();

            float totalNewtons = 1800f;

            _arm.UpperArm.ConfigureJoint(totalNewtons * 0.5f);
            _arm.Elbow.ConfigureJoint(totalNewtons * 0.375f);
            _arm.Hand.Hand.ConfigureJoint(totalNewtons * 0.125f);
        }

        public override void ApplyVitals() {
            // Apply mass
            float individualShoulderMass = ShoulderMass * 0.5f;

            _arm.Clavicle.SetMass(individualShoulderMass);
            _arm.Scapula.SetMass(individualShoulderMass);
            _arm.UpperArm.SetMass(UpperArmMass);
            _arm.Elbow.SetMass(ElbowMass);
            _arm.Hand.Hand.SetMass(HandMass);
        }

        public override void CalculateVitals() {
            // Calculate mass
            float density = 1.1f * 1000f;

            _boneMasses[0] = Internal_CalculateMass(_spineProportions.chestEllipsoid, density);
            _boneMasses[1] = Internal_CalculateMass(_proportions.upperArmEllipsoid, density);
            _boneMasses[2] = Internal_CalculateMass(_proportions.elbowEllipsoid, density);
            _boneMasses[3] = Internal_CalculateMass(_proportions.handProportions.wristEllipsoid, density * 3f);
        }

        protected override void OnInjectDependencies(HumanoidVitalsPayload payload)
        {
            _spineProportions = payload.Proportions.spineProportions;

            if (_isLeft) {
                _proportions = payload.Proportions.leftArmProportions;
                _arm = payload.Skeleton.LeftArm;
            }
            else {
                _proportions = payload.Proportions.rightArmProportions;
                _arm = payload.Skeleton.RightArm;
            }
        }
    }
}
