using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class CylinderGripJoint : IGripJoint
    {
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

        public ConfigurableJoint Joint => _joint;

        private bool _isFree = false;

        public void AttachJoints(IInteractor interactor, Grip grip)
        {
            var rb = interactor.GetRigidbody();

            var grabberPoint = interactor.GetGrabberPoint();
            var grabPoint = grabberPoint.GetParentTransform().Transform(grip.GetTargetInInteractor(grabberPoint));

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

            var host = grip.GetHostOrDefault();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbodyOrDefault();
            }

            joint.autoConfigureConnectedAnchor = false;

            _joint = joint;

            grabPoint = grabberPoint.GetParentTransform().Transform(grip.GetPivotInInteractor(grabberPoint));

            joint.SetWorldAnchor((Vector3)grabPoint.position);
            joint.SetWorldConnectedAnchor(_center.position);

            _joint.swapBodies = true;

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
            }
            else
            {
                float force = Mathf.LerpUnclamped(0f, 5000f, Mathf.Pow(friction, 4f));

                _joint.xDrive = new JointDrive()
                {
                    positionSpring = 0f,
                    positionDamper = force * 10f,
                    maximumForce = force
                };

                _joint.angularXDrive = new JointDrive()
                {
                    positionSpring = 0f,
                    positionDamper = force * 0.1f,
                    maximumForce = force
                };
            }
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
            _joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Locked);
            _joint.angularXMotion = ConfigurableJointMotion.Free;

            _joint.linearLimit = new SoftJointLimit() { limit = _height * 0.5f };

            _joint.xDrive = new JointDrive() { positionSpring = 0f, positionDamper = 1000f, maximumForce = 500000f };

            _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };

            _isFree = false;
        }
    }
}
