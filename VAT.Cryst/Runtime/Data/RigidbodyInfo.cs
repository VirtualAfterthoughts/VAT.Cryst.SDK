using System;

using UnityEngine;

namespace VAT.Cryst.Data
{
    [Serializable]
    public class RigidbodyInfo
    {
        public static RigidbodyInfo Default => new()
        {
            Mass = 1f,
            Drag = 0f,
            AngularDrag = 0.05f,
            AutomaticCenterOfMass = true,
            CenterOfMass = Vector3.zero,
            AutomaticInertiaTensor = true,
            InertiaTensor = Vector3.one,
            InertiaTensorRotation = Quaternion.identity,
            UseGravity = true,
            IsKinematic = false,
            Interpolation = RigidbodyInterpolation.None,
            CollisionDetectionMode = CollisionDetectionMode.Discrete,
            Constraints = RigidbodyConstraints.None,
        };

        [SerializeField]
        [Min(1e-07f)]
        private float _mass;

        [SerializeField]
        [Min(0f)]
        private float _drag;

        [SerializeField]
        [Min(0f)]
        private float _angularDrag;

        [SerializeField]
        private bool _automaticCenterOfMass;

        [SerializeField]
        private Vector3 _centerOfMass;

        [SerializeField]
        private bool _automaticInertiaTensor;

        [SerializeField]
        private Vector3 _inertiaTensor;

        [SerializeField]
        private Quaternion _inertiaTensorRotation;

        [SerializeField]
        private bool _useGravity;

        [SerializeField]
        private bool _isKinematic;

        [SerializeField]
        private RigidbodyInterpolation _interpolation;

        [SerializeField]
        private CollisionDetectionMode _collisionDetectionMode;

        [SerializeField]
        private RigidbodyConstraints _constraints;

        public float Mass { get { return _mass; } set { _mass = value; } }

        public float Drag { get { return _drag; } set { _drag = value; } }
        public float AngularDrag { get { return _angularDrag; } set { _angularDrag = value; } }

        public bool AutomaticCenterOfMass { get { return _automaticCenterOfMass; } set { _automaticCenterOfMass = value; } }
        public Vector3 CenterOfMass { get { return _centerOfMass; } set { _centerOfMass = value; } }

        public bool AutomaticInertiaTensor { get { return _automaticInertiaTensor; } set { _automaticInertiaTensor = value; } }
        public Vector3 InertiaTensor { get { return _inertiaTensor; } set { _inertiaTensor = value; } }
        public Quaternion InertiaTensorRotation { get { return _inertiaTensorRotation; } set { _inertiaTensorRotation = value; } }

        public bool UseGravity { get { return _useGravity; } set { _useGravity = value; } }
        public bool IsKinematic { get { return _isKinematic; } set { _isKinematic = value; } }

        public RigidbodyInterpolation Interpolation { get { return _interpolation; } set { _interpolation = value; } }
        public CollisionDetectionMode CollisionDetectionMode { get { return _collisionDetectionMode; } set { _collisionDetectionMode = value; } }
        public RigidbodyConstraints Constraints { get { return _constraints; } set { _constraints = value; } }

        public void CopyTo(Rigidbody rigidbody)
        {
            rigidbody.mass = _mass;

            rigidbody.drag = _drag;
            rigidbody.angularDrag = _angularDrag;

            rigidbody.automaticCenterOfMass = _automaticCenterOfMass;

            if (!_automaticCenterOfMass)
            {
                rigidbody.centerOfMass = _centerOfMass;
            }

            rigidbody.automaticInertiaTensor = _automaticInertiaTensor;

            if (!_automaticInertiaTensor)
            {
                rigidbody.inertiaTensor = _inertiaTensor;
                rigidbody.inertiaTensorRotation = _inertiaTensorRotation;
            }

            rigidbody.useGravity = _useGravity;
            rigidbody.isKinematic = _isKinematic;

            rigidbody.interpolation = _interpolation;
            rigidbody.collisionDetectionMode = _collisionDetectionMode;
            rigidbody.constraints = _constraints;
        }

        public void CopyFrom(Rigidbody rigidbody)
        {
            _mass = rigidbody.mass;

            _drag = rigidbody.drag;
            _angularDrag = rigidbody.angularDrag;

            _automaticCenterOfMass = rigidbody.automaticCenterOfMass;
            _centerOfMass = rigidbody.centerOfMass;

            _automaticInertiaTensor = rigidbody.automaticInertiaTensor;
            _inertiaTensor = rigidbody.inertiaTensor;
            _inertiaTensorRotation = rigidbody.inertiaTensorRotation;

            _useGravity = rigidbody.useGravity;
            _isKinematic = rigidbody.isKinematic;

            _interpolation = rigidbody.interpolation;
            _collisionDetectionMode = rigidbody.collisionDetectionMode;
            _constraints = rigidbody.constraints;
        }

        public bool HasChanged(Rigidbody rigidbody)
        {
            if (_mass != rigidbody.mass)
            {
                return true;
            }

            if (_drag != rigidbody.drag)
            {
                return true;
            }

            if (_angularDrag != rigidbody.angularDrag)
            {
                return true;
            }

            if (_automaticCenterOfMass != rigidbody.automaticCenterOfMass)
            {
                return true;
            }

            if (_centerOfMass != rigidbody.centerOfMass)
            {
                return true;
            }

            if (_automaticInertiaTensor != rigidbody.automaticInertiaTensor)
            {
                return true;
            }

            if (_inertiaTensor != rigidbody.inertiaTensor)
            {
                return true;
            }

            if (_inertiaTensorRotation != rigidbody.inertiaTensorRotation)
            {
                return true;
            }

            if (_useGravity != rigidbody.useGravity)
            {
                return true;
            }

            if (_isKinematic != rigidbody.isKinematic)
            {
                return true;
            }

            if (_interpolation != rigidbody.interpolation)
            {
                return true;
            }

            if (_collisionDetectionMode != rigidbody.collisionDetectionMode)
            {
                return true;
            }

            if (_constraints != rigidbody.constraints)
            {
                return true;
            }

            return false;
        }
    }
}
