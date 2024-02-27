using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractorGripPair
    {
        public IInteractor Interactor { get; }
        public IGrippable Grip { get; }

        public InteractorGripPair(IInteractor interactor, IGrippable grip)
        {
            Interactor = interactor;
            Grip = grip;
        }
    }
}
