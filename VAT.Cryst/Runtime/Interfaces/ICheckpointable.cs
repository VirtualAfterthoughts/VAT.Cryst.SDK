using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
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
        SimpleTransform CheckpointTransform { get; }
    }
}
