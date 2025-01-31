using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

namespace VAT.Interaction.Entities
{
    [DisallowMultipleComponent]
    public class CrystBody : MonoBehaviour
    {
        public static readonly ComponentCache<CrystBody> Cache = new();

        [SerializeField]
        private Rigidbody _rigidbody = null;

        [SerializeField]
        private Collider[] _colliders = new Collider[0];

        [SerializeField]
        private RigidbodyInfo _defaultInfo = RigidbodyInfo.Default;

        private CrystEntity _entity = null;
        public CrystEntity Entity { get => _entity; set => _entity = value; }

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

        private Link<CrystBody> _link = null;
        public Link<CrystBody> Link => _link;

        private void Awake()
        {
            _link = new Link<CrystBody>(this);

            Cache.Add(gameObject, this);

            Link.OnLinkConnected += OnLinkConnected;
            Link.OnLinkDisconnected += OnLinkDisconnected;
        }

        private void OnLinkConnected(Link<CrystBody> from, Link<CrystBody> to, Link<CrystBody>.LinkType type)
        {
            foreach (var collider in to.Origin.Colliders)
            {
                foreach (var other in Colliders)
                {
                    Physics.IgnoreCollision(collider, other, true);
                }
            }
        }

        private void OnLinkDisconnected(Link<CrystBody> from, Link<CrystBody> to, Link<CrystBody>.LinkType type)
        {
            foreach (var collider in to.Origin.Colliders)
            {
                foreach (var other in Colliders)
                {
                    Physics.IgnoreCollision(collider, other, false);
                }
            }
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

            _rigidbody = gameObject.AddOrGetComponent<Rigidbody>();

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
