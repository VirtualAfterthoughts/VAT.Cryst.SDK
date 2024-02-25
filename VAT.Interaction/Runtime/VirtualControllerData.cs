using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class VirtualControllerElement
    {
        public InteractorGripPair gripPair;
        public SimpleTransform rig;
        public SimpleTransform targetInRig;
    }

    public class VirtualControllerData
    {
        public List<VirtualControllerElement> elements;

        public VirtualControllerData(List<InteractorGripPair> gripPairs) { 
            elements = new List<VirtualControllerElement>(gripPairs.Count);

            for (var i = 0; i < gripPairs.Count; i++)
            {
                var pair = gripPairs[i];
                var targetData = pair.Interactor.GetTargetData();

                elements.Add(new VirtualControllerElement()
                {
                    gripPair = gripPairs[i],
                    rig = targetData.rig,
                    targetInRig = targetData.targetInRig,
                });
            }
        }
    }
}
