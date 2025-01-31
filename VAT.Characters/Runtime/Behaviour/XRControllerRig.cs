using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

using VAT.Avatars;

using VAT.Input.Skeleton;
using VAT.Input;
using VAT.Input.XR;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public class XRControllerRig : ControllerRig
    {
        public override void OnRigEnable()
        {
            base.OnRigEnable();

            XRManager.InitializeApiAsync();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            if (!XRManager.HasApi())
            {
                return;
            }

            base.OnLateUpdate(deltaTime);

            var leftWrist = XRManager.Api.LeftHand.GetWristTransform();
            _leftWrist.transform.SetPositionAndRotation(vrRoot.TransformPoint(leftWrist.position), vrRoot.TransformRotation(leftWrist.rotation));

            var rightWrist = XRManager.Api.RightHand.GetWristTransform();
            _rightWrist.transform.SetPositionAndRotation(vrRoot.TransformPoint(rightWrist.position), vrRoot.TransformRotation(rightWrist.rotation));

            XRManager.Api.UpdateApi();
        }

        protected override Vector3 OnProcessMovement()
        {
            var thumbstick = XRManager.Api.LeftController.GetThumbstick();

            var movementAxis = thumbstick.GetAxis();
            var flattenedHead = Quaternion.LookRotation(_head.forward.FlattenNeck(_head.up, transform.up), transform.up);
            var movement = flattenedHead * new Vector3(movementAxis.x, 0f, movementAxis.y);

            return movement;
        }

        protected override bool OnProcessJump()
        {
            var button = XRManager.Api.RightController.GetPrimaryButton();

            return button.GetPressed();
        }

        public override bool TryGetArm(Handedness handedness, out IInputArm arm)
        {
            if (!XRManager.HasApi())
            {
                arm = default;
                return false;
            }

            var root = GetRoot();

            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_leftWrist.position, _leftWrist.rotation)), XRManager.Api.LeftController));
                    return true;
                case Handedness.RIGHT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_rightWrist.position, _rightWrist.rotation)), XRManager.Api.RightController));
                    return true;
            }
        }

        protected override void OnProcessTracking()
        {

        }
    }
}
