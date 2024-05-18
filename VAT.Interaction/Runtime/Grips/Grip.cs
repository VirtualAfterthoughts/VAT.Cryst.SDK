using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;
using VAT.Input.Data;
using VAT.Shared.Data;

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
        [Tooltip("The modes of interaction enabled for this grip. Near means it can be interacted with in close proximity, Far means it can be interacted with from far away such as a force grab.")]
        private HoverFlags _hoverFlags = HoverFlags.NEAR;

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

        public event InteractorDelegate OnAttached, OnDetached;
        public event InteractorDelegate OnHoverBegin, OnHoverEnd;

        public HandPose DefaultClosedPose
        {
            get
            {
                return _defaultClosedPose;
            }
        }

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
        }

        public void OnAttachComplete(IInteractor interactor)
        {
            _gripJoints[interactor].LockJoints();

            _interactorStates[interactor].isAttaching = false;

            OnAttached?.Invoke(interactor);
        }

        public void OnAttachUpdate(IInteractor interactor)
        {
            float force = 0f;

            var controller = interactor.GetInputHand().GetInputController();

            if (controller != null)
            {
                force = controller.GetGripForce();
            }

            _gripJoints[interactor].UpdateJoints(Mathf.Lerp(_lowFriction, _highFriction, force));
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

            if (!wasAttaching)
            {
                OnDetached?.Invoke(interactor);
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

            return (false, HandPoseHelper.DefaultOpenPose);
        }

        public virtual (bool valid, HandPoseData data) GetClosedPose(IInteractor interactor)
        {
            if (_defaultClosedPose != null)
            {
                return (true, _defaultClosedPose.data);
            }

            return (false, HandPoseHelper.DefaultClosedPose);
        }

        public (bool valid, float priority) ValidateInteraction(IInteractor interactor)
        {
            if (!IsInteractable() || (_attachedInteractors.Count > 0 && _swapMode == GripSwapMode.SINGLE))
                return (false, 0f);

            var palm = interactor.GetPalm();
            var palmHost = palm.GetHostTransform();

            var target = GrabTargetHelper.GetTargetInWorld(this, interactor);
            var grabCenter = palmHost.Transform(palm.GetProximityCenterInHost());

            float distance = ((Vector3)(target.position - grabCenter.position)).magnitude;

            float angle = Quaternion.Angle(target.rotation, grabCenter.rotation) * Mathf.Deg2Rad * distance;

            return (true, (distance + angle) * _priority);
        }

        public void BeginHover(IInteractor interactor)
        {
            OnHoverBegin?.Invoke(interactor);
        }

        public void EndHover(IInteractor interactor)
        {
            OnHoverEnd?.Invoke(interactor);
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

        public virtual SimpleTransform GetPivotInWorld(PalmPoint point, HandPoseData pose)
        {
            return GrabTargetHelper.CalculateTargetInWorld(this, point, pose);
        }

        public HandPoseData GetDefaultPose()
        {
            return _defaultClosedPose ? _defaultClosedPose.data : HandPoseHelper.DefaultClosedPose;
        }

        public virtual SimpleTransform GetPivotInInteractor(PalmPoint point, HandPoseData pose)
        {
            return GrabTargetHelper.GetTargetInInteractor(point, pose);
        }

        public InteractableHost GetHost()
        {
            return _host;
        }

        public HoverFlags GetHoverFlags()
        {
            return _hoverFlags;
        }

        private readonly Dictionary<IInteractor, SimpleTransform> _targetsInHost = new();

        public SimpleTransform GetTargetInHost(IInteractor interactor)
        {
            if (_targetsInHost.TryGetValue(interactor, out SimpleTransform target))
            {
                return target;
            }

            return CalculateTargetInHost(interactor.GetPalm(), GetClosedPose(interactor).data);
        }

        public void SetTargetInHost(IInteractor interactor, SimpleTransform target)
        {
            _targetsInHost[interactor] = target;
        }

        public abstract SimpleTransform CalculateTargetInHost(PalmPoint point, HandPoseData pose);

        public virtual SimpleTransform CalculateDefaultTargetInHost(PalmPoint point, HandPoseData pose)
        {
            return CalculateTargetInHost(point, pose);
        }
    }
}
