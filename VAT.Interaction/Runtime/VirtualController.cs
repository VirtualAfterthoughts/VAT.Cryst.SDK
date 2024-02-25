using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public class VirtualController : IInteractorOverride
    {
        private readonly List<InteractorGripPair> _gripPairs = new();

        private readonly List<VirtualControllerOverride> _virtualControllerOverrides = new();

        private VirtualControllerData _currentData;

        public void RegisterOverride(VirtualControllerOverride virtualControllerOverride)
        {
            _virtualControllerOverrides.Add(virtualControllerOverride);
        }

        public void UnregisterOverride(VirtualControllerOverride virtualControllerOverride)
        {
            _virtualControllerOverrides.Remove(virtualControllerOverride);
        }

        public void RegisterPair(IInteractor interactor, Grip grip)
        {
            _gripPairs.Add(new InteractorGripPair(interactor, grip));
            interactor.RegisterOverride(this);
        }

        public void UnregisterPair(IInteractor interactor)
        {
            _gripPairs.RemoveAll((p) => p.Interactor == interactor);
            interactor.UnregisterOverride(this);
        }

        public void Solve()
        {
            _currentData = new VirtualControllerData(_gripPairs);

            foreach (var vcOverride in _virtualControllerOverrides)
            {
                vcOverride.Solve(_currentData);
            }
        }

        SimpleTransform IInteractorOverride.Solve(IInteractor interactor, SimpleTransform rig, SimpleTransform targetInRig)
        {
            if (_currentData != null)
            {
                foreach (var element in _currentData.elements)
                {
                    if (element.gripPair.Interactor == interactor)
                    {
                        return element.targetInRig;
                    }
                }
            }

            return targetInRig;
        }
    }
}
