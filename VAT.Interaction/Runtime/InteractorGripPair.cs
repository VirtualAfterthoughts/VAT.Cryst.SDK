using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractorGripPair
    {
        public IInteractor Interactor { get; }
        public Grip Grip { get; }

        public InteractorGripPair(IInteractor interactor, Grip grip)
        {
            Interactor = interactor;
            Grip = grip;
        }
    }
}
