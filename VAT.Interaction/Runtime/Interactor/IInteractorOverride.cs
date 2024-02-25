using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public interface IInteractorOverride
    {
        SimpleTransform OnOverrideTarget(IInteractor interactor, SimpleTransform rig, SimpleTransform targetInRig);
    }
}
