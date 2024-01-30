using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Entities.PhysX;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public sealed class InteractableHost : MonoBehaviour
    {
        private IInteractable[] _interactables;
        private Rigidbody _rb;

        public Rigidbody GetRigidbody()
        {
            return _rb;
        }

        private void Awake()
        {
            _rb = gameObject.GetComponent<Rigidbody>();
            _interactables = GetComponentsInChildren<IInteractable>();
        }

        public void EnableInteraction()
        {
            foreach (var interactable in _interactables)
            {
                interactable.EnableInteraction();
            }
        }

        public void DisableInteraction()
        {
            foreach (var interactable in _interactables)
            {
                interactable.DisableInteraction();
            }
        }
    }
}
