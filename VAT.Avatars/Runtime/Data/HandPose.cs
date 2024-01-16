using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Data {
    [Serializable]
    public struct PoseData {
        public FingerPose[] fingers;
    }

    [Serializable]
    public struct FingerPose {
        public SimpleTransform point;
    }

    public abstract class HandPose : ScriptableObject, IHandPose {
        public abstract string Address { get; }

        public MasterHandPoseContentReference masterPose;

        public PoseData data;
        
        public void OnLoaded() {
            if (masterPose.TryGetContent(out var content)) {
                content.MainScriptableObject.LoadAsset((v) => {
                    if (v is MasterHandPose pose) {
                        pose.InjectPose(this);
                    }
                });
            }
        }
    }
}
