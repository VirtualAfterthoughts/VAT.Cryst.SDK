using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public sealed class InteractableHostManager : MonoBehaviour
    {
        private InteractableHost[] _hosts;

        private void Awake()
        {
            _hosts = GetComponentsInChildren<InteractableHost>();
        }

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
    }
}
