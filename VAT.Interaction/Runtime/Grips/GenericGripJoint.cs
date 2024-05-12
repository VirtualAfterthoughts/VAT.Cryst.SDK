using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class GenericGripJoint : IGripJoint
    {
        private Grip _grip;
        private IInteractor _interactor;
        private ConfigurableJoint _joint = null;

        private ConfigurableJointSpace _jointSpace = null;

        private bool _isFree = false;

        public void AttachJoints(IInteractor interactor, Grip grip)
        {
            _grip = grip;

            _interactor = interactor;

            var rb = interactor.GetRigidbody();

            var grabberPoint = interactor.GetPalm();
            var grabPoint = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, grip.GetDefaultPose()));

            var target = grip.GetTargetInHost(grabberPoint);
            var hostTransform = grip.GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.rotation);
            var initialRotation = rb.transform.rotation;
            rb.transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.gameObject.AddComponent<ConfigurableJoint>();

            var host = grip.GetHost();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbody();
            }

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = grip.GetPivotInInteractor(interactor.GetPalm(), grip.GetClosedPose(interactor).data).position;
            joint.SetWorldConnectedAnchor(grip.GetPivotInWorld(interactor.GetPalm(), grip.GetClosedPose(interactor).data).position);

            _joint = joint;

            _jointSpace = new ConfigurableJointSpace(joint);

            rb.transform.rotation = initialRotation;
        }

        public void DetachJoints()
        {
            GameObject.Destroy(_joint);
            _joint = null;
        }

        public void UpdateJoints(float friction)
        {
            if (_isFree)
            {
                _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = Mathf.Lerp(_joint.xDrive.positionSpring, 5000f, Time.deltaTime * 0.5f), positionDamper = 0f, maximumForce = float.PositiveInfinity };
                _joint.slerpDrive = new JointDrive() { positionSpring = 0f, positionDamper = Mathf.Lerp(_joint.slerpDrive.positionDamper, 900f, Time.deltaTime * 0.5f), maximumForce = float.PositiveInfinity };
            }
            else
            {
                float force = Mathf.LerpUnclamped(0f, 9000f, friction);

                _joint.slerpDrive = new JointDrive()
                {
                    positionSpring = force,
                    positionDamper = force * 0.1f,
                    maximumForce = force
                };
            }

            var grabberPoint = _interactor.GetPalm();
            var target = _grip.GetTargetInWorld(grabberPoint);
            var selfTarget = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, _grip.GetDefaultPose()));

            target = target.Transform(selfTarget.InverseTransform(grabberPoint.GetHostTransform()));

            _jointSpace.SetTargetRotationWorld(target.rotation);
        }

        public void FreeJoints()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);
            _joint.rotationDriveMode = RotationDriveMode.Slerp;

            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 5f, positionDamper = 0f, maximumForce = float.MaxValue };

            _joint.linearLimit = new SoftJointLimit() { limit = Vector3.Distance(_joint.GetWorldAnchor(), _joint.GetWorldConnectedAnchor()) };

            _isFree = true;
        }

        public void LockJoints()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Free);

            _joint.linearLimit = new SoftJointLimit() { limit = 0.05f };
            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };
            _joint.slerpDrive = new JointDrive() { positionSpring = 9000f, positionDamper = 500f, maximumForce = 1200f };

            _isFree = false;
        }
    }
}
