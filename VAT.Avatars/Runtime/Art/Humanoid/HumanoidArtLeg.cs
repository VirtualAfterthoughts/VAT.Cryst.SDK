using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtLeg : HumanoidArtBoneGroupT<HumanoidLegDescriptor, HumanoidLeg, HumanoidPhysLeg>
    {
        public override int BoneCount => 4;

        public ArtBone UpperLeg => Bones[0];
        public ArtBone LowerLeg => Bones[1];
        public ArtBone Foot => Bones[2];
        public ArtBone Toe => Bones[3];

        public override void Solve() {
            UpperLeg.Solve(PhysGroup.Hip.Transform);
            LowerLeg.Solve(PhysGroup.Knee.Transform);
            Foot.Solve(PhysGroup.Ankle.Transform);
            Toe.Solve(PhysGroup.Ankle.TransformBone(DataGroup.Ankle, DataGroup.Toe));
        }

        public override void WriteOffsets(HumanoidLeg dataGroup) {
            UpperLeg.WriteOffset(dataGroup.Hip);
            LowerLeg.WriteOffset(dataGroup.Knee);
            Foot.WriteOffset(dataGroup.Ankle);
            Toe.WriteOffset(dataGroup.Toe);
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
