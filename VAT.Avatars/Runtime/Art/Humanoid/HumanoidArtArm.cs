using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class HumanoidArtArm : HumanoidArtBoneGroupT<HumanoidArmDescriptor, HumanoidArm, HumanoidPhysArm>
    {
        public override int BoneCount => 7;

        public ArtBone CollarBone => Bones[0];
        public ArtBone ShoulderBlade => Bones[1];
        public ArtBone UpperArm => Bones[2];
        public ArtBone LowerArm => Bones[3];
        public ArtBone Wrist => Bones[4];
        public ArtBone Carpal => Bones[5];

        public HumanoidArtHand Hand = null;

        public override void Initiate()
        {
            base.Initiate();

            Hand = new HumanoidArtHand();
            Hand.Initiate();
        }

        public override void Solve() {
            CollarBone.Solve(PhysGroup.Clavicle.Transform);
            ShoulderBlade.Solve(PhysGroup.Scapula.Transform);

            // Twist upper arm and elbow
            SimpleTransform upperArm = PhysGroup.UpperArm.Transform;
            SimpleTransform elbow = PhysGroup.Elbow.Transform;
            SimpleTransform wrist = PhysGroup.Hand.TransformBone(DataGroup.Hand.Hand, DataGroup.Wrist);

            float twistAngle = Vector3.SignedAngle(elbow.up, wrist.up, elbow.forward);

            elbow.rotation = Quaternion.AngleAxis(twistAngle, elbow.forward) * elbow.rotation;

            upperArm.rotation = Quaternion.AngleAxis(twistAngle * 0.45f, upperArm.forward) * upperArm.rotation;

            UpperArm.Solve(upperArm);
            LowerArm.Solve(elbow);
            Wrist.Solve(wrist);
            Carpal.Solve(PhysGroup.Hand.TransformBone(DataGroup.Hand.Hand, DataGroup.Carpal));

            Hand.Solve();
        }

        public override void WriteData(HumanoidArm dataGroup, HumanoidPhysArm physGroup)
        {
            base.WriteData(dataGroup, physGroup);

            Hand.WriteData(dataGroup.Hand, physGroup);
        }

        public override void WriteOffsets(HumanoidArm dataGroup) {
            CollarBone.WriteOffset(dataGroup.Clavicle);
            ShoulderBlade.WriteOffset(dataGroup.Scapula);
            UpperArm.WriteOffset(dataGroup.UpperArm);
            LowerArm.WriteOffset(dataGroup.Elbow);
            Wrist.WriteOffset(dataGroup.Wrist);
            Carpal.WriteOffset(dataGroup.Carpal);

            Hand.WriteOffsets(dataGroup.Hand);
        }

        public override void WriteTransforms(HumanoidArmDescriptor artDescriptorGroup)
        {
            CollarBone.WriteReference(artDescriptorGroup.collarBone);
            ShoulderBlade.WriteReference(artDescriptorGroup.shoulderBlade);
            UpperArm.WriteReference(artDescriptorGroup.upperArm);
            LowerArm.WriteReference(artDescriptorGroup.lowerArm);
            Wrist.WriteReference(artDescriptorGroup.wrist);
            Carpal.WriteReference(artDescriptorGroup.carpal);

            Hand.WriteTransforms(artDescriptorGroup.hand);
        }
    }
}
