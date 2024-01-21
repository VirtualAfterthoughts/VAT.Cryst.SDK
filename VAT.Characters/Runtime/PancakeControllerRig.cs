using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Input;
using VAT.Input.Desktop;

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

    public class PancakeControllerRig : ControllerRig {
        private DesktopInputActions _inputActions;

        public override void OnAwake()
        {
            _inputActions = new DesktopInputActions();
            _inputActions.Enable();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            SolveCamera(deltaTime);
        }

        private Vector2 _headAxis;

        private void SolveCamera(float deltaTime)
        {
            // Camera control
            float speed = 150f * deltaTime;
            Cursor.lockState = CursorLockMode.Locked;

            var action = _inputActions.Desktop.Look;
            var lookDelta = action.ReadValue<Vector2>();

            if (action.activeControl?.device.description.deviceClass == "Mouse")
            {
                lookDelta /= deltaTime;
                lookDelta *= 0.001f;
            }

            _headAxis += lookDelta * speed;
            _headAxis.y = Mathf.Clamp(_headAxis.y, -80f, 80f);

            _head.rotation = Quaternion.AngleAxis(_headAxis.x, transform.up) * Quaternion.AngleAxis(_headAxis.y, -transform.right) * transform.rotation;
        }

        public override bool TryGetInput(out IBasicInput input)
        {
            var movementAxis = _inputActions.Desktop.Movement.ReadValue<Vector2>();
            var movement = _head.rotation * new Vector3(movementAxis.x, 0f, movementAxis.y);

            var jump = _inputActions.Desktop.Jump.ReadValue<float>();

            input = new PancakeInput(movement, jump >= 0.5f);
            return true;
        }
    }
}
