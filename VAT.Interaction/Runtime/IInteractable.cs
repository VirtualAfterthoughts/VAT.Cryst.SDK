using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IInteractable
    {
        bool IsInteractable();

        void EnableInteraction();

        void DisableInteraction();

        void OnHoverBegin(IInteractor interactor);

        void OnHoverEnd(IInteractor interactor);

        float GetPriority(IInteractor interactor);
    }
}
