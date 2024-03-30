using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Entities.PhysX;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractableHost : MonoBehaviour
    {
        private List<IInteractable> _interactables = new();
        private Rigidbody _rb;

        private readonly List<Collider> _colliders = new List<Collider>();

        private InteractableHostManager _manager;

        public List<Collider> Colliders => _colliders;

        public VirtualController VirtualController { get; } = new VirtualController();

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

        private void OnEnable()
        {
            FindManager();
        }

        private void OnDisable()
        {
            UnregisterManager();
        }

        public void FindManager()
        {
            UnregisterManager();

            _manager = GetComponentInParent<InteractableHostManager>();

            if (_manager != null)
            {
                _manager.RegisterHost(this);
            }
        }

        public void UnregisterManager()
        {
            if (_manager != null)
            {
                _manager.UnregisterHost(this);
                _manager = null;
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
