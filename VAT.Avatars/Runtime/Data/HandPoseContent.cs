using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

using VAT.Serialization.JSON;

namespace VAT.Avatars.Data
{
    [StaticContentIdentifier("Hand Pose", typeof(HandPose))]
    public sealed class HandPoseContent : StaticScriptableObjectContent {
#if UNITY_EDITOR
        public override string EditorAssetGroup => "HandPose";
#endif

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json) {
#if UNITY_EDITOR
            if (Application.isPlaying) {
#endif

                MainScriptableObject?.LoadAsset((v) => {
                    if (v is HandPose pose) {
                        pose.OnLoaded();
                    }
                });

#if UNITY_EDITOR
            }
#endif
            }
    }
}
