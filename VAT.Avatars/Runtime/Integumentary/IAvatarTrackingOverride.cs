using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars
{
    public interface IAvatarTrackingOverride
    {
        public SimpleTransform Solve(SimpleTransform rig, SimpleTransform targetInRig);
    }
}
