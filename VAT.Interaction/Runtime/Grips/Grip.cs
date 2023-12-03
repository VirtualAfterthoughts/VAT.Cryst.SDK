using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Interaction
{
    public abstract class Grip : Interactable
    {
        [SerializeField]
        private Transform _targetTransform;

        protected virtual void Awake()
        {
            if (_targetTransform == null)
            {
                _targetTransform = transform;
            }
        }

        public override bool CanAttach(IGrabber grabber)
        {
            return true;
        }

        public override SimpleTransform GetTargetInWorld(IGrabber grabber)
        {
            return SimpleTransform.Create(_targetTransform);
        }
    }
}
