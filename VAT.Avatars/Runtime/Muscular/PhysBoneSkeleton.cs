using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Skeletal;
using VAT.Cryst;
using VAT.Input;
using VAT.Shared.Data;

namespace VAT.Avatars.Muscular
{
    public abstract class PhysBoneSkeleton : ISkeleton, IRigT<PhysBone> {
        public abstract int BoneGroupCount { get; }

        public abstract PhysBoneGroup[] BoneGroups { get; }

        public abstract void Initiate();

        public abstract void InitiateRuntime();

        public virtual void Solve() {
            for (var i = 0; i < BoneGroupCount; i++)
                GetGroup(i).Solve();
        }

        public PhysBoneGroup GetGroup(int index) {
            return BoneGroups[index];
        }

        public virtual void NeutralPose()
        {
            for (var i = 0; i < BoneGroupCount; i++)
                GetGroup(i).NeutralPose();
        }

        public virtual void ResetAnchors() {
            for (var i = 0; i < BoneGroupCount; i++)
                GetGroup(i).ResetAnchors();
        }

        public abstract SimpleTransform GetHead();

        public virtual void IgnoreCollisions(bool ignore) {
            for (var i = 0; i < BoneGroupCount; i++) {
                var first = GetGroup(i);

                for (var j = 0; j < BoneGroupCount; j++) {
                    first.IgnoreCollision(GetGroup(j), ignore);
                }
            }
        }

        public abstract bool TryGetHead(out PhysBone result);
        public virtual PhysBone[] GetHeads() {
            TryGetHead(out var result);
            return new PhysBone[] { result };
        }

        public abstract bool TryGetHand(Handedness handedness, out PhysBone result);
        public virtual PhysBone[] GetHands(Handedness handedness)
        {
            TryGetHand(handedness, out var result);
            return new PhysBone[] { result };
        }

        public virtual bool TryGetPelvis(out PhysBone result) {
            result = null;
            return false;
        }
        public virtual PhysBone[] GetPelvises() {
            return Array.Empty<PhysBone>();
        }

        public virtual bool TryGetFoot(Handedness handedness, out PhysBone result) {
            result = null;
            return false;
        }
        public virtual PhysBone[] GetFeet(Handedness handedness) {
            return Array.Empty<PhysBone>();
        }

        public abstract PhysBone GetRoot();

        public abstract SimpleTransform GetFloor();
    }
}
