using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public sealed class InteractableHostManager : MonoBehaviour
    {
        private List<InteractableHost> _hosts = new();

        public void EnableInteraction()
        {
            foreach (var host in _hosts)
            {
                host.EnableInteraction();
            }
        }

        public void DisableInteraction()
        {
            foreach (var host in _hosts)
            {
                host.DisableInteraction();
            }
        }

        public void RegisterHost(InteractableHost host)
        {
            _hosts.Add(host);
        }

        public void UnregisterHost(InteractableHost host)
        {
            _hosts.Remove(host);
        }
    }
}
