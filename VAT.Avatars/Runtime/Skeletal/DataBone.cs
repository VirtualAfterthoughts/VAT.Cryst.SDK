using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;

using static Unity.Mathematics.math;

using VAT.Avatars.REWORK;

namespace VAT.Avatars.Skeletal {
    using Unity.Mathematics;
    using VAT.Shared.Data;

    public sealed class DataBone : IBone {
        private float3 _localPosition;
        private quaternion _localRotation = quaternion.identity;
        private float3 _localScale = 1f;

        private List<DataBone> _children = null;
        private DataBone _parent = null;

        public SimpleTransform Transform { get { return SimpleTransform.Create(position, rotation, lossyScale); }
            set {
                position = value.position;
                rotation = value.rotation;
                localScale = value.lossyScale;
            }
        }

        public float3 position {
            get {
                if (_parent == null)
                    return _localPosition;

                return _parent.TransformPoint(_localPosition);
            }
            set {
                if (_parent == null)
                    _localPosition = value;
                else
                    _localPosition = _parent.InverseTransformPoint(value);
            }
        }
        public quaternion rotation { 
            get {
                if (_parent == null)
                    return _localRotation;

                return _parent.TransformRotation(_localRotation);
            }
            set
            {
                if (_parent == null)
                    _localRotation = value;
                else
                    _localRotation = _parent.InverseTransformRotation(value);
            }
        }
        public float3 lossyScale { 
            get {
                if (_parent == null)
                    return _localScale;

                return _localScale * _parent.lossyScale; 
            }
        }

        public float3 localPosition { get { return _localPosition; } set { _localPosition = value; } }
        public quaternion localRotation { get { return _localRotation; } set { _localRotation = value; } }
        public float3 localScale { get { return _localScale; } set { _localScale = value; } }

        public float3 forward { get { return mul(rotation, new float3(0f, 0f, 1f)); } }
        public float3 up { get { return mul(rotation, new float3(0f, 1f, 0f)); } }
        public float3 right { get { return mul(rotation, new float3(1f, 0f, 0f)); } }

        public DataBone Parent { 
            get { 
                return _parent; 
            } 
            set { 
                if (_parent != null) {
                    _parent.Internal_RemoveChild(this);
                }

                if (value != null)
                    value.Internal_InsertChild(this);
            } 
        }

        public DataBone() : this(null) { }

        public DataBone(DataBone parent) {
            position = float3.zero;
            rotation = quaternion.identity;
            Parent = parent;
        }

        public float3 TransformPoint(float3 point) {
            BurstCompiled_Transform.BurstCompiled_TransformPoint(point, position, rotation, lossyScale, out var result);
            return result;
        }

        public quaternion TransformRotation(quaternion rotation)
        {
            BurstCompiled_Transform.BurstCompiled_TransformRotation(rotation, this.rotation, this.lossyScale, out var result);
            return result;
        }

        public float3 InverseTransformPoint(float3 point) {
            BurstCompiled_Transform.BurstCompiled_InverseTransformPoint(point, position, rotation, lossyScale, out var result);
            return result;
        }

        public quaternion InverseTransformRotation(quaternion rotation)
        {
            BurstCompiled_Transform.BurstCompiled_InverseTransformRotation(rotation, this.rotation, lossyScale, out var result);
            return result;
        }

        private void Internal_InsertChild(DataBone child) {
            _children ??= new List<DataBone>();
            
            _children.Add(child);
            child._parent = this;
        }

        private void Internal_RemoveChild(DataBone child) {
            if (child._parent != this)
                return;
            
            _children.Remove(child);
            child.Parent = null;
        }

        public DataBone GetChild(int index) {
            return _children[index];
        }

        IBone IBone.GetChild(int index)
        {
            return _children[index];
        }

        public int ChildCount { get { return _children.Count; } }

        IBone IBone.Parent => Parent;
    }
}
