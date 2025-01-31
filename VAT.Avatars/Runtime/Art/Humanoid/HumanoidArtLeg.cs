using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Bones;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class HumanoidArtLeg : HumanoidArtBoneGroupT<HumanoidLegDescriptor, IHumanLeg>
    {
        public override int BoneCount => 4;

        public ArtBone UpperLeg => Bones[0];
        public ArtBone LowerLeg => Bones[1];
        public ArtBone Foot => Bones[2];
        public ArtBone Toe => Bones[3];

        public override void Solve()
        {
            SimpleTransform pelvis = BoneGroup.Hip.Parent.Transform;

            SimpleTransform hip = BoneGroup.Hip.Transform;
            SimpleTransform knee = BoneGroup.Knee.Transform;
            SimpleTransform ankle = BoneGroup.Ankle.Transform;
            SimpleTransform toe = BoneGroup.Toe.Transform;

            // Upper leg twist
            Vector3 twistUp = Quaternion.FromToRotation(pelvis.up, hip.up) * pelvis.forward;
            float upperTwist = Vector3.SignedAngle(hip.forward, twistUp, hip.up);

            hip.rotation = Quaternion.AngleAxis(upperTwist * 0.7f, hip.up) * hip.rotation;

            // Lower leg twist
            Vector3 twistLower = Quaternion.FromToRotation(hip.up, ankle.up) * hip.forward;
            float lowerTwist = Vector3.SignedAngle(ankle.forward, twistLower, knee.up);

            knee.rotation = Quaternion.AngleAxis(lowerTwist * 0.7f, knee.up) * knee.rotation;

            UpperLeg.Solve(hip);
            LowerLeg.Solve(knee);
            Foot.Solve(ankle);
            Toe.Solve(toe);
        }

        public override void WriteOffsets(IHumanLeg boneGroup)
        {
            UpperLeg.WriteOffset(boneGroup.Hip);
            LowerLeg.WriteOffset(boneGroup.Knee);
            Foot.WriteOffset(boneGroup.Ankle);
            Toe.WriteOffset(boneGroup.Toe);
        }

        public override void WriteTransforms(HumanoidLegDescriptor artDescriptorGroup)
        {
            UpperLeg.WriteReference(artDescriptorGroup.upperLeg);
            LowerLeg.WriteReference(artDescriptorGroup.lowerLeg);
            Foot.WriteReference(artDescriptorGroup.foot);
            Toe.WriteReference(artDescriptorGroup.toe);
        }
    }
}
