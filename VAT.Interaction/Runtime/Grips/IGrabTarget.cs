using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Data;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabTarget
    {
        SimpleTransform GetTargetInHost(PalmPoint point);

        SimpleTransform GetTargetInWorld(PalmPoint point);

        SimpleTransform GetTargetInInteractor(PalmPoint point);

        SimpleTransform GetTargetInWorld(PalmPoint point, HandPoseData pose);

        SimpleTransform GetTargetInInteractor(PalmPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInWorld(PalmPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInInteractor(PalmPoint point, HandPoseData pose);
    }
}
