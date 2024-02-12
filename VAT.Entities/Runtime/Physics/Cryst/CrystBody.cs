using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

using VAT.Shared;
using VAT.Shared.Utilities;

namespace VAT.Entities
{
    [DisallowMultipleComponent]
    /// <summary>
    /// The abstraction of a physics body in Crystalline. When adding through code, invoke <see cref="CreateItem"/>.
    /// </summary>
    public abstract class CrystBody : CachedMonoBehaviour, IRecreatable, ICrystBody, IEntityComponent
    {
        public static ComponentCache<CrystBody> Cache = new();

        [SerializeField]
        [HideInInspector]
        protected bool _hasBody = false;
        public bool HasBody { get { return _hasBody; } }

        protected ICrystEntity _entity = null;
        public ICrystEntity Entity { get { return _entity; } }

        private bool _hasEntity = false;
        public bool HasEntity => _hasEntity;

        public abstract float Mass { get; set; }

        public abstract float3 Position { get; set; }
        public abstract quaternion Rotation { get; set; }

        public abstract float3 Velocity { get; set; }
        public abstract float3 AngularVelocity { get; set; }

        private Collider[] _colliders = null;

        private void Awake() {
            Cache.Add(GameObject, this);

            if (HasBody)
            {
                CollectColliders();
            }

            OnBodyAwake();
        }

        public void CollectColliders()
        {
            var children = GetComponentsInChildren<Collider>(true);
            List<Collider> result = new();

            foreach (var child in children)
            {
                if (child.GetComponentInParent<CrystBody>(true) == this)
                {
                    result.Add(child);
                }
            }

            _colliders = result.ToArray();
        }

        private void OnDestroy() {
            Cache.Remove(GameObject);

            OnBodyDestroy();
        }

        private void OnEnable()
        {
            if (!HasEntity)
            {
                _entity = GetComponentInParent<ICrystEntity>();

                if (_entity != null)
                {
                    _hasEntity = true;
                    _entity.Hierarchy.AddBody(this);
                }
            }
        }

        private void OnDisable()
        {
            if (HasEntity)
            {
                _hasEntity = false;

                _entity.Hierarchy.RemoveBody(this);
                _entity = null;
            }
        }

        protected virtual void OnBodyAwake() { }

        protected virtual void OnBodyDestroy() { }

        public abstract void AddForce(float3 force, CrystForceMode mode = CrystForceMode.Force);

        public abstract void AddTorque(float3 torque, CrystForceMode mode = CrystForceMode.Force);

        [ContextMenu("Create Body")]
        /// <summary>
        /// Recreates the physics body if it does not currently exist.
        /// </summary>
        public void CreateItem()
        {
            OnCreateItem();

            CollectColliders();
        }

        protected abstract void OnCreateItem();

        [ContextMenu("Destroy Body")]
        /// <summary>
        /// Destroys the physics body.
        /// </summary>
        public void DestroyItem()
        {
            OnDestroyItem();
        }

        protected abstract void OnDestroyItem();

        /// <summary>
        /// Applies any changes to the rigidbody's properties.
        /// </summary>
        public abstract void ApplyChanges();

        public abstract void Freeze();
        public abstract void Unfreeze();

        public Collider[] GetColliders()
        {
            return _colliders;
        }
    }
}
