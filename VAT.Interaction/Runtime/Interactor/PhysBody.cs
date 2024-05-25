using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public class PhysBody : MonoBehaviour
    {
        [SerializeField]
        private InteractableHost[] _bodyHosts = new InteractableHost[0];

        [SerializeField]
        private PhysLimb[] _limbs = new PhysLimb[0];

        public InteractableHost[] BodyHosts { get { return _bodyHosts; } set { _bodyHosts = value; } }

        public PhysLimb[] Limbs 
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

        private void AttachLimbs(PhysLimb[] limbs)
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

        private void DetachLimbs(PhysLimb[] limbs)
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
