using System.Collections;
using System.Collections.Generic;

namespace VAT.Avatars.Muscular
{
    using UnityEngine;
    using VAT.Avatars.REWORK;

    public abstract class PhysBoneGroup : IBoneGroup {
        public abstract int BoneCount { get; }

        public virtual int SubGroupCount => 0;

        public virtual PhysBone FirstBone => GetBone(0);
        public virtual PhysBone LastBone => GetBone(BoneCount - 1);

        public virtual bool PivotAtParent => false;

        public abstract IBone[] Bones { get; }
        public abstract IBoneGroup[] SubGroups { get; }

        public abstract void Initiate();

        public virtual void NeutralPose() { }

        public virtual void ResetAnchors() {
            for (var i = 0; i < BoneCount; i++)
                GetBone(i).ResetAnchors();

            for (var i = 0; i < SubGroupCount; i++)
                GetSubGroup(i).ResetAnchors();
        }

        public abstract void Solve();

        public virtual IReadOnlyList<Collider> GetColliders() {
            var colliders = new List<Collider>();

            for (var i = 0; i < BoneCount; i++) {
                var bone = GetBone(i);

                colliders.AddRange(bone.Colliders);
            }
            
            for (var i = 0; i < SubGroupCount; i++) {
                colliders.AddRange(GetSubGroup(i).GetColliders());
            }

            return colliders;
        }

        public virtual void IgnoreCollision(PhysBoneGroup other, bool ignore) {
            var localColliders = GetColliders();
            var otherColliders = other.GetColliders();

            for (var i = 0; i < localColliders.Count; i++) {
                var localCollider = localColliders[i];

                for (var j = 0; j < otherColliders.Count; j++) {
                    Physics.IgnoreCollision(localCollider, otherColliders[j], ignore);
                }
            }
        }

        public abstract PhysBone GetBone(int index);

        public abstract PhysBoneGroup GetSubGroup(int index);

        public virtual void Attach(PhysBoneGroup group) {
            FirstBone.Parent = group.LastBone;
        }
    }

    public abstract class PhysBoneGroupT<TBone> : PhysBoneGroup 
        where TBone : PhysBone 
    {
        public override IBone[] Bones => TBones;
        public override IBoneGroup[] SubGroups => PhysSubGroups;

        protected TBone[] _bones = null;
        public virtual TBone[] TBones => _bones;

        protected PhysBoneGroup[] _subGroups = null;
        public virtual PhysBoneGroup[] PhysSubGroups => _subGroups;

        public virtual TBone GenericFirstBone => TBones[0];
        public virtual TBone GenericLastBone => TBones[BoneCount - 1];

        public override void Initiate() {
            _bones = new TBone[BoneCount];
            _subGroups = new PhysBoneGroup[SubGroupCount];
        }

        public override PhysBone GetBone(int index) {
            return TBones[index];
        }

        public override PhysBoneGroup GetSubGroup(int index) {
            return PhysSubGroups[index];
        }
    }
}
