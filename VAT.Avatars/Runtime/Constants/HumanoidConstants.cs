using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Constants
{
    public static class HumanoidConstants {
        #region ANGULAR LIMITS
        // Neck
        public static readonly JointAngularLimits SkullLimits = new(-60f, 60f, 0f, 10f);
        public static readonly JointAngularLimits UpperNeckLimits = new(-20f, 20f, 47f, 0f);
        public static readonly JointAngularLimits LowerNeckLimits = new(-20f, 20f, 11f, 11f);

        // Spine
        public static readonly JointAngularLimits UpperChestLimits = new(-2f, 2f, 9f, 6f);
        public static readonly JointAngularLimits ChestLimits = new(-30f, 15f, 20f, 6f);
        public static readonly JointAngularLimits SpineLimits = new(-75f, 30f, 0f, 35f);
        public static readonly JointAngularLimits PelvisLimits = JointAngularLimits.Free;

        // Arm
        public static readonly JointAngularLimits ClavicleLimits = new(20f, 20f, 20f, 20f);
        public static readonly JointAngularLimits ScapulaLimits = new(-30f, 30f, 30f, 30f);
        public static readonly JointAngularLimits UpperArmLimits = JointAngularLimits.Free;
        public static readonly JointAngularLimits ElbowLimits = new(-150f, 10f, 160f, 150f);
        public static readonly JointAngularLimits HandLimits = JointAngularLimits.Free;

        // Leg
        public static readonly JointAngularLimits HipLimits = new(-45f, 160f, 36f, 40f);
        public static readonly JointAngularLimits KneeLimits = new(-140f, 0f, 0f, 0f);
        public static readonly JointAngularLimits AnkleLimits = new(-30f, 50f, 0f, 0f);
        #endregion
    }
}
