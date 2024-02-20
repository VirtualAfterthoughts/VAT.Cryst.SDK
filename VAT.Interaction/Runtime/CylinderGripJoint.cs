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

            var grabPoint = interactor.GetGrabPoint(grip.GetPalmPosition());

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var initialRotation = rb.transform.rotation;
            rb.transform.rotation = _center.rotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.gameObject.AddComponent<ConfigurableJoint>();
            joint.axis = Quaternion.Inverse(initialRotation) * grabPoint.forward;
            joint.secondaryAxis = Quaternion.Inverse(initialRotation) * grabPoint.up;

            var host = grip.GetHostOrDefault();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbodyOrDefault();
            }

            joint.autoConfigureConnectedAnchor = false;

            _joint = joint;

            grabPoint = interactor.GetGrabPoint(grip.GetPalmPosition());
            var grabNormal = interactor.GetRigidbody().transform.up;

            joint.SetWorldAnchor((Vector3)grabPoint.position - grabNormal * _radius);
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
                _joint.yDrive = new JointDrive()
                {
                    positionSpring = 0f,
                    positionDamper = Mathf.LerpUnclamped(0f, 500000f, friction * friction),
                    maximumForce = float.PositiveInfinity
                };

                _joint.slerpDrive = new JointDrive()
                {
                    positionSpring = 0f,
                    positionDamper = Mathf.LerpUnclamped(0f, 500000f, friction * friction),
                    maximumForce = float.PositiveInfinity
                };
            }
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
            _joint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Locked);
            _joint.yMotion = ConfigurableJointMotion.Limited;
            _joint.angularYMotion = ConfigurableJointMotion.Free;

            _joint.linearLimit = new SoftJointLimit() { limit = _height * 0.5f };
            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 0f, positionDamper = 1000f, maximumForce = 500000f };
            _joint.slerpDrive = new JointDrive();

            _isFree = false;
        }
    }
}
