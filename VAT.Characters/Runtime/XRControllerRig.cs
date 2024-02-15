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
    public readonly struct XRInput : IBasicInput
    {
        private readonly Vector3 _movement;
        private readonly bool _jump;

        public XRInput(Vector3 movement, bool jump)
        {
            _movement = movement;
            _jump = jump;
        }

        public readonly bool GetJump()
        {
            return _jump;
        }

        public readonly Vector3 GetMovement()
        {
            return _movement;
        }
    }

    public struct XRHand : IHand
    {
        public SimpleTransform Transform { get => _transform; set => _transform = value; }

        private SimpleTransform _transform;
        private XRController _controller;
        private Input.XRHand _hand;

        public XRHand(SimpleTransform transform, XRController controller, Input.XRHand hand)
        {
            _transform = transform;
            _controller = controller;
            _hand = hand;
        }

        public IInputController GetInputControllerOrDefault()
        {
            return _controller;
        }

        public IInputHand GetInputHandOrDefault()
        {
            return _hand;
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
                var controller = hand.GetInputControllerOrDefault();
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

        public override bool TryGetInput(out IBasicInput input)
        {
            _api.LeftController.TryGetThumbstick(out var thumbstick);

            var movementAxis = thumbstick.GetAxis();
            var movement = _head.rotation * new Vector3(movementAxis.x, 0f, movementAxis.y);

            _api.RightController.TryGetPrimaryButton(out var button);

            var jump = button.GetPressed();

            input = new PancakeInput(movement, jump);
            return true;
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
        {
            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new XRArm(new XRHand(SimpleTransform.Create(transform).InverseTransform(_leftWrist), _api.LeftController, _api.LeftHand));
                    return true;
                case Handedness.RIGHT:
                    arm = new XRArm(new XRHand(SimpleTransform.Create(transform).InverseTransform(_rightWrist), _api.RightController, _api.RightHand));
                    return true;
            }
        }
    }
}
