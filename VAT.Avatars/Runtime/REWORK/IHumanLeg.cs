using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Avatars.REWORK
{
    public interface IHumanLeg : IBoneGroup
    {
        public IBone Hip { get; }
        public IBone Knee { get; }
        public IBone Ankle { get; }
        public IBone Toe { get; }
    }
}
