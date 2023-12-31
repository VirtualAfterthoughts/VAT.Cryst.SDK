using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Utilities;

namespace VAT.Cryst
{
    public interface ICheckpointable
    {
        public static ComponentCache<ICheckpointable> Cache = new();

        void SetCheckpoint(ICheckpoint checkpoint);
    }

    public readonly struct SimpleCheckpoint
    {
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;

        private readonly Vector3 _velocity;
        private readonly Vector3 _angularVelocity;

        public Vector3 Position => _position;
        public Quaternion Rotation => _rotation;

        public Vector3 Velocity => _velocity;
        public Vector3 AngularVelocity => _angularVelocity;

        public SimpleCheckpoint(Vector3 position, Quaternion rotation)
            : this(position, rotation, Vector3.zero, Vector3.zero) { }

        public SimpleCheckpoint(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            _position = position;
            _rotation = rotation;
            _velocity = velocity;
            _angularVelocity = angularVelocity;
        }
    }

    public interface ICheckpoint
    {
        SimpleCheckpoint CheckpointTransform { get; }
    }
}
