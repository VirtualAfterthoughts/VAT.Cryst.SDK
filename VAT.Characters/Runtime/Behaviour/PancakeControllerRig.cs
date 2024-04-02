using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Input;
using VAT.Input.Desktop;
using VAT.Shared.Data;
using VAT.Input.Skeleton;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public class PancakeControllerRig : ControllerRig {
        public Transform neckPivot;

        private DesktopInputActions _inputActions;
        private DesktopController _leftController;
        private DesktopController _rightController;

        private DesktopHand _leftHand;
        private DesktopHand _rightHand;

        public override void OnRigEnable()
        {
            base.OnRigEnable();

            _inputActions = new DesktopInputActions();
            _inputActions.Enable();

            _leftController = new DesktopController(Handedness.LEFT, _inputActions);
            _rightController = new DesktopController(Handedness.RIGHT, _inputActions);

            _leftHand = new DesktopHand(_leftController);
            _rightHand = new DesktopHand(_rightController);
        }

        private Vector2 _headAxis;

        protected override void OnProcessTracking()
        {
            float deltaTime = Time.deltaTime;

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

            neckPivot.rotation = Quaternion.AngleAxis(_headAxis.x, vrRoot.up) * Quaternion.AngleAxis(_headAxis.y, -vrRoot.right) * vrRoot.rotation;

            // Update hands
            _leftHand.Update();
            _rightHand.Update();
        }

        protected override bool OnProcessJump()
        {
            return _inputActions.Gameplay.Jump.ReadValue<float>() > 0.5f;
        }

        protected override Vector3 OnProcessMovement()
        {
            var movementAxis = _inputActions.Gameplay.Movement.ReadValue<Vector2>();
            var flattenedHead = Quaternion.LookRotation(neckPivot.forward.FlattenNeck(neckPivot.up, transform.up), transform.up);
            var movement = flattenedHead * new Vector3(movementAxis.x, 0f, movementAxis.y);

            return movement;
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
        {
            var root = SimpleTransform.Create(transform);

            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_leftWrist)), _leftController, _leftHand));
                    return true;
                case Handedness.RIGHT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_rightWrist)), _rightController, _rightHand));
                    return true;
            }
        }
    }
}
