using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;
using VAT.Shared.Data;

namespace VAT.Avatars.Art
{
    public class HumanoidArtFinger : HumanoidArtBoneGroupT<HumanoidFingerDescriptor, IFingerGroup> {
        public override int BoneCount => 4;

        public ArtBone MetaCarpal => Bones[0] as ArtBone;
        public ArtBone Proximal => Bones[1] as ArtBone;
        public ArtBone Middle => Bones[2] as ArtBone;
        public ArtBone Distal => Bones[3] as ArtBone;

        public override void Solve()
        {
            // MetaCarpal.Solve(DataGroup.MetaCarpal.Transform);
            Proximal.Solve(BoneGroup.Proximal.Transform);
            Middle.Solve(BoneGroup.Middle.Transform);
            Distal.Solve(BoneGroup.Distal.Transform);
        }

        public override void WriteOffsets(IFingerGroup boneGroup) {
            // MetaCarpal.WriteOffset(boneGroup.MetaCarpal);
            Proximal.WriteOffset(boneGroup.Proximal);
            Middle.WriteOffset(boneGroup.Middle);
            Distal.WriteOffset(boneGroup.Distal);
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
