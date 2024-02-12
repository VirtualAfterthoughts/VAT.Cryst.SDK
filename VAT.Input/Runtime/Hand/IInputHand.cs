using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Avatars;

namespace VAT.Input
{
    public interface IInputHand
    {
        public HandPoseData GetHandPose();
    }
}
