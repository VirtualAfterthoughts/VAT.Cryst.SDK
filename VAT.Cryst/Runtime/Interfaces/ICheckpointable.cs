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

    public interface ICheckpoint
    {
        Vector3 CheckpointPosition { get; }
        Quaternion CheckpointRotation { get; }

        Vector3 CheckpointVelocity { get; }
        Vector3 CheckpointAngularVelocity { get; }
    }
}
