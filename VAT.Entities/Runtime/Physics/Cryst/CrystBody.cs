using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    /// <summary>
    /// The abstraction of a physics body in Crystalline. When adding through code, invoke <see cref="CreateItem"/>.
    /// </summary>
    public abstract class CrystBody : CachedMonoBehaviour, IRecreatable, ICrystBody
    {
        public static ComponentCache<CrystBody> Cache = new();

        [SerializeField]
        [HideInInspector]
        protected bool _hasBody = false;
        public bool HasBody { get { return _hasBody; } }

        public abstract float Mass { get; set; }

        public abstract float3 Position { get; set; }
        public abstract quaternion Rotation { get; set; }

        public abstract float3 Velocity { get; set; }
        public abstract float3 AngularVelocity { get; set; }

        private void Awake() {
            Cache.Add(GameObject, this);

            OnBodyAwake();
        }

        private void OnDestroy() {
            Cache.Remove(GameObject);

            OnBodyDestroy();
        }

        protected virtual void OnBodyAwake() { }

        protected virtual void OnBodyDestroy() { }

        public abstract void AddForce(float3 force, CrystForceMode mode = CrystForceMode.Force);

        public abstract void AddTorque(float3 torque, CrystForceMode mode = CrystForceMode.Force);

        /// <summary>
        /// Recreates the physics body if it does not currently exist.
        /// </summary>
        public abstract void CreateItem();

        /// <summary>
        /// Destroys the physics body.
        /// </summary>
        public abstract void DestroyItem();

        /// <summary>
        /// Applies any changes to the rigidbody's properties.
        /// </summary>
        public abstract void ApplyChanges();
    }
}
