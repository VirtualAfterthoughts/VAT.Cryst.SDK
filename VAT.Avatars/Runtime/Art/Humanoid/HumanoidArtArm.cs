using System.Collections;
using System.Collections.Generic;

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

        public HumanoidArtHand Hand = null;

        public override void Initiate()
        {
            base.Initiate();

            Hand = new HumanoidArtHand();
            Hand.Initiate();
        }

        public override void Solve() {
            CollarBone.Solve(BoneGroup.Clavicle.Transform);
            ShoulderBlade.Solve(BoneGroup.Scapula.Transform);

            // Twist upper arm and elbow
            SimpleTransform upperArm = BoneGroup.UpperArm.Transform;
            SimpleTransform elbow = BoneGroup.Elbow.Transform;
            //SimpleTransform wrist = BoneGroup.Hand.TransformBone(BoneGroup.Hand.Hand, BoneGroup.Wrist);

            //float twistAngle = Vector3.SignedAngle(elbow.up, wrist.up, elbow.forward);

            //elbow.rotation = Quaternion.AngleAxis(twistAngle, elbow.forward) * elbow.rotation;

            //upperArm.rotation = Quaternion.AngleAxis(twistAngle * 0.45f, upperArm.forward) * upperArm.rotation;

            UpperArm.Solve(upperArm);
            LowerArm.Solve(elbow);
            //Wrist.Solve(wrist);
            //Carpal.Solve(PhysGroup.Hand.TransformBone(DataGroup.Hand.Hand, DataGroup.Carpal));

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
            //Wrist.WriteOffset(boneGroup.Wrist);
            //Carpal.WriteOffset(boneGroup.Carpal);

            Hand.WriteOffsets(boneGroup.Hand);
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
