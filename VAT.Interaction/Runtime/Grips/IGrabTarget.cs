using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabTarget
    {
        SimpleTransform GetTargetInHost(IPalm point);

        SimpleTransform GetTargetInWorld(IPalm point);

        SimpleTransform GetTargetInInteractor(IPalm point);

        SimpleTransform GetTargetInWorld(IPalm point, HandPoseData pose);

        SimpleTransform GetTargetInInteractor(IPalm point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInWorld(IPalm point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInInteractor(IPalm point, HandPoseData pose);
    }
}
