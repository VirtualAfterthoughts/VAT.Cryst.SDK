using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Characters
{
    public interface ISubBehaviour
    {
        void OnRegister(IBehaviourRig behaviourRig);

        void OnDeregister(IBehaviourRig behaviourRig);

        void Solve();
    }
}
