using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Avatars.Skeletal;
using VAT.Avatars.REWORK;
using VAT.Entities;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Muscular
{
    public abstract class PhysBone : IBone
    {
        private List<PhysBone> _children = null;
        private PhysBone _parent = null;

        public abstract CrystBody Body { get; }
        public abstract CrystJoint Joint { get; }

        protected GameObject _gameObject = null;
        public GameObject UnityGameObject { get { return _gameObject; } }

        protected Transform _transform = null;
        public Transform UnityTransform { get { return _transform; } }

        private readonly List<Collider> _colliders = new();
        public List<Collider> Colliders { get { return _colliders; } }

        public virtual SimpleTransform Transform
        {
            get => SimpleTransform.Create(_transform);
            set
            {
                _transform.SetPositionAndRotation(value.position, value.rotation);
                _transform.localScale = value.lossyScale;
            }
        }

        public PhysBone Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null) {
                    _parent.Internal_RemoveChild(this);
                    DetachJoint();
                }

                if (value != null) {
                    value.Internal_InsertChild(this);
                }

                AttachJoint(value);
            }
        }

        IBone IBone.Parent => Parent;

        int IBone.ChildCount => _children.Count;

        public virtual void MatchBone(IBone bone) {
            Transform = bone.Transform;
            Joint.RecalculateJointSpace();
        }

        public abstract void AttachJoint(PhysBone bone);

        public virtual bool DetachJoint() {
            if (!Joint.HasJoint)
                return false;

            Joint.DestroyItem();
            return true;
        }

        public virtual void Destroy() {
            _gameObject.TryDestroy();
        }

        public abstract void SetMass(float kg);

        public abstract void Solve(SimpleTransform target);

        public void ResetAnchors() {
            ResetAnchors(Joint.Transform.position);
        }

        public abstract void ResetAnchors(float3 center);

        public abstract void SetAnchor(float3 anchor);

        public abstract void SetConnectedAnchor(float3 connectedAnchor);

        public void InsertCollider(Collider collider) {
            _colliders.Add(collider);
        }

        public void RemoveCollider(Collider collider) {
            _colliders.Remove(collider);
        }

        private void Internal_InsertChild(PhysBone child) {
            _children ??= new List<PhysBone>();

            _children.Add(child);
            child._parent = this;
        }

        private void Internal_RemoveChild(PhysBone child) {
            if (child._parent != this)
                return;

            _children.Remove(child);
            child._parent = null;
        }

        IBone IBone.GetChild(int index)
        {
            return _children[index];
        }
    }
}
