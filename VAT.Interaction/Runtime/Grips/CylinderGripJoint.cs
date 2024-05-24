using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class CylinderGripJoint : IGripJoint
    {
        private IInteractor _interactor;
        private Grip _grip;

        private Transform _center;
        private float _radius;
        private float _height;

        public CylinderGripJoint(Transform center, float radius, float height)
        {
            _center = center;
            _radius = radius;
            _height = height;
        }

        private ConfigurableJoint _joint = null;

        private ConfigurableJointSpace _jointSpace = null;

        public ConfigurableJoint Joint => _joint;

        private bool _isFree = false;

        public void AttachJoints(IInteractor interactor, Grip grip)
        {
            _grip = grip;
            _interactor = interactor;

            var rb = interactor.GetRigidbody();

            var grabberPoint = interactor.GetPalm();
            var grabPoint = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, grip.GetDefaultPose()));

            float dot = Vector3.Dot(grabPoint.up, _center.up);

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var initialRotation = rb.transform.rotation;
            var centerRotation = _center.rotation;

            if (dot < 0f)
                centerRotation = Quaternion.AngleAxis(180f, _center.right) * centerRotation;

            rb.transform.rotation = centerRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.gameObject.AddComponent<ConfigurableJoint>();
            joint.axis = Quaternion.Inverse(initialRotation) * grabPoint.up;
            joint.secondaryAxis = Quaternion.Inverse(initialRotation) * grabPoint.forward;

            var host = grip.GetHost();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbody();
            }

            joint.autoConfigureConnectedAnchor = false;

            _joint = joint;

            grabPoint = grabberPoint.GetHostTransform().Transform(grip.GetPivotInInteractor(grabberPoint, grip.GetClosedPose(interactor).data));

            joint.SetWorldAnchor((Vector3)grabPoint.position);
            joint.SetWorldConnectedAnchor(_center.position);

            _joint.swapBodies = true;

            _jointSpace = new ConfigurableJointSpace(_joint);

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
                var target = GetGripTarget();

                _joint.targetPosition = GetTargetPosition(target);
                _joint.targetRotation = GetTargetRotation(target);

                _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = Mathf.Lerp(_joint.xDrive.positionSpring, 5000f, Time.deltaTime * 0.5f), positionDamper = 0f, maximumForce = float.PositiveInfinity };
            }
            else
            {
                float force = Mathf.LerpUnclamped(0f, 10000f, Mathf.Pow(friction, 4f));

                _joint.xDrive = new JointDrive()
                {
                    positionSpring = force * 30f,
                    positionDamper = force * 0.1f,
                    maximumForce = force * 10f
                };

                _joint.angularXDrive = new JointDrive()
                {
                    positionSpring = force * 1f,
                    positionDamper = force * 0.1f,
                    maximumForce = force * 0.1f,
                };

                _joint.angularYZDrive = new JointDrive()
                {
                    positionSpring = force,
                    positionDamper = force * 0.1f,
                    maximumForce = force,
                };

                UpdateTargets(friction);
            }
        }

        private SimpleTransform GetGripTarget()
        {
            var grabberPoint = _interactor.GetPalm();
            var target = GrabTargetHelper.GetTargetInWorld(_grip, _interactor);
            var selfTarget = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, _grip.GetDefaultPose()));

            target = target.Transform(selfTarget.InverseTransform(grabberPoint.GetHostTransform()));
            return target;
        }

        private Vector3 GetTargetPosition(SimpleTransform target)
        {
            var targetPos = _jointSpace.GetTargetPositionWorld(target.position);

            targetPos.y = 0f;
            targetPos.z = 0f;

            return targetPos;
        }

        private Quaternion GetTargetRotation(SimpleTransform target)
        {
            Quaternion targetRot = _jointSpace.GetTargetRotationWorld(target.rotation);

            return targetRot;
        }

        private void UpdateTargets(float friction)
        {
            var target = GetGripTarget();

            var targetRot = GetTargetRotation(target);

            var lastTargetRot = _joint.targetRotation;
            _joint.targetRotation = Quaternion.RotateTowards(targetRot, _joint.targetRotation, 10f * friction);

            var targetAngularVelocity = PhysicsExtensions.GetAngularVelocity(lastTargetRot, _joint.targetRotation);
            _joint.targetAngularVelocity = targetAngularVelocity;

            var targetPos = GetTargetPosition(target);

            _joint.targetPosition = Vector3.MoveTowards(targetPos, _joint.targetPosition, 0.05f * friction);
        }

        public void FreeJoints()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);
            _joint.rotationDriveMode = RotationDriveMode.XYAndZ;

            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 5f, positionDamper = 0f, maximumForce = float.MaxValue };

            _joint.linearLimit = new SoftJointLimit() { limit = Vector3.Distance(_joint.GetWorldAnchor(), _joint.GetWorldConnectedAnchor()) };

            _isFree = true;
        }

        public void LockJoints()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
            _joint.angularXMotion = ConfigurableJointMotion.Free;

            _joint.angularYLimit = _joint.angularZLimit = new SoftJointLimit() { limit = 100f };
            _joint.angularYZLimitSpring = new SoftJointLimitSpring() { spring = 500000f, damper = 10000f };

            _joint.linearLimit = new SoftJointLimit() { limit = _height * 0.5f };

            _joint.xDrive = new JointDrive() { positionSpring = 0f, positionDamper = 1000f, maximumForce = 500000f };

            _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };

            var target = GetGripTarget();

            _joint.targetPosition = GetTargetPosition(target);
            _joint.targetRotation = GetTargetRotation(target);

            _isFree = false;
        }
    }
}
