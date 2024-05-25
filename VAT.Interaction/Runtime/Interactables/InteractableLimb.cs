using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractableLimb : MonoBehaviour
    {
        [SerializeField]
        private InteractableHost[] _limbHosts = new InteractableHost[0];

        public InteractableHost[] LimbHosts { 
            get 
            {
                return _limbHosts; 
            } 
            set 
            { 
                _limbHosts = value;

                _limbGroup = new InteractableHostGroup(value);
            } 
        }

        private InteractableSpine _parentBody = null;

        public InteractableSpine ParentBody { get { return _parentBody; } set { _parentBody = value; } }

        private InteractableHostGroup _limbGroup = null;
        public InteractableHostGroup LimbGroup { get { return _limbGroup; } }

        private void Awake()
        {
            _limbGroup = new InteractableHostGroup(_limbHosts);
        }
    }
}
