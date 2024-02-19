using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Input;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Interaction
{
    public abstract class Grip : MonoBehaviour, IInteractable
    {
        public enum GripSwapMode
        {
            NONE = 0,
            SWAP = 1,
            SINGLE = 2,
        }

        [Header("Grabbing")]
        [SerializeField]
        private GripSwapMode _swapMode = GripSwapMode.NONE;

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("The priority of the grip compared to other grips. A value of 0 means complete priority, a value of 1 is default, and higher values are less prioritized.")]
        private float _priority = 1f;

        [Header("Visuals")]
        [SerializeField]
        private HandPose _defaultClosedPose;

        [SerializeField]
        private HandPose _defaultOpenPose;

        [Header("Physics")]
        [SerializeField]
        [Range(0f, 2f)]
        private float _lowFriction = 0.5f;

        [SerializeField]
        [Range(0f, 2f)]
        private float _highFriction = 1f;

        private List<IInteractor> _attachedInteractors = new();
        private Dictionary<IInteractor, IGripJoint> _gripJoints = new();

        private InteractableHost _host = null;

        private bool _isInteractable = true;

        private void OnEnable()
        {
            FindHost();
        }

        private void OnDisable()
        {
            UnregisterHost();

            for (var i = _attachedInteractors.Count - 1; i >= 0; i--)
            {
                _attachedInteractors[i].DetachGrip(this);
            }
        }

        public void UnregisterHost()
        {
            if (_host != null)
            {
                _host.UnregisterInteractable(this);
                _host = null;
            }
        }

        public void FindHost()
        {
            UnregisterHost();

            _host = GetComponentInParent<InteractableHost>();

            if (_host != null)
            {
                _host.RegisterInteractable(this);
            }
        }

        protected virtual IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new GenericGripJoint();
        }

        public Vector2 GetPalmPosition()
        {
            if (_defaultClosedPose != null)
            {
                return _defaultClosedPose.centerOfPressure;
            }
            else
            {
                return Vector2.up;
            }
        }

        public void OnAttachConfirm(IInteractor interactor)
        {
            if (_swapMode == GripSwapMode.SWAP)
            {
                foreach (var otherInteractor in _attachedInteractors.ToArray())
                {
                    otherInteractor.DetachGrip(this);
                }
            }

            var gripJoint = OnCreateGripJoint(interactor);
            gripJoint.AttachJoints(interactor, this);
            gripJoint.FreeJoints();
            _gripJoints[interactor] = gripJoint;

            _attachedInteractors.Add(interactor);
        }

        public void OnAttachComplete(IInteractor interactor)
        {
            _gripJoints[interactor].LockJoints();
        }

        public void OnAttachUpdate(IInteractor interactor)
        {
            _gripJoints[interactor].UpdateJoints(_highFriction);
        }

        public void OnDetachConfirm(IInteractor interactor)
        {
            _gripJoints[interactor].DetachJoints();
            _gripJoints.Remove(interactor);

            _attachedInteractors.Remove(interactor);

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

        public virtual (bool valid, HandPoseData data) GetOpenPose(IInteractor interactor)
        {
            if (_defaultOpenPose != null)
            {
                return (true, _defaultOpenPose.data);
            }

            return (false, default);
        }

        public virtual (bool valid, HandPoseData data) GetClosedPose(IInteractor interactor)
        {
            if (_defaultClosedPose != null)
            {
                return (true, _defaultClosedPose.data);
            }

            return (false, default);
        }

        public (bool valid, float priority) ValidateInteractable(IInteractor interactor)
        {
            if (_attachedInteractors.Count > 0 && _swapMode == GripSwapMode.SINGLE)
                return (false, 0f);

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

        public InteractableHost GetHostOrDefault()
        {
            return _host;
        }
    }
}
