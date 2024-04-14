using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IInteractable : IHoverable
    {
        bool IsInteractable();

        void EnableInteraction();

        void DisableInteraction();

        (bool valid, float priority) ValidateInteraction(IInteractor interactor);

        InteractableHost GetHostOrDefault();

        void FindHost();
    }
}
