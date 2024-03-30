using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IInteractable
    {
        event InteractorDelegate OnHoverBegin, OnHoverEnd;

        bool IsInteractable();

        void EnableInteraction();

        void DisableInteraction();

        void BeginHover(IInteractor interactor);

        void EndHover(IInteractor interactor);

        (bool valid, float priority) ValidateInteractable(IInteractor interactor);

        InteractableHost GetHostOrDefault();

        void FindHost();
    }
}
