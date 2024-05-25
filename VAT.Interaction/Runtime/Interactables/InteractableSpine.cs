using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class InteractableSpine : MonoBehaviour
    {
        [SerializeField]
        private InteractableHost[] _spineHosts = new InteractableHost[0];

        [SerializeField]
        private InteractableLimb[] _limbs = new InteractableLimb[0];

        public InteractableHost[] SpineHosts { get { return _spineHosts; } set { _spineHosts = value; } }

        public InteractableLimb[] Limbs 
        {
            get 
            {
                return _limbs; 
            } 
            set 
            {
                DetachLimbs(_limbs);

                _limbs = value;

                AttachLimbs(value);
            } 
        }

        private void Awake()
        {
            AttachLimbs(_limbs);
        }

        private void AttachLimbs(InteractableLimb[] limbs)
        {
            foreach (var limb in limbs)
            {
                if (!limb)
                {
                    continue;
                }

                limb.ParentBody = this;
            }
        }

        private void DetachLimbs(InteractableLimb[] limbs)
        {
            foreach (var limb in limbs)
            {
                if (!limb)
                {
                    continue;
                }

                limb.ParentBody = null;
            }
        } 
    }
}
