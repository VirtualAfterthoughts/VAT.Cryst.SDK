using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public partial class Interactable : MonoBehaviour, IInteractable
    {
        public event Action<IGrabber> OnAttachedEvent;
        public event Action<IGrabber> OnDetachedEvent;

        public event Action<IGrabber, IGrippable> OnForceDetachEvent;

        private readonly List<IGrabber> _attachedGrabbers = new();

        public IReadOnlyList<IGrabber> AttachedGrabbers => _attachedGrabbers;

        private readonly Dictionary<IGrabber, ConfigurableJoint> _attachedJoints = new();

        public IReadOnlyDictionary<IGrabber, ConfigurableJoint> AttachedJoints => _attachedJoints;

        public virtual bool CanAttach(IGrabber grabber)
        {
            return false;
        }

        public SimpleTransform GetGrabberInHost(IGrabber grabber)
        {
            return SimpleTransform.Create(GetHostGameObject().transform).InverseTransform(GetTargetInWorld(grabber));
        }

        public SimpleTransform GetHostInGrabber(IGrabber grabber)
        {
            return GetTargetInWorld(grabber).InverseTransform(GetHostGameObject().transform);
        }

        public virtual SimpleTransform GetTargetInWorld(IGrabber grabber)
        {
            return SimpleTransform.Create(transform);
        }

        public void DetachGrabber(IGrabber grabber)
        {
            OnForceDetachEvent?.Invoke(grabber, this);
        }

        public void DetachGrabbers()
        {
            for (var i = _attachedGrabbers.Count - 1; i >= 0; i--)
            {
                DetachGrabber(_attachedGrabbers[i]);
            }
        }

        public void OnAttached(IGrabber grabber)
        {
            _attachedGrabbers.Add(grabber);

            OnAttachedEvent?.Invoke(grabber);
        }

        public void OnDetached(IGrabber grabber)
        {
            _attachedGrabbers.Remove(grabber);

            OnDetachedEvent?.Invoke(grabber);
        }

        public void UpdateJoint(IGrabber grabber)
        {
            OnUpdateJoint(grabber, _attachedJoints[grabber]);
        }

        protected virtual void OnUpdateJoint(IGrabber grabber, ConfigurableJoint joint)
        {

        }

        public void CreateJoint(IGrabber grabber)
        {
            DestroyJoint(grabber);

            _attachedJoints[grabber] = OnCreateJoint(grabber);
        }

        protected virtual ConfigurableJoint OnCreateJoint(IGrabber grabber)
        {
            var gameObject = grabber.GetGrabberGameObject();
            var transform = gameObject.transform;

            SimpleTransform initialTransform = transform;

            ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;

            var grabPoint = grabber.GetGrabPoint();
            joint.anchor = transform.InverseTransformPoint(grabPoint.position);

            var target = GetGrabberInHost(grabber);
            var hostTransform = GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.rotation);
            transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(transform.rotation));

            // Now we setup the connected body and connected anchor
            // This will refresh the joint space
            if (HasRigidbody)
            {
                joint.connectedBody = GetHostRigidbody();
                joint.connectedAnchor = target.position;
            }
            else
            {
                joint.connectedBody = null;
                joint.connectedAnchor = hostTransform.TransformPoint(target.position);
            }

            // Reset transform back to initial state
            transform.SetPositionAndRotation(initialTransform.position, initialTransform.rotation);

            // Setup joint parameters
            joint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Limited);
            joint.linearLimitSpring = new SoftJointLimitSpring() { spring = 5e+06f, damper = 1e+06f };

            return joint;
        }

        public void DestroyJoint(IGrabber grabber)
        {
            if (_attachedJoints.TryGetValue(grabber, out var joint))
            {
                Destroy(joint);
                _attachedJoints.Remove(grabber);
            }
        }
    }
}
