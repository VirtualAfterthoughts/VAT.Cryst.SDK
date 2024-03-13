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
    public abstract class Grip : MonoBehaviour, IGrippable
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
        private Dictionary<IInteractor, InteractorGripState> _interactorStates = new();
        private Dictionary<IInteractor, IGripJoint> _gripJoints = new();

        private InteractableHost _host = null;

        private bool _isInteractable = true;

        public HandPose DefaultClosedPose
        {
            get
            {
                return _defaultClosedPose;
            }
        }

        public event Action<IInteractor> AttachBeginEvent, AttachCancelEvent, AttachCompleteEvent, DetachCompleteEvent;

        public bool IsHeld => _attachedInteractors.Count > 0;

        public List<IInteractor> AttachedInteractors => _attachedInteractors;

        private void OnEnable()
        {
            FindHost();
        }

        private void OnDisable()
        {
            UnregisterHost();

            ForceDetachInteractors();
        }

        public IInteractor GetFirstInteractor()
        {
            if (_attachedInteractors.Count > 0)
                return _attachedInteractors[0];

            return null;
        }

        public void UnregisterHost()
        {
            if (_host != null)
            {
                foreach (var interactor in _attachedInteractors)
                {
                    _host.VirtualController.UnregisterPair(interactor);
                }

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

                foreach (var interactor in _attachedInteractors)
                {
                    _host.VirtualController.RegisterPair(interactor, this);
                }
            }
        }

        protected virtual IGripJoint OnCreateGripJoint(IInteractor interactor)
        {
            return new GenericGripJoint();
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

            _interactorStates.Add(interactor, new InteractorGripState()
            {
                interactor = interactor,
                isAttaching = true,
            });

            if (_host != null)
            {
                _host.VirtualController.RegisterPair(interactor, this);
            }

            AttachBeginEvent?.Invoke(interactor);
        }

        public void OnAttachComplete(IInteractor interactor)
        {
            _gripJoints[interactor].LockJoints();

            _interactorStates[interactor].isAttaching = false;

            AttachCompleteEvent?.Invoke(interactor);
        }

        public void OnAttachUpdate(IInteractor interactor)
        {
            _gripJoints[interactor].UpdateJoints(Mathf.Lerp(_lowFriction, _highFriction, interactor.GetGripForce()));
        }

        public void OnDetachConfirm(IInteractor interactor)
        {
            _gripJoints[interactor].DetachJoints();
            _gripJoints.Remove(interactor);

            var wasAttaching = _interactorStates[interactor].isAttaching;

            _attachedInteractors.Remove(interactor);
            _interactorStates.Remove(interactor);

            if (_host != null)
            {
                _host.VirtualController.UnregisterPair(interactor);
            }

            if (wasAttaching)
            {
                AttachCancelEvent?.Invoke(interactor);
            }
            else
            {
                DetachCompleteEvent?.Invoke(interactor);
            }

        }

        public void ForceDetachInteractors()
        {
            for (var i = _attachedInteractors.Count - 1; i >= 0; i--)
            {
                _attachedInteractors[i].DetachGrip(this);
            }
        }

        public void DisableInteraction()
        {
            _isInteractable = false;

            ForceDetachInteractors();
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
            if (!IsInteractable() || (_attachedInteractors.Count > 0 && _swapMode == GripSwapMode.SINGLE))
                return (false, 0f);

            var grabberPoint = interactor.GetGrabberPoint();

            var target = GetTargetInWorld(grabberPoint);
            var grabCenter = grabberPoint.GetGrabCenter();

            float distance = ((Vector3)(target.position - grabCenter.position)).magnitude;

            float angle = Quaternion.Angle(target.rotation, grabCenter.rotation) * Mathf.Deg2Rad * distance;

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
            return SimpleTransform.Create(GetHostGameObject().transform).InverseTransform(GetTargetInWorld(interactor.GetGrabberPoint()));
        }

        public SimpleTransform GetHostInInteractor(IInteractor interactor)
        {
            return GetTargetInWorld(interactor.GetGrabberPoint()).InverseTransform(SimpleTransform.Create(GetHostGameObject().transform));
        }

        public SimpleTransform GetTargetInWorld(IGrabPoint point)
        {
            return GetTargetInWorld(point, _defaultClosedPose.data);
        }

        public abstract SimpleTransform GetTargetInWorld(IGrabPoint point, HandPoseData pose);

        public virtual SimpleTransform GetPivotInWorld(IGrabPoint point, HandPoseData pose)
        {
            return GetTargetInWorld(point, pose);
        }

        public SimpleTransform GetTargetInInteractor(IGrabPoint point)
        {
            return GetTargetInInteractor(point, _defaultClosedPose.data);
        }
        private Quaternion GetAxisOffset(IGrabPoint point, HandPoseData pose)
        {
            var grabPoint = point.GetDefaultGrabPoint();
            var normal = -point.GetGrabNormal();

            float dot = Vector3.Dot(grabPoint.right, normal);

            var offset = pose.rotationOffset.normalized;

            if (dot > 0f)
            {
                offset.ToAngleAxis(out var angle, out var axis);
                axis.z = -axis.z;
                axis.y = -axis.y;

                offset = Quaternion.AngleAxis(angle, axis);
            }

            return offset;
        }

        public SimpleTransform GetTargetInInteractor(IGrabPoint point, HandPoseData pose)
        {
            var offset = GetAxisOffset(point, pose);

            var local = point.GetParentTransform().InverseTransform(point.GetGrabPoint(pose.centerOfPressure));

            local.rotation *= offset;
            return local;
        }

        public virtual SimpleTransform GetPivotInInteractor(IGrabPoint point, HandPoseData pose)
        {
            return GetTargetInInteractor(point, pose);
        }

        public InteractableHost GetHostOrDefault()
        {
            return _host;
        }

        public virtual SimpleTransform GetDefaultTargetInWorld(IGrabPoint point, HandPoseData pose)
        {
            return GetTargetInWorld(point, pose);
        }

        public virtual SimpleTransform GetDefaultTargetInInteractor(IGrabPoint point, HandPoseData pose)
        {
            return GetTargetInInteractor(point, pose);
        }
    }
}
