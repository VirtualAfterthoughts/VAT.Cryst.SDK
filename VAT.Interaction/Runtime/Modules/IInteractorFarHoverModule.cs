using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public interface IInteractorFarHoverModule : IInteractorModule
    {
        IInteractable GetFarHoveringInteractable();
    }
}
