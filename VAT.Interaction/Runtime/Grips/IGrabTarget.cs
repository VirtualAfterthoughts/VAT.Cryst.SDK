using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Avatars;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IGrabTarget
    {
        SimpleTransform GetTargetInWorld(IGrabPoint point);

        SimpleTransform GetTargetInInteractor(IGrabPoint point);

        SimpleTransform GetTargetInWorld(IGrabPoint point, HandPoseData pose);

        SimpleTransform GetTargetInInteractor(IGrabPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInWorld(IGrabPoint point, HandPoseData pose);

        SimpleTransform GetDefaultTargetInInteractor(IGrabPoint point, HandPoseData pose);
    }
}
