using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class VirtualControllerPayload
    {
        public InteractorGripPair interactorGripPair;
        public SimpleTransform rig;
        public SimpleTransform targetInRig;
    }
}
