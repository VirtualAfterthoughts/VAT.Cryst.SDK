using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractableHostManager : MonoBehaviour, IHostManager
    {
        public IReadOnlyList<IHost> Hosts => throw new System.NotImplementedException();

        private readonly List<IHost> _hosts = new();

        private void Awake()
        {
            foreach (var interactableHost in GetComponentsInChildren<InteractableHost>())
            {
                interactableHost.InjectManager(this);
                _hosts.Add(interactableHost);
            }
        }

        public GameObject GetManagerGameObject()
        {
            return gameObject;
        }
    }
}
