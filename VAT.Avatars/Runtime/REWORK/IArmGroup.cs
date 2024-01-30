using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.Delegates;

using VAT.Shared.Data;

namespace VAT.Avatars.REWORK
{
    public interface IArmGroup : IBoneGroup
    {
        public IBone UpperArm { get; }
        public IBone Elbow { get; }
        public IHandGroup Hand { get; }

        public event TargetProcessorCallback OnProcessTarget;
    }
}
