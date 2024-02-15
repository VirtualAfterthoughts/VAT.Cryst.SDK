using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public abstract class Grip : MonoBehaviour, IInteractable
    {
        private Dictionary<IInteractor, ConfigurableJoint> _joints = new();

        public HandPose pose;

        private IHost _host = null;

        public abstract void DisableInteraction();
        public abstract void EnableInteraction();
        public abstract float GetPriority(IInteractor interactor);
        public abstract bool IsInteractable();
        public abstract void OnHoverBegin(IInteractor interactor);
        public abstract void OnHoverEnd(IInteractor interactor);

        public GameObject GetHostGameObject()
        {
            if (_host != null)
            {
                return _host.GetGameObject();
            }
            else
            {
                return gameObject;
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public SimpleTransform GetInteractorInHost(IInteractor interactor)
        {
            return SimpleTransform.Create(GetHostGameObject().transform).InverseTransform(GetTargetInWorld(interactor));
        }

        public SimpleTransform GetHostInInteractor(IInteractor interactor)
        {
            return GetTargetInWorld(interactor).InverseTransform(GetHostGameObject().transform);
        }

        public abstract SimpleTransform GetTargetInWorld(IInteractor interactor);

        public void OnAttachBegin(IInteractor interactor)
        {
            AttachJoint(interactor);
        }

        public void OnAttachEnd(IInteractor interactor)
        {
            LockJoint(interactor);
        }

        public void OnDetached(IInteractor interactor)
        {
            DetachJoint(interactor);
        }

        public void UpdateJoint(IInteractor interactor)
        {
            var joint = _joints[interactor];
            joint.xDrive = joint.yDrive = joint.zDrive = new JointDrive() { positionSpring = Mathf.Lerp(joint.xDrive.positionSpring, 5000f, Time.deltaTime * 0.5f), positionDamper = 0f, maximumForce = float.PositiveInfinity };
        }

        private void LockJoint(IInteractor interactor)
        {
            var joint = _joints[interactor];
            joint.SetJointMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Free);

            joint.linearLimit = new SoftJointLimit() { limit = 0.05f };
            joint.xDrive = joint.yDrive = joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };
            joint.slerpDrive = new JointDrive() { positionSpring = 9000f, positionDamper = 1000f, maximumForce = 1200f };
        }

        private void AttachJoint(IInteractor interactor)
        {
            var rb = interactor.GetRigidbody();
            rb.CreateItem();

            var grabPoint = interactor.GetGrabPoint();
            var target = GetInteractorInHost(interactor);
            var hostTransform = GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.rotation);
            var initialRotation = rb.transform.rotation;
            rb.transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.GameObject.AddComponent<ConfigurableJoint>();
            joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            joint.xDrive = joint.yDrive = joint.zDrive = new JointDrive() { positionSpring = 5f, positionDamper = 0f, maximumForce = float.MaxValue };

            var host = GetHostOrDefault();

            if (host != null)
            {
                joint.connectedBody = host.GetRigidbodyOrDefault();
            }

            joint.autoConfigureConnectedAnchor = false;
            joint.SetWorldAnchor(interactor.GetGrabPoint().position);
            joint.SetWorldConnectedAnchor(GetTargetInWorld(interactor).position);

            joint.linearLimit = new SoftJointLimit() { limit = Vector3.Distance(joint.GetWorldAnchor(), joint.GetWorldConnectedAnchor()) };

            _joints[interactor] = joint;

            rb.transform.rotation = initialRotation;
        }

        private void DetachJoint(IInteractor interactor)
        {
            var joint = _joints[interactor];
            _joints.Remove(interactor);
            GameObject.Destroy(joint);
        }

        public void RegisterHost(IHost host)
        {
            _host = host;
        }

        public void UnregisterHost()
        {
            _host = null;
        }

        public IHost GetHostOrDefault()
        {
            return _host;
        }
    }
}
