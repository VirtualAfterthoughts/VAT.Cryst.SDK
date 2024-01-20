using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars
{
    public static class LimbExtensions
    {
        public static TLimb TransformLimb<TLimb>(this SimpleTransform transform, TLimb limb) where TLimb : ILimb
        {
            for (var i = 0; i < limb.JointCount; i++)
            {
                var joint = limb.GetJoint(i);

                joint.Transform = transform.Transform(joint.Transform);

                limb.SetJoint(i, joint);
            }

            return limb;
        }

        public static TLimb InverseTransformLimb<TLimb>(this SimpleTransform transform, TLimb limb) where TLimb : ILimb
        {
            for (var i = 0; i < limb.JointCount; i++)
            {
                var joint = limb.GetJoint(i);

                joint.Transform = transform.InverseTransform(joint.Transform);

                limb.SetJoint(i, joint);
            }

            return limb;
        }
    }

    public interface ILimb
    {
        int JointCount { get; }

        IJoint GetJoint(int index);

        void SetJoint(int index, IJoint joint);
    }
}
