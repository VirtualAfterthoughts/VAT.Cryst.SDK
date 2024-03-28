using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars.REWORK;

namespace VAT.Avatars.Art
{
    public abstract class ArtBoneGroup : IBoneGroup {
        protected ArtBone[] _bones = null;
        public ArtBone[] Bones => _bones;
        public abstract int BoneCount { get; }

        public virtual ArtBoneGroup[] SubGroups { get; } = null;
        public virtual int SubGroupCount { get; } = 0;

        IBoneGroup[] IBoneGroup.SubGroups => SubGroups;

        IBone[] IBoneGroup.Bones => Bones;

        public virtual void Initiate()
        {
            _bones = new ArtBone[BoneCount];

            for (var i = 0; i < Bones.Length; i++)
            {
                _bones[i] = new ArtBone();
            }
        }

        public virtual void Deinitiate()
        {
            for (var i = 0; i < BoneCount; i++)
            {
                _bones[i].Deinitiate();
            }
        }

        public abstract void Solve();
    }
}
