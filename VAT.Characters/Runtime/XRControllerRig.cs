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

            if (TryGetArm(Handedness.RIGHT, out var arm))
            {
                var hand = arm.GetHandOrNull();

                var controller = hand.GetInputControllerOrNull();
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
            XRManager.Api.LeftController.TryGetThumbstick(out var thumbstick);

            var movementAxis = thumbstick.GetAxis();
            var flattenedHead = Quaternion.LookRotation(_head.forward.FlattenNeck(_head.up, transform.up), transform.up);
            var movement = flattenedHead * new Vector3(movementAxis.x, 0f, movementAxis.y);

            XRManager.Api.RightController.TryGetPrimaryButton(out var button);

            var jump = button.GetPressed();

            input = new GenericInput(movement, jump);
            return true;
        }

        public override bool TryGetArm(Handedness handedness, out IArm arm)
        {
            if (!XRManager.HasApi())
            {
                arm = default;
                return false;
            }

            var root = SimpleTransform.Create(transform);

            switch (handedness)
            {
                default:
                    arm = default;
                    return false;
                case Handedness.LEFT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_leftWrist)), XRManager.Api.LeftController, XRManager.Api.LeftHand));
                    return true;
                case Handedness.RIGHT:
                    arm = new GenericArm(new GenericHand(root.InverseTransform(SimpleTransform.Create(_rightWrist)), XRManager.Api.RightController, XRManager.Api.RightHand));
                    return true;
            }
        }
    }
}
