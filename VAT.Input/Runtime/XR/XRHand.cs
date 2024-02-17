using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;

namespace VAT.Input
{
    public class XRHand : IInputHand
    {
        private readonly IXRPoseProvider[] _providers;

        public XRHand(params IXRPoseProvider[] providers) {
            _providers = providers;
        }

        public HandPoseData GetHandPose()
        {
            foreach (var provider in _providers)
            {
                if (!provider.IsValid())
                {
                    continue;
                }

                return provider.GetHandPose();
            }

            return new HandPoseData();
        }
    }
}
