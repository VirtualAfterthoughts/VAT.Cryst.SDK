using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Entities.PhysX
{
    [DisallowMultipleComponent]
    public sealed class CrystRigidbody : CrystBody {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SimpleRigidbody _info = SimpleRigidbody.Default;

        public Rigidbody Rigidbody { get { return _rigidbody; } }

        public override float Mass { 
            get => _info.mass; 
            set 
            { 
                _info.mass = value;
                ApplyChanges();
            } 
        }

        public override float3 Position { get => Rigidbody.position; set => Rigidbody.position = value; }
        public override quaternion Rotation { get => Rigidbody.rotation; set => Rigidbody.rotation = value; }

        public override float3 Velocity { get => Rigidbody.velocity; set => Rigidbody.velocity = value; }
        public override float3 AngularVelocity { get => Rigidbody.angularVelocity; set => Rigidbody.angularVelocity = value; }

        public static ForceMode ConvertForceMode(CrystForceMode mode) {
            return mode switch {
                CrystForceMode.Acceleration => ForceMode.Acceleration,
                CrystForceMode.Impulse => ForceMode.Impulse,
                CrystForceMode.VelocityChange => ForceMode.VelocityChange,
                _ => ForceMode.Force,
            };
        }

        protected override void OnBodyAwake() {
            base.OnBodyAwake();

            if (_rigidbody != null) {
                _info.Apply(_rigidbody);
                _hasBody = true;
            }
        }

        public override void AddForce(float3 force, CrystForceMode mode = CrystForceMode.Force) {
            Rigidbody.AddForce(force, ConvertForceMode(mode));
        }

        public override void AddTorque(float3 torque, CrystForceMode mode = CrystForceMode.Force) {
            Rigidbody.AddTorque(torque, ConvertForceMode(mode));
        }

        protected override void OnCreateItem()
        {
            if (!HasBody) {
                _rigidbody = gameObject.AddOrGetComponent<Rigidbody>();
                _info.Apply(_rigidbody);

                _hasBody = true;
            }
        }

        protected override void OnDestroyItem()
        {
            if (HasBody) {
                _info = SimpleRigidbody.Create(_rigidbody);

#if UNITY_EDITOR
                DestroyImmediate(_rigidbody);
#else
                Destroy(_rigidbody);
#endif

                // Make sure the process succeeded, it could have failed to delete
                if (!_rigidbody) {
                    _rigidbody = null;

                    _hasBody = false;
                }
            }
        }

        public override void ApplyChanges()
        {
            if (HasBody)
            {
                _info.Apply(_rigidbody);
            }
        }

        public override void Freeze()
        {
            if (HasBody)
            {
                _rigidbody.isKinematic = true;
            }
        }

        public override void Unfreeze()
        {
            if (_hasBody)
            {
                _rigidbody.isKinematic = _info.isKinematic;
            }
        }

#if UNITY_EDITOR
        private void Reset() {
            _hasBody = TryGetComponent(out _rigidbody);

            if (_hasBody) {
                _info = SimpleRigidbody.Create(_rigidbody);
            }
            else {
                _info = SimpleRigidbody.Default;
                CreateItem();
            }
        }

        private void OnValidate() {
            _hasBody = TryGetComponent(out _rigidbody);

            if (_hasBody) {
                _info.Apply(_rigidbody);
            }
        }
#endif

        public static implicit operator Rigidbody(CrystRigidbody body) {
            if (!body)
                return null;

            return body.Rigidbody;
        }
    }
}
