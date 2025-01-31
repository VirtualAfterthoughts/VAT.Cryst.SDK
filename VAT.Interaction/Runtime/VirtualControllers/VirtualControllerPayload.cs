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
        public SimpleTransform TargetInGripHost { get; set; }

        public List<InteractorGripPair> GripPairs { get; }

        public VirtualControllerPayload(InteractorGripPair activePair, SimpleTransform rig, SimpleTransform targetInRig, SimpleTransform targetInGripHost, List<InteractorGripPair> gripPairs)
        {
            ActivePair = activePair;
            Rig = rig;
            TargetInRig = targetInRig;
            TargetInGripHost = targetInGripHost;
            GripPairs = gripPairs;
        }
    }
}
