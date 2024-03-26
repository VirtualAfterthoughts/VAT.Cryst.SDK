using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Input;
using VAT.Input.Desktop;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public readonly struct PancakeInput : IBasicInput
    {
        private readonly Vector3 _movement;
        private readonly bool _jump;

        public PancakeInput(Vector3 movement, bool jump)
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

    public struct PancakeHand : IHand
    {
        public SimpleTransform Transform { get => _transform; set => _transform = value; }

        private SimpleTransform _transform;
        private DesktopController _controller;
        private DesktopHand _hand;

        public PancakeHand(SimpleTransform transform, DesktopController controller, DesktopHand hand)
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

    public struct PancakeArm : IArm
    {
        private IJoint[] _joints;

        public int JointCount => 1;

        public PancakeArm(PancakeHand hand)
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
            hand = (PancakeHand)GetJoint(0);
            return true;
        }

        public bool TryGetUpperArm(out IJoint upperArm)
        {
            upperArm = default;
            return false;
        }
    }

    public class PancakeControllerRig : ControllerRig {
        public Transform neckPivot;

        private DesktopInputActions _inputActions;
        private DesktopController _leftController;
        private DesktopController _rightController;

        private DesktopHand _leftHand;
        private DesktopHand _rightHand;

        public override void OnAwake()
        {
            _inputActions = new DesktopInputActions();
            _inputActions.Enable();

            _leftController = new DesktopController(Handedness.LEFT, _inputActions);
            _rightController = new DesktopController(Handedness.RIGHT, _inputActions);

            _leftHand = new DesktopHand(_leftController);
            _rightHand = new DesktopHand(_rightController);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            SolveCamera(deltaTime);

            _leftHand.Update();
            _rightHand.Update();
        }

        private Vector2 _headAxis;

        private void SolveCamera(float deltaTime)
        {
            // Camera control
            float speed = 150f * deltaTime;
            Cursor.lockState = CursorLockMode.Locked;

            var action = _inputActions.Gameplay.Look;
            var lookDelta = action.ReadValue<Vector2>();

            if (action.activeControl?.device.description.deviceClass == "Mouse")
            {
                lookDelta /= deltaTime;
                lookDelta *= 0.001f;
            }

            _headAxis += lookDelta * speed;
            _headAxis.y = Mathf.Clamp(_headAxis.y, -80f, 80f);

            neckPivot.rotation = Quaternion.AngleAxis(_headAxis.x, transform.up) * Quaternion.AngleAxis(_headAxis.y, -transform.right) * transform.rotation;
        }

        public override bool TryGetInput(out IBasicInput input)
        {
            var movementAxis = _inputActions.Gameplay.Movement.ReadValue<Vector2>();
            var flattenedHead = Quaternion.LookRotation(neckPivot.forward.FlattenNeck(neckPivot.up, transform.up), transform.up);
            var movement = flattenedHead * new Vector3(movementAxis.x, 0f, movementAxis.y);

            var jump = _inputActions.Gameplay.Jump.ReadValue<float>();

            input = new PancakeInput(movement, jump >= 0.5f);
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
                    arm = new PancakeArm(new PancakeHand(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_leftWrist)), _leftController, _leftHand));
                    return true;
                case Handedness.RIGHT:
                    arm = new PancakeArm(new PancakeHand(SimpleTransform.Create(transform).InverseTransform(SimpleTransform.Create(_rightWrist)), _rightController, _rightHand));
                    return true;
            }
        }
    }
}
