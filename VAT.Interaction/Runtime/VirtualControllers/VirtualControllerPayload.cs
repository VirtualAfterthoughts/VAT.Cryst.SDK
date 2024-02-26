using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class VirtualControllerPayload
    {
        public InteractorGripPair ActivePair { get; }
        public SimpleTransform Rig { get; }
        public SimpleTransform TargetInRig { get; set; }

        public List<InteractorGripPair> GripPairs { get; }

        public VirtualControllerPayload(InteractorGripPair activePair, SimpleTransform rig, SimpleTransform targetInRig, List<InteractorGripPair> gripPairs) 
        {
            ActivePair = activePair;
            Rig = rig;
            TargetInRig = targetInRig;
            GripPairs = gripPairs;
        }
    }
}
