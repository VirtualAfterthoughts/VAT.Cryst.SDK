using System;
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
        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("The priority of the grip compared to other grips. A value of 0 means complete priority, a value of 1 is default, and higher values are less prioritized.")]
        private float _priority = 1f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _lowFriction = 0.5f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _highFriction = 1f;

        private Dictionary<IInteractor, GripJoint> _joints = new();

        public HandPose pose;

        private IHost _host = null;

        private bool _isInteractable = true;

        protected virtual GripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new GripJoint();
        }

        public void OnAttachConfirm(IInteractor interactor)
        {
            var gripJoint = OnCreateGripJoint(interactor);
            gripJoint.AttachJoint(interactor, this);
            gripJoint.FreeJoint();
            _joints[interactor] = gripJoint;
        }

        public void OnAttachComplete(IInteractor interactor)
        {
            _joints[interactor].LockJoint();
        }

        public void OnAttachUpdate(IInteractor interactor)
        {
            _joints[interactor].UpdateJoint();
        }

        public void OnDetachConfirm(IInteractor interactor)
        {
            _joints[interactor].DetachJoint();
            _joints.Remove(interactor);
        }

        public void DisableInteraction()
        {
            _isInteractable = false;
        }

        public void EnableInteraction()
        {
            _isInteractable = true;
        }

        public bool IsInteractable()
        {
            return _isInteractable;
        }

        public (bool valid, float priority) ValidateInteractable(IInteractor interactor)
        {
            var target = GetTargetInWorld(interactor);
            var grabPoint = interactor.GetGrabPoint();

            float distance = ((Vector3)(target.position - grabPoint.position)).magnitude;

            float angle = Quaternion.Angle(target.rotation, grabPoint.rotation) * Mathf.Deg2Rad;

            return (true, (distance + angle) * _priority);
        }

        public void OnHoverBegin(IInteractor interactor)
        {

        }

        public void OnHoverEnd(IInteractor interactor)
        {

        }

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

        public SimpleTransform GetInteractorInHost(IInteractor interactor)
        {
            return SimpleTransform.Create(GetHostGameObject().transform).InverseTransform(GetTargetInWorld(interactor));
        }

        public SimpleTransform GetHostInInteractor(IInteractor interactor)
        {
            return GetTargetInWorld(interactor).InverseTransform(SimpleTransform.Create(GetHostGameObject().transform));
        }

        public abstract SimpleTransform GetTargetInWorld(IInteractor interactor);

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
