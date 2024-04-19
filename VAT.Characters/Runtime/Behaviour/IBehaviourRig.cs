using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.Skeleton;

using VAT.Shared.Data;

namespace VAT.Characters
{
    public interface IBehaviourRig : ICrystRig
    {
        SimpleTransform GetRoot();

        void SetRoot(SimpleTransform root);

        SimpleTransform GetBehaviourSpace();

        void SetBehaviourSpace(SimpleTransform transform);

        SimpleTransform GetLocalHead();

        IHand GetPrimaryHand();

        IHand GetSecondaryHand();
    }
}
