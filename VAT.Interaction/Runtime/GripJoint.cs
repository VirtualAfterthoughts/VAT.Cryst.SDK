using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public class GripJoint
    {
        public IInteractor Interactor => _interactor;

        public Grip Grip => _grip;

        private IInteractor _interactor = null;
        private Grip _grip = null;

        private ConfigurableJoint _joint = null;

        public ConfigurableJoint Joint => _joint;

        private bool _isFree = false;

        public void AttachJoint(IInteractor interactor, Grip grip)
        {
            _interactor = interactor;
            _grip = grip;

            var rb = interactor.GetRigidbody();
            rb.CreateItem();

            var grabPoint = interactor.GetGrabPoint();
            var target = grip.GetInteractorInHost(_interactor);
            var hostTransform = grip.GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.rotation);
            var initialRotation = rb.transform.rotation;
            rb.transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.GameObject.AddComponent<ConfigurableJoint>();

            var host = grip.GetHostOrDefault();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbodyOrDefault();
            }

            joint.autoConfigureConnectedAnchor = false;
            joint.SetWorldAnchor(interactor.GetGrabPoint().position);
            joint.SetWorldConnectedAnchor(grip.GetTargetInWorld(interactor).position);

            OnAttachJoint(joint);

            _joint = joint;

            rb.transform.rotation = initialRotation;
        }

        protected virtual void OnAttachJoint(ConfigurableJoint joint)
        {

        }
        
        public void DetachJoint()
        {
            GameObject.Destroy(_joint);
            _joint = null;
            _grip = null;
            _interactor = null;
        }

        public void UpdateJoint()
        {
            if (_isFree)
            {
                OnUpdateFreeJoint();
            }
            else
            {
                OnUpdateLockedJoint();
            }
        }

        protected virtual void OnUpdateFreeJoint()
        {
            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = Mathf.Lerp(_joint.xDrive.positionSpring, 5000f, Time.deltaTime * 0.5f), positionDamper = 0f, maximumForce = float.PositiveInfinity };
        }

        protected virtual void OnUpdateLockedJoint()
        {

        }

        public void FreeJoint()
        {
            OnFreeJoint();

            _isFree = true;
        }

        protected virtual void OnFreeJoint()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);
            _joint.rotationDriveMode = RotationDriveMode.Slerp;

            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 5f, positionDamper = 0f, maximumForce = float.MaxValue };

            _joint.linearLimit = new SoftJointLimit() { limit = Vector3.Distance(_joint.GetWorldAnchor(), _joint.GetWorldConnectedAnchor()) };

        }

        public void LockJoint()
        {
            OnLockJoint();

            _isFree = false;
        }

        protected virtual void OnLockJoint()
        {
            _joint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Free);

            _joint.linearLimit = new SoftJointLimit() { limit = 0.05f };
            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };
            _joint.slerpDrive = new JointDrive() { positionSpring = 9000f, positionDamper = 500f, maximumForce = 1200f };
        }
    }
}
