using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

using VAT.Avatars;

using VAT.Input;
using VAT.Input.XR;

using VAT.Shared.Data;

namespace VAT.Characters
{
    public struct XRHand : IHand
    {
        public SimpleTransform Transform { get => _transform; set => _transform = value; }

        private SimpleTransform _transform;
        private XRController _controller;

        public XRHand(SimpleTransform transform, XRController controller)
        {
            _transform = transform;
            _controller = controller;
        }

        public bool TryGetInputController(out IInputController controller)
        {
            controller = _controller;
            return true;
        }
    }

    public struct XRArm : IArm
    {
        private IJoint[] _joints;

        public int JointCount => 1;

        public XRArm(XRHand hand)
        {
            _joints = new IJoint[] { hand };
        }

        public IJoint GetJoint(int index)
        {
            return _joints[index];
        }

        public void SetJoint(int index, IJoint joint)
        {
            _joints[index] = joint;
        }

        public bool TryGetElbow(out IJoint elbow)
        {
            elbow = default;
            return false;
        }

        public bool TryGetHand(out IHand hand)
        {
            hand = (XRHand)GetJoint(0);
            return true;
        }

        public bool TryGetUpperArm(out IJoint upperArm)
        {
            upperArm = default;
            return false;
        }
    }

    public class XRControllerRig : ControllerRig {
        public Transform vrRoot;

        private XRApi _api;

        public override void OnAwake()
        {
            _api = new XRApi();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            if (TryGetArm(Handedness.RIGHT, out var arm) && arm.TryGetHand(out var hand))
            {
                hand.TryGetInputController(out var controller);
                controller.TryGetThumbstick(out var thumbstick);

                float turnAxis = thumbstick.GetAxis().x;

                if (Mathf.Abs(turnAxis) > 0.1f)
                {
                    vrRoot.RotateAround(_head.position, vrRoot.up, Time.deltaTime * turnAxis * 200f);
                }

                float crouchAxis = thumbstick.GetAxis().y;

                if (Mathf.Abs(crouchAxis) > 0.1f)
                {
                    float crouchDelta = crouchAxis * Time.deltaTime * 2f;

                    var vrPos = vrRoot.localPosition;
                    vrPos.y = Mathf.Clamp(vrPos.y + crouchDelta, -1.3f, 0f);

                    vrRoot.localPosition = vrPos;
                }
            }
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
        {
            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new XRArm(new XRHand(SimpleTransform.Create(transform).InverseTransform(_leftWrist), _api.LeftController));
                    return true;
                case Handedness.RIGHT:
                    arm = new XRArm(new XRHand(SimpleTransform.Create(transform).InverseTransform(_rightWrist), _api.RightController));
                    return true;
            }
        }
    }
}