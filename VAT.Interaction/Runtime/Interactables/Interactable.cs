using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public partial class Interactable : MonoBehaviour, IInteractable
    {
        public bool HasRigidbody => _host.HasRigidbody;

        public IHost Host => _host;
        private IHost _host = null;

        protected virtual void OnEnable()
        {
            IInteractable.Cache.Add(gameObject, this);
        }

        protected virtual void OnDisable()
        {
            IInteractable.Cache.Remove(gameObject, this);

            DetachGrabbers();
        }

        public void InjectHost(IHost host)
        {
            _host = host;
        }

        public Rigidbody GetHostRigidbody()
        {
            return _host.GetHostRigidbody();
        }

        public GameObject GetHostGameObject()
        {
            return _host.GetHostGameObject();
        }
    }
}
