using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VAT.Avatars;

using VAT.Shared.Data;

namespace VAT.Input
{
    public interface IInputHand
    {
        HandPoseData GetHandPose();

        SimpleTransform GetWristTransform();
    }
}
