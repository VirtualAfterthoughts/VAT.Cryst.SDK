using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabTarget
    {
        SimpleTransform GetTargetInHost(IPalmPoint point);

        SimpleTransform GetTargetInWorld(IPalmPoint point);

        SimpleTransform GetTargetInInteractor(IPalmPoint point);

        SimpleTransform GetTargetInWorld(IPalmPoint point, HandPoseData pose);

        SimpleTransform GetTargetInInteractor(IPalmPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInWorld(IPalmPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInInteractor(IPalmPoint point, HandPoseData pose);
    }
}
