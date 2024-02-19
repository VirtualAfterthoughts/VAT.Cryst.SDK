using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Entities.PhysX;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public sealed class InteractableHost : MonoBehaviour
    {
        private List<IInteractable> _interactables = new();
        private Rigidbody _rb;

        private readonly List<Collider> _colliders = new List<Collider>();

        public List<Collider> Colliders => _colliders;

        public Rigidbody GetRigidbodyOrDefault()
        {
            return _rb;
        }

        public Collider[] GetColliders()
        {
            return Colliders.ToArray();
        }

        private void Awake()
        {
            _rb = gameObject.GetComponent<Rigidbody>();

            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                if (collider.GetComponentInParent<InteractableHost>() == this)
                {
                    _colliders.Add(collider);
                }
            }
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

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void RegisterInteractable(IInteractable interactable)
        {
            _interactables.Add(interactable);
        }

        public void UnregisterInteractable(IInteractable interactable)
        {
            _interactables.Remove(interactable);
        }
    }
}
