using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.Muscular;
using VAT.Avatars.REWORK;
using VAT.Avatars.Skeletal;

namespace VAT.Avatars.Art
{
    public abstract class HumanoidArtBoneGroupT<TArtGroup, TBoneGroup> : ArtBoneGroup
        where TArtGroup : IArtDescriptorGroup
        where TBoneGroup : IBoneGroup
    {
        protected TBoneGroup _boneGroup;
        public TBoneGroup BoneGroup => _boneGroup;

        public abstract void WriteTransforms(TArtGroup artDescriptorGroup);

        public virtual void WriteData(TBoneGroup boneGroup) {
            _boneGroup = boneGroup;
        }

        public abstract void WriteOffsets(TBoneGroup dataGroup);
    }
}
