using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;
using VAT.Shared.Data;

namespace VAT.Input
{
    public class XRHand
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

        public SimpleTransform GetWristTransform()
        {
            foreach (var provider in _providers)
            {
                if (!provider.IsValid())
                {
                    continue;
                }

                return provider.GetWristTransform();
            }

            return SimpleTransform.Default;
        }
    }
}
