using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Data;

using VAT.Shared.Utilities;

namespace VAT.Interaction.Entities
{
    [DisallowMultipleComponent]
    public class CrystBody : MonoBehaviour, IEntityChild
    {
        public static readonly ComponentCache<CrystBody> Cache = new();

        [SerializeField]
        private Rigidbody _rigidbody = null;

        [SerializeField]
        private Collider[] _colliders = new Collider[0];

        [SerializeField]
        private RigidbodyInfo _defaultInfo = RigidbodyInfo.Default;

        private IEntity _parentEntity = null;
        public IEntity ParentEntity { get => _parentEntity; set => _parentEntity = value; }

        public Rigidbody Rigidbody
        {
            get
            {
                return _rigidbody;
            }
        }

        public Collider[] Colliders
        {
            get
            {
                return _colliders;
            }
            set
            {
                _colliders = value;
            }
        }

        public RigidbodyInfo Info
        {
            get
            {
                return _defaultInfo;
            }
        }

        public bool HasBody
        {
            get
            {
                return Rigidbody != null;
            }
        }

        private void Awake()
        {
            Cache.Add(gameObject, this);
        }

        private void OnDestroy()
        {
            Cache.Remove(gameObject, this);
        }

        public void CollectColliders()
        {
            Colliders = FindCollidersInChildren();
        }

        public Collider[] FindCollidersInChildren()
        {
            var childColliders = GetComponentsInChildren<Collider>();

            List<Collider> validColliders = new();

            foreach (var collider in childColliders)
            {
                if (collider.GetComponentInParent<CrystBody>(true) != this)
                {
                    continue;
                }

                validColliders.Add(collider);
            }

            return validColliders.ToArray();
        }

        public void CreateBody()
        {
            if (HasBody)
            {
                return;
            }

            _rigidbody = gameObject.AddComponent<Rigidbody>();

            Info.CopyTo(_rigidbody);
        }

        public void DestroyBody()
        {
            if (!HasBody)
            {
                return;
            }

            Destroy(Rigidbody);
        }

        public void Freeze(bool frozen = true)
        {
            if (!HasBody)
            {
                return;
            }

            if (frozen)
            {
                _rigidbody.isKinematic = true;
            }
            else
            {
                _rigidbody.isKinematic = Info.IsKinematic;
            }
        }

        public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if (!HasBody)
            {
                return;
            }

            Rigidbody.AddForce(force, mode);
        }

        public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            if (!HasBody)
            {
                return;
            }

            Rigidbody.AddTorque(torque, mode);
        }
    }
}
