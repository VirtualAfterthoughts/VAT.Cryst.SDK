using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.Nervous;
using VAT.Avatars.REWORK;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Skeletal
{
    public abstract class DataBoneGroup : IBoneGroup
    {
        protected DataBone[] _bones = null;
        public virtual DataBone[] Bones => _bones;
        public abstract int BoneCount { get; }

        protected DataBoneGroup[] _subGroups = null;
        public virtual DataBoneGroup[] SubGroups => _subGroups;
        public virtual int SubGroupCount => 0;

        public virtual DataBone FirstBone => Bones[0];
        public virtual DataBone LastBone => Bones[BoneCount - 1];

        IBone[] IBoneGroup.Bones => Bones;

        IBoneGroup[] IBoneGroup.SubGroups => SubGroups;

        protected IAvatarPayload _avatarPayload;

        public virtual void Initiate()
        {
            _bones = new DataBone[BoneCount];

            DataBone parent = null;
            for (var i = 0; i < BoneCount; i++)
            {
                _bones[i] = new DataBone(parent);
                parent = _bones[i];
            }

            _subGroups = new DataBoneGroup[SubGroupCount];
        }

        public virtual void BindPose() {
            for (var i = 0; i < SubGroupCount; i++)
                SubGroups[i].BindPose();
        }

        public virtual void NeutralPose() {
            for (var i = 0; i < SubGroupCount; i++)
                SubGroups[i].NeutralPose();
        }

        public virtual void Write(IAvatarPayload payload) {
            _avatarPayload = payload;
        }

        public abstract void Solve();

        public virtual void Attach(DataBoneGroup group)
        {
            FirstBone.Parent = group.LastBone;
        }

#if UNITY_EDITOR
        public virtual void DrawGizmos() {
            if (_bones == null)
                return;

            Mesh mesh = Resources.Load<Mesh>("Meshes/mesh_bone");
            if (mesh == null)
                return;

            if (BoneCount > 0) {
                DataBone root = _bones[0].Parent;
                foreach (var bone in _bones)
                {
                    if (root != null) {
                        Vector3 direction = bone.position - root.position;
                        if (direction.sqrMagnitude <= 0f)
                            continue;

                        Quaternion rotation = Quaternion.LookRotation(direction, bone.up);
                        float scale = (Quaternion.Inverse(rotation) * direction).z;

                        Gizmos.DrawMesh(mesh, root.position, rotation, new Vector3(scale * 0.2f, scale * 0.2f, scale * 0.5f));
                    }

                    root = bone;
                }
            }

            if (SubGroupCount > 0) {
                foreach (var group in SubGroups) {
                    group.DrawGizmos();
                }
            }
        }
#endif
    }
}
