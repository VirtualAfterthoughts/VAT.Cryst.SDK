using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars.REWORK;

namespace VAT.Avatars
{
    public interface ILegGroup : IBoneGroup
    {
        public IBone Hip { get; }
        public IBone Knee { get; }
        public IBone Ankle { get; }
        public IBone Toe { get; }
    }
}
