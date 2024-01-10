using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities.PhysX
{
    /// <summary>
    /// A data structure containing a snapshot of a Rigidbody.
    /// </summary>
    [Serializable]
    public struct SimpleRigidbody
    {
        public static readonly SimpleRigidbody Default = new()
        {
            mass = 1f,
            drag = 0f,
            angularDrag = 0.05f,
            useGravity = true,
            isKinematic = false,
            interpolation = RigidbodyInterpolation.None,
            collisionDetectionMode = CollisionDetectionMode.Discrete,
            constraints = RigidbodyConstraints.None,
        };

        public float mass;

        public float drag;

        public float angularDrag;

        public bool useGravity;

        public bool isKinematic;

        public RigidbodyInterpolation interpolation;

        public CollisionDetectionMode collisionDetectionMode;

        public RigidbodyConstraints constraints;

        public Vector3 centerOfMass;

        public static SimpleRigidbody Create(Rigidbody rigidbody)
        {
            return new()
            {
                mass = rigidbody.mass,
                drag = rigidbody.drag,
                angularDrag = rigidbody.angularDrag,
                useGravity = rigidbody.useGravity,
                isKinematic = rigidbody.isKinematic,
                interpolation = rigidbody.interpolation,
                collisionDetectionMode = rigidbody.collisionDetectionMode,
                constraints = rigidbody.constraints,
                centerOfMass = rigidbody.centerOfMass,
            };
        }

        public void Apply(Rigidbody rigidbody)
        {
            rigidbody.mass = mass;
            rigidbody.drag = drag;
            rigidbody.angularDrag = angularDrag;
            rigidbody.useGravity = useGravity;
            rigidbody.isKinematic = isKinematic;
            rigidbody.interpolation = interpolation;
            rigidbody.collisionDetectionMode = collisionDetectionMode;
            rigidbody.constraints = constraints;
            rigidbody.centerOfMass = centerOfMass;
        }
    }
}
