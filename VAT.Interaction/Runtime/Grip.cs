using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public abstract class Grip : MonoBehaviour, IInteractable
    {
        private Dictionary<IInteractor, ConfigurableJoint> _joints = new Dictionary<IInteractor, ConfigurableJoint>();

        public abstract void DisableInteraction();
        public abstract void EnableInteraction();
        public abstract float GetPriority(IInteractor interactor);
        public abstract bool IsInteractable();
        public abstract void OnHoverBegin(IInteractor interactor);
        public abstract void OnHoverEnd(IInteractor interactor);

        private GameObject GetHostGameObject()
        {
            return GetComponentInParent<InteractableHost>().gameObject;
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

        private void LockJoint(IInteractor interactor)
        {
            var joint = _joints[interactor];
            joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);

            joint.linearLimit = new SoftJointLimit() { limit = 0.05f };
            joint.xDrive = joint.yDrive = joint.zDrive = joint.slerpDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };
        }

        private void AttachJoint(IInteractor interactor)
        {
            var rb = interactor.GetRigidbody();
            rb.CreateItem();

            var host = GetComponentInParent<InteractableHost>();

            var grabPoint = interactor.GetGrabPoint();
            var target = GetInteractorInHost(interactor);
            var hostTransform = GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.rotation);
            rb.transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.GameObject.AddComponent<ConfigurableJoint>();
            joint.SetJointMotion(ConfigurableJointMotion.Free, ConfigurableJointMotion.Free);
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            joint.xDrive = joint.yDrive = joint.zDrive = new JointDrive() { positionSpring = 1200f, positionDamper = 100f, maximumForce = 1200f };
            joint.connectedBody = host.GetRigidbody();

            joint.autoConfigureConnectedAnchor = false;
            joint.SetWorldAnchor(interactor.GetGrabPoint().position);
            joint.SetWorldConnectedAnchor(GetTargetInWorld(interactor).position);

            _joints[interactor] = joint;
        }

        private void DetachJoint(IInteractor interactor)
        {
            var joint = _joints[interactor];
            _joints.Remove(interactor);
            GameObject.Destroy(joint);
        }
    }
}
