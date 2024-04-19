using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Cryst.State;

namespace VAT.Interaction
{
    public class InteractorState
    {
        public BoolState GrabState = new();

        public BoolState ActionGrabState = new();
    }
}
