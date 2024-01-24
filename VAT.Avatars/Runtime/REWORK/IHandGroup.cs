using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHandGroup : IBoneGroup
    {
        public IBone Hand { get; }

        public IBone Palm { get; }
    }
}
