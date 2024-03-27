using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.REWORK;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneGroup : IBoneGroup {
        public abstract ArtBone[] Bones { get; }
        public abstract int BoneCount { get; }
        public abstract ArtBoneGroup[] SubGroups { get; }
        public abstract int SubGroupCount { get; }

        IBoneGroup[] IBoneGroup.SubGroups => SubGroups;

        IBone[] IBoneGroup.Bones => Bones;

        public abstract void Initiate();
        public abstract void Deinitiate();

        public abstract void Solve();
    }

    public abstract class ArtBoneGroupT<TBone> : ArtBoneGroup where TBone : ArtBone {
        public sealed override ArtBone[] Bones => TBones;
        public override int SubGroupCount => 0;
        public override ArtBoneGroup[] SubGroups => null;

        protected TBone[] _bones = null;
        public virtual TBone[] TBones => _bones;

        public override void Initiate() {
            _bones = new TBone[BoneCount];
        }
    }
}
