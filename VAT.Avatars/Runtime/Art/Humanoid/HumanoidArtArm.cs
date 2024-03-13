using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class HumanoidArtArm : HumanoidArtBoneGroupT<HumanoidArmDescriptor, IHumanArm>
    {
        public override int BoneCount => 7;

        public ArtBone CollarBone => Bones[0] as ArtBone;
        public ArtBone ShoulderBlade => Bones[1] as ArtBone;
        public ArtBone UpperArm => Bones[2] as ArtBone;
        public ArtBone LowerArm => Bones[3] as ArtBone;
        public ArtBone Wrist => Bones[4] as ArtBone;
        public ArtBone Carpal => Bones[5] as ArtBone;

        public ArtBone[] LowerTwists = new ArtBone[0];

        public HumanoidArtHand Hand = null;

        private bool _isLeft = false;

        public override void Initiate()
        {
            base.Initiate();

            Hand = new HumanoidArtHand();
            Hand.Initiate();
        }

        public override void Solve() {
            float mult = _isLeft ? 1f : -1f;

            var clavicle = BoneGroup.Clavicle.Transform;

            CollarBone.Solve(clavicle);
            ShoulderBlade.Solve(BoneGroup.Scapula.Transform);

            // Twist upper arm and elbow
            SimpleTransform upperArm = BoneGroup.UpperArm.Transform;
            SimpleTransform elbow = BoneGroup.Elbow.Transform;
            SimpleTransform wrist = BoneGroup.Wrist.Transform;
            SimpleTransform carpal = BoneGroup.Carpal.Transform;

            // Axis correction
            elbow.rotation = Quaternion.AngleAxis(90f * mult, elbow.forward) * elbow.rotation;
            upperArm.rotation = Quaternion.AngleAxis(90f * mult, upperArm.forward) * upperArm.rotation;

            // Upper arm twist
            Vector3 twistUp = Quaternion.FromToRotation(clavicle.forward, upperArm.forward) * clavicle.up;
            float upperTwist = Vector3.SignedAngle(upperArm.up, twistUp, upperArm.forward);

            upperArm.rotation = Quaternion.AngleAxis(upperTwist * 0.5f, upperArm.forward) * upperArm.rotation;

            UpperArm.Solve(upperArm);
            LowerArm.Solve(elbow);

            for (var i = 0; i < LowerTwists.Length; i++)
            {
                var bone = LowerTwists[i];
                int count = LowerTwists.Length + 2;
                int position = i + 1;

                float percent = ((float)position / (float)count);

                var transform = elbow;
                transform.rotation = Quaternion.Lerp(transform.rotation, wrist.rotation, percent);

                bone.Solve(transform);
            }

            Wrist.Solve(wrist);
            Carpal.Solve(carpal);

            Hand.Solve();
        }

        public override void WriteData(IHumanArm boneGroup)
        {
            base.WriteData(boneGroup);

            Hand.WriteData(boneGroup.Hand);
        }

        public override void WriteOffsets(IHumanArm boneGroup) {
            CollarBone.WriteOffset(boneGroup.Clavicle);
            ShoulderBlade.WriteOffset(boneGroup.Scapula);
            UpperArm.WriteOffset(boneGroup.UpperArm);
            LowerArm.WriteOffset(boneGroup.Elbow);

            foreach (var lowerTwist in LowerTwists)
            {
                lowerTwist.WriteOffset(boneGroup.Elbow);
            }

            Wrist.WriteOffset(boneGroup.Wrist);
            Carpal.WriteOffset(boneGroup.Carpal);

            Hand.WriteOffsets(boneGroup.Hand);
        }

        public override void WriteTransforms(HumanoidArmDescriptor artDescriptorGroup)
        {
            _isLeft = artDescriptorGroup.isLeft;

            CollarBone.WriteReference(artDescriptorGroup.collarBone);
            ShoulderBlade.WriteReference(artDescriptorGroup.shoulderBlade);
            UpperArm.WriteReference(artDescriptorGroup.upperArm);
            LowerArm.WriteReference(artDescriptorGroup.lowerArm);

            LowerTwists = new ArtBone[artDescriptorGroup.lowerTwists != null ? artDescriptorGroup.lowerTwists.Length : 0];
            for (var i = 0; i <  LowerTwists.Length; i++)
            {
                var bone = new ArtBone();

                bone.WriteReference(artDescriptorGroup.lowerTwists[i]);

                LowerTwists[i] = bone;
            }

            Wrist.WriteReference(artDescriptorGroup.wrist);
            Carpal.WriteReference(artDescriptorGroup.carpal);

            Hand.WriteTransforms(artDescriptorGroup.hand);
        }
    }
}
