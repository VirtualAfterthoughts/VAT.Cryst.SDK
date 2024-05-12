using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private List<HostLink> _links = new();

        public List<HostLink> Links => _links;

        public List<Collider> Colliders => _colliders;

        public VirtualController VirtualController { get; } = new VirtualController();

        private InteractableHostGroup _selfGroup = null;

        public InteractableHostGroup SelfGroup => _selfGroup;

        public Rigidbody GetRigidbody()
        {
            return _rb;
        }

        public void Link(HostLink link)
        {
            _links.Add(link);
        }

        public void Unlink(HostLink link)
        {
            _links.Remove(link);
        }

        public void AttachGroup(InteractableHostGroup group)
        {
            var link = new HostLink() { host = this, linkedGroup = group };
            link.Attach();
        }

        public void DetachGroup(InteractableHostGroup group)
        {
            var linksToDetach = new List<HostLink>();
            foreach (var link in Links)
            {
                if (link.linkedGroup == group)
                {
                    linksToDetach.Add(link);
                }
            }

            foreach (var link in linksToDetach)
            {
                link.Detach();
            }
        }

        public Collider[] GetColliders()
        {
            return Colliders.ToArray();
        }

        private void Awake()
        {
            _selfGroup = new InteractableHostGroup(this);

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

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;   
            }

            Gizmos.color = Color.magenta;

            foreach (var link in Links)
            {
                foreach (var host in link.linkedGroup.hosts)
                {
                    Gizmos.DrawLine(transform.position, host.transform.position);
                }
            }
        }
#endif
    }
}
