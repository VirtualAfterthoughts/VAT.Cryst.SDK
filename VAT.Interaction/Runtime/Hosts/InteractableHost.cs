using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public partial class InteractableHost : MonoBehaviour, IHost
    {
        public bool HasRigidbody => _hasRigidbody;

        public IReadOnlyList<IHostable> Hostables => _hostables;

        private bool _hasRigidbody;
        private Rigidbody _rigidbody;

        private readonly List<IHostable> _hostables = new();

        private IHostManager _manager;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _hasRigidbody = _rigidbody != null;
            
            // Find all interactables
            foreach (var interactable in GetComponentsInChildren<Interactable>())
            {
                if (interactable.GetComponentInParent<InteractableHost>() == this)
                {
                    interactable.InjectHost(this);
                    _hostables.Add(interactable);
                }
            }
        }

        public void InjectManager(IHostManager manager)
        {
            _manager = manager;
        }

        public GameObject GetHostGameObject()
        {
            return gameObject;
        }

        public Rigidbody GetHostRigidbody()
        {
            return _rigidbody;
        }

        public GameObject GetManagerGameObject()
        {
            return _manager.GetManagerGameObject();
        }
    }
}
