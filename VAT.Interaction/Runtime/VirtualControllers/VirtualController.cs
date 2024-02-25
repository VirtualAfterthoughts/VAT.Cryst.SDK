using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class VirtualController : IInteractorOverride
    {
        private readonly List<IVirtualControllerOverride> _controllerOverrides = new();

        private readonly List<InteractorGripPair> _gripPairs = new();

        public void RegisterOverride(IVirtualControllerOverride controllerOverride)
        {
            _controllerOverrides.Add(controllerOverride);
        }

        public void UnregisterOverride(IVirtualControllerOverride controllerOverride) 
        {
            _controllerOverrides.Remove(controllerOverride);
        }

        public InteractorGripPair GetGripPair(IInteractor interactor)
        {
            return _gripPairs.Find((p) => p.Interactor == interactor);
        }

        public void RegisterPair(IInteractor interactor, Grip grip)
        {
            _gripPairs.Add(new InteractorGripPair(interactor, grip));

            interactor.RegisterOverride(this);
        }

        public void UnregisterPair(IInteractor interactor)
        {
            var gripPair = GetGripPair(interactor);
            if (gripPair != null)
            {
                _gripPairs.Remove(gripPair);
            }

            interactor.UnregisterOverride(this);
        }

        public SimpleTransform OnOverrideTarget(IInteractor interactor, SimpleTransform rig, SimpleTransform targetInRig)
        {
            var gripPair = GetGripPair(interactor);

            if (gripPair != null)
            {
                VirtualControllerPayload payload = new()
                {
                    interactorGripPair = gripPair,
                    rig = rig,
                    targetInRig = targetInRig
                };

                foreach (var controllerOverride in _controllerOverrides)
                {
                    controllerOverride.OnSolveController(payload);
                }

                return payload.targetInRig;
            }


            return targetInRig;
        }
    }
}
