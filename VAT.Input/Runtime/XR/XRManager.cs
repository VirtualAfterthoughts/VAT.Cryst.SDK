using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input.XR;

namespace VAT.Input.XR
{
    public class XRManager
    {
        private readonly XRHand _leftHand;
        private readonly XRHand _rightHand;

        private XRInputActions _actions;

        public XRHand LeftHand
        { 
            get
            {
                return _leftHand;
            } 
        }

        public XRHand RightHand
        {
            get
            {
                return _rightHand;
            }
        }

        public XRInputActions Actions
        {
            get
            {
                return _actions;
            }
        }

        public XRManager()
        {
            _actions = new XRInputActions();
            _leftHand = new XRHand(_actions, Handedness.LEFT);
            _rightHand = new XRHand(_actions, Handedness.RIGHT);
        }
    }
}
