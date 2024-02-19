using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public interface IGripJoint
    {
        void AttachJoints(IInteractor interactor, Grip grip);

        void DetachJoints();

        void UpdateJoints();

        void FreeJoints();

        void LockJoints();
    }
}