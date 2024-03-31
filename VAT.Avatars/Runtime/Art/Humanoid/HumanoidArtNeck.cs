using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public class HumanoidArtNeck : HumanoidArtBoneGroupT<HumanoidNeckDescriptor, IHumanNeck>, IHumanNeck
    {
        public override int BoneCount => 3;

        public ArtBone Head => Bones[0];
        public ArtBone UpperNeck => Bones[1];
        public ArtBone LowerNeck => Bones[2];

        private RelativeBone _eyeCenter = null;
        public RelativeBone EyeCenter => _eyeCenter;

        IBone IHumanNeck.C4Vertebra => LowerNeck;

        IBone IHumanNeck.C1Vertebra => UpperNeck;

        IBone IHumanNeck.Skull => Head;

        IBone IHumanNeck.EyeCenter => EyeCenter;

        public override void Solve()
        {
            LowerNeck.Solve(BoneGroup.C4Vertebra.Transform);
            UpperNeck.Solve(BoneGroup.C1Vertebra.Transform);
            Head.Solve(BoneGroup.Skull.Transform);
        }

        public override void WriteOffsets(IHumanNeck boneGroup) {
            Head.WriteOffset(boneGroup.Skull);
            UpperNeck.WriteOffset(boneGroup.C1Vertebra);
            LowerNeck.WriteOffset(boneGroup.C4Vertebra);

            _eyeCenter = new RelativeBone(Head, boneGroup.Skull, boneGroup.EyeCenter);
        }

        public override void WriteTransforms(HumanoidNeckDescriptor artDescriptorGroup)
        {
            Head.WriteReference(artDescriptorGroup.head);
            UpperNeck.WriteReference(artDescriptorGroup.upperNeck);
            LowerNeck.WriteReference(artDescriptorGroup.lowerNeck);
        }
    }
}
