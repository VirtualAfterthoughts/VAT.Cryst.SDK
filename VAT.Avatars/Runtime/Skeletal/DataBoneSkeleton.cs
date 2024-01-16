using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;

using VAT.Avatars.Nervous;

namespace VAT.Avatars.Skeletal
{
    public abstract class DataBoneSkeleton : ISkeleton, IRigT<DataBone> {
        public abstract DataBoneGroup[] BoneGroups { get; }
        public abstract int BoneGroupCount { get; }

        public virtual void Write(IAvatarPayload payload) {
            foreach (var group in BoneGroups) {
                group.Write(payload);
            }
        }

        public abstract void Initiate();

        public abstract void Solve();

        public virtual void BindPose() {
            if (BoneGroups != null) { 
                for (var i = 0; i < BoneGroups.Length; i++) {
                    BoneGroups[i].BindPose();
                }
            }
            else {
                Debug.LogWarning("Tried to bind pose of a DataSkeleton but the BoneGroups were null!");
            }
        }

        public virtual void NeutralPose() {
            if (BoneGroups != null) {
                for (var i = 0; i < BoneGroups.Length; i++) {
                    BoneGroups[i].NeutralPose();
                }
            }
            else {
                Debug.LogWarning("Tried to neutral pose of a DataSkeleton but the BoneGroups were null!");
            }
        }

        public abstract bool TryGetHead(out DataBone result);
        public virtual DataBone[] GetHeads() {
            TryGetHead(out var result);
            return new DataBone[] { result };
        }

        public abstract bool TryGetHand(Handedness handedness, out DataBone result);
        public virtual DataBone[] GetHands(Handedness handedness) {
            TryGetHand(handedness, out var result);
            return new DataBone[] { result };
        }

        public virtual bool TryGetPelvis(out DataBone result) {
            result = null;
            return false;
        }
        public virtual DataBone[] GetPelvises() {
            return Array.Empty<DataBone>();
        }

        public virtual bool TryGetFoot(Handedness handedness, out DataBone result) {
            result = null;
            return false;
        }
        public virtual DataBone[] GetFeet(Handedness handedness) {
            return Array.Empty<DataBone>();
        }

        public abstract DataBone GetRoot();

#if UNITY_EDITOR
        public void DrawGizmos() {
            OnDrawGizmos();
        }

        protected virtual void OnDrawGizmos() { 
            if (BoneGroups != null) {
                foreach (var group in BoneGroups) {
                    group.DrawGizmos();
                }
            }
        }
#endif
    }
}
