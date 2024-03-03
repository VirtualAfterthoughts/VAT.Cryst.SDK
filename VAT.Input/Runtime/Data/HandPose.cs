using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Avatars;

namespace VAT.Input
{
    [CreateAssetMenu(fileName = "New Hand Pose", menuName = "Cryst/Input/Hand Pose")]
    public class HandPose : ScriptableObject
    {
        public HandPoseData data;

        public Mesh previewMesh;

        public Vector2 centerOfPressure = Vector2.up;

        public Quaternion rotationOffset = Quaternion.identity;
    }
}
