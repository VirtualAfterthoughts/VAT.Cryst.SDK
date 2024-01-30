using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class TargetGrip : Grip
    {
        public Transform target;

        public override void DisableInteraction()
        {
            throw new System.NotImplementedException();
        }

        public override void EnableInteraction()
        {
            throw new System.NotImplementedException();
        }

        public override float GetPriority(IInteractor interactor)
        {
            throw new System.NotImplementedException();
        }

        public override SimpleTransform GetTargetInWorld(IInteractor interactor)
        {
            return SimpleTransform.Create(target);
        }

        public override bool IsInteractable()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHoverBegin(IInteractor interactor)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnHoverEnd(IInteractor interactor)
        {
            //throw new System.NotImplementedException();
        }
    }
}
