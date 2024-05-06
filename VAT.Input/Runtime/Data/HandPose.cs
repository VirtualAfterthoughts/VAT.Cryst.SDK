using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Input.Data
{
    [CreateAssetMenu(fileName = "New Hand Pose", menuName = "Cryst/Input/Hand Pose")]
    public class HandPose : ScriptableObject
    {
        public HandPoseData data;
    }
}
