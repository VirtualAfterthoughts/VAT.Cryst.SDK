using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;

namespace VAT.Input
{
    public class XRHand : IInputHand
    {
        public XRHand() { 
        }

        public HandPoseData GetHandPose()
        {
            return new HandPoseData();
        }
    }
}
