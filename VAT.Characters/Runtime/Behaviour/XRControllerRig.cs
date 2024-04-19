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
    public class XRControllerRig : ControllerRig {
        private HandActions _leftActions = new();
        private HandActions _rightActions = new();

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
            _leftWrist.transform.position = vrRoot.TransformPoint(leftWrist.position);
            _leftWrist.transform.rotation = vrRoot.TransformRotation(leftWrist.rotation);

            var rightWrist = XRManager.Api.RightHand.GetWristTransform();
            _rightWrist.transform.position = vrRoot.TransformPoint(rightWrist.position);
            _rightWrist.transform.rotation = vrRoot.TransformRotation(rightWrist.rotation);

            _leftActions.UpdateActions(XRManager.Api.LeftHand, XRManager.Api.LeftController);
            _rightActions.UpdateActions(XRManager.Api.RightHand, XRManager.Api.RightController);
        }

        protected override Vector3 OnProcessMovement()
        {
            var thumbstick = XRManager.Api.LeftController.GetThumbstickOrNull();

            var movementAxis = thumbstick.GetAxis();
            var flattenedHead = Quaternion.LookRotation(_head.forward.FlattenNeck(_head.up, transform.up), transform.up);
            var movement = flattenedHead * new Vector3(movementAxis.x, 0f, movementAxis.y);

            return movement;
        }

        protected override bool OnProcessJump()
        {
            var button = XRManager.Api.RightController.GetPrimaryButtonOrNull();

            return button.GetPressed();
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
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
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_leftWrist)), XRManager.Api.LeftController, XRManager.Api.LeftHand, _leftActions));
                    return true;
                case Handedness.RIGHT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_rightWrist)), XRManager.Api.RightController, XRManager.Api.RightHand, _leftActions));
                    return true;
            }
        }

        protected override void OnProcessTracking()
        {
            
        }
    }
}
