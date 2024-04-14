using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class HoverHolder
    {

        private IInteractable _hoveringInteractable = null;
        public IInteractable HoveringInteractable
        {
            get
            {
                return _hoveringInteractable;
            }
            set
            {
                // Make sure this is a different interactable
                if (_hoveringInteractable != value)
                {
                    // Begin hover
                    if (_hoveringInteractable == null)
                    {
                        value.BeginHover(_interactor);
                    }
                    // End hover
                    else
                    {
                        _hoveringInteractable.EndHover(_interactor);

                        value?.BeginHover(_interactor);
                    }

                    _hoveringInteractable = value;
                }
            }
        }

        private readonly IInteractor _interactor = null;

        public HoverHolder(IInteractor interactor)
        {
            _interactor = interactor;
        }
    }
}
