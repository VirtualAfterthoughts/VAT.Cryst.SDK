using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Avatars.Data
{
    [CreateAssetMenu(fileName = "New Humanoid Hand Pose", menuName = "VAT/Data/Poses/Humanoid Hand Pose")]
    public sealed class HumanoidHandPose : HandPose {
        public const string PoseAddress = "HumanoidHandPose";

        public override string Address => PoseAddress;
    }
}
