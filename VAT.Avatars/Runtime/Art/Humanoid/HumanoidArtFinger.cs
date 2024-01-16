using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class HumanoidArtFinger : HumanoidArtBoneGroupT<HumanoidFingerDescriptor, HumanoidFinger, HumanoidPhysArm> {
        public override int BoneCount => 4;

        public ArtBone MetaCarpal => Bones[0];
        public ArtBone Proximal => Bones[1];
        public ArtBone Middle => Bones[2];
        public ArtBone Distal => Bones[3];

        public override void Solve()
        {
            var dataHand = DataGroup.FirstBone.Parent;
            var physHand = PhysGroup.Hand;

            SimpleTransform metaCarpal = physHand.TransformBone(dataHand, DataGroup.MetaCarpal);
            SimpleTransform proximal = physHand.TransformBone(dataHand, DataGroup.Proximal);
            SimpleTransform middle = physHand.TransformBone(dataHand, DataGroup.Middle);
            SimpleTransform distal = physHand.TransformBone(dataHand, DataGroup.Distal);

            MetaCarpal.Solve(metaCarpal);
            Proximal.Solve(proximal);
            Middle.Solve(middle);
            Distal.Solve(distal);
        }

        public void SolveData() {
            MetaCarpal.Solve(DataGroup.MetaCarpal.Transform);
            Proximal.Solve(DataGroup.Proximal.Transform);
            Middle.Solve(DataGroup.Middle.Transform);
            Distal.Solve(DataGroup.Distal.Transform);
        }

        public override void WriteOffsets(HumanoidFinger dataGroup) {
            MetaCarpal.WriteOffset(dataGroup.MetaCarpal);
            Proximal.WriteOffset(dataGroup.Proximal);
            Middle.WriteOffset(dataGroup.Middle);
            Distal.WriteOffset(dataGroup.Distal);
        }

        public override void WriteTransforms(HumanoidFingerDescriptor artDescriptorGroup)
        {
            MetaCarpal.WriteReference(artDescriptorGroup.metaCarpal);
            Proximal.WriteReference(artDescriptorGroup.proximal);
            Middle.WriteReference(artDescriptorGroup.middle);
            Distal.WriteReference(artDescriptorGroup.distal);
        }
    }
}