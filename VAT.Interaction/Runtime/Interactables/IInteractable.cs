using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Interaction.Entities;

namespace VAT.Interaction
{
    public interface IInteractable : IHoverable
    {
        bool IsInteractable();

        void EnableInteraction();

        void DisableInteraction();

        (bool valid, float priority) ValidateInteraction(IInteractor interactor);

        GameObject GetHostGameObject();

        CrystBody GetHost();

        void FindHost();
    }
}
