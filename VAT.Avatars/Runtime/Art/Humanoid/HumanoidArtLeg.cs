using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtLeg : HumanoidArtBoneGroupT<HumanoidLegDescriptor, IHumanLeg>
    {
        public override int BoneCount => 4;

        public ArtBone UpperLeg => Bones[0] as ArtBone;
        public ArtBone LowerLeg => Bones[1] as ArtBone;
        public ArtBone Foot => Bones[2] as ArtBone;
        public ArtBone Toe => Bones[3] as ArtBone;

        public override void Solve() {
            UpperLeg.Solve(BoneGroup.Hip.Transform);
            LowerLeg.Solve(BoneGroup.Knee.Transform);
            Foot.Solve(BoneGroup.Ankle.Transform);
            // Toe.Solve(BoneGroup.Ankle.TransformBone(BoneGroup.Ankle, BoneGroup.Toe));
        }

        public override void WriteOffsets(IHumanLeg boneGroup) {
            UpperLeg.WriteOffset(boneGroup.Hip);
            LowerLeg.WriteOffset(boneGroup.Knee);
            Foot.WriteOffset(boneGroup.Ankle);
            // Toe.WriteOffset(boneGroup.Toe);
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
