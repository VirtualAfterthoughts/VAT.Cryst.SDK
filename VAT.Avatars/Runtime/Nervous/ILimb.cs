using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars
{
    public interface ILimb
    {
        int JointCount { get; }

        IJoint GetJoint(int index);

        void SetJoint(int index, IJoint joint);
    }
}
