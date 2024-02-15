using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VAT.Avatars;
using VAT.Avatars.Integumentary;
using VAT.Entities.PhysX;
using VAT.Input;
using VAT.Interaction;
using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public class CrystInteractor : MonoBehaviour, IInteractor
    {
        public List<InteractableHost> hosts = new List<InteractableHost>();
        public CrystRigidbody rb;
        public Handedness handedness;
        public IInputController controller;
        public IInputHand hand;
        public AvatarArm arm;
        public HandPoseData openPose;
        public HandPoseData closedPose;

        public float grabRadius = 0.2f;

        private Grip _attachedGrip;

        private bool _isSnatching = false;

        private IInteractable _hoveringInteractable;
        public IInteractable HoveringInteractable
        {
            get
            {
                return _hoveringInteractable;
            }
            set
            {
                // Check if we are able to hover currently
                if (IsInteractionLocked())
                {
                    Debug.LogWarning("Attempted to hover an Interactable, but the Interactor cannot hover!", this);
                    value = null;
                }

                // Make sure this is a different interactable
                if (_hoveringInteractable != value)
                {
                    // Begin hover
                    if (_hoveringInteractable == null)
                    {
                        value.OnHoverBegin(this);
                    }
                    // End hover
                    else
                    {
                        _hoveringInteractable.OnHoverEnd(this);

                        if (value != null)
                            value.OnHoverBegin(this);
                    }

                    _hoveringInteractable = value;
                }
            }
        }

        private bool _isInteractionLocked = false;

        private void Awake()
        {
            rb = GetComponent<CrystRigidbody>();
        }

        private void Start()
        {
            arm.DataArm.OnProcessTarget += OnProcessTarget;

            arm.DataArm.Hand.SetOpenPose(openPose);
            arm.DataArm.Hand.SetClosedPose(closedPose);

            _lastTarget = transform;
        }

        private float _pinAmount = 0f;
        private SimpleTransform _lastTarget = SimpleTransform.Default;

        private SimpleTransform OnProcessTarget(in SimpleTransform target)
        {
            Vector3 velocity = PhysicsExtensions.GetLinearVelocity(_lastTarget.position, target.position);
            _pinAmount = Mathf.Lerp(_pinAmount, 0f, Mathf.Clamp01(velocity.magnitude * 0.5f - 0.05f));

            _lastTarget = target;

            var values = GetValues();

            var goal = values.Item1;
            goal.rotation = target.rotation;

            return SimpleTransform.Lerp(target, values.Item1, values.Item2);
        }

        public bool IsInteractionLocked()
        {
            return _isInteractionLocked;
        }

        public void LockInteraction()
        {
            _isInteractionLocked = true;
        }

        public void UnlockInteraction()
        {
            _isInteractionLocked = false;
        }

        private SimpleTransform lastTar = SimpleTransform.Default;

        private float _lerp;

        public (SimpleTransform, float) GetValues()
        {
            if (_isSnatching)
            {
                var target = _attachedGrip.GetTargetInWorld(this);
                target.lossyScale = 1f;
                var grabPoint = GetGrabPoint();
                grabPoint.rotation = target.rotation;

                var self = target.Transform(grabPoint.InverseTransform(transform));
                lastTar = self;
                _lerp = Mathf.Lerp(_lerp, 1f, Time.deltaTime * 12f);
                return (self, _lerp);
            }
            else
            {
                _lerp = Mathf.Lerp(_lerp, _pinAmount, Time.deltaTime * 12f);
                return (lastTar, _lerp);
            }
        }

        public void LateUpdate()
        {
            arm.DataArm.Hand.SetBlendPose(hand.GetHandPose());

            controller.TryGetGrip(out var grip);

            OnUpdateHover();

            if (grip.GetAxis() > 0.6f && !_attachedGrip && HoveringInteractable is Grip grp)
            {
                AttachGrip(grp);
            }
            else if (grip.GetAxis() < 0.4f && _attachedGrip)
            {
                DetachGrips();
            }

            if (_isSnatching)
            {
                float distance = math.length(_attachedGrip.GetTargetInWorld(this).position - GetGrabPoint().position);

                if (_attachedGrip.pose != null)
                {
                    var targetPose = _attachedGrip.pose.data;
                    var newPose = HandPoseCreator.Lerp(openPose, targetPose, Mathf.Pow(distance / grabRadius - 1f, 2f));

                    arm.DataArm.Hand.SetClosedPose(newPose);
                }

                _attachedGrip.UpdateJoint(this);

                if (distance <= 0.05f)
                {
                    _isSnatching = false;
                    _attachedGrip.OnAttachEnd(this);

                    if (_attachedGrip.pose != null)
                    {
                        arm.DataArm.Hand.SetClosedPose(_attachedGrip.pose.data);
                    }

                    ResetPin();
                }
            }
        }

        public void AttachGrip(Grip grip)
        {
            grip.OnAttachBegin(this);
            _attachedGrip = grip;
            _isSnatching = true;

            ToggleCollsion(grip, true);

            HoveringInteractable = null;
        }

        public void DetachGrips()
        {
            arm.DataArm.Hand.SetOpenPose(openPose);
            arm.DataArm.Hand.SetClosedPose(closedPose);

            if (_attachedGrip)
            {
                DetachGrip(_attachedGrip);
                _attachedGrip = null;
                _isSnatching = false;
            }

            ResetPin();
        }

        private void ResetPin()
        {
            _pinAmount = 0.8f;
            lastTar = transform;
        }

        public void DetachGrip(Grip grip)
        {
            grip.OnDetached(this);

            ToggleCollsion(grip, false);

            _attachedGrip = null;
        }

        public void ToggleCollsion(Grip grip, bool ignore)
        {
            var gripHost = grip.GetHostOrDefault();

            if (gripHost != null)
            {
                foreach (var host in hosts)
                {
                    foreach (var col1 in host.Colliders)
                    {
                        foreach (var col2 in gripHost.GetColliders())
                        {
                            Physics.IgnoreCollision(col1, col2, ignore);
                        }
                    }
                }
            }
        }

        protected void OnUpdateHover()
        {
            if (_attachedGrip)
                return;

            var grabPoint = GetGrabPoint();
            var colliders = Physics.OverlapSphere(grabPoint.position, grabRadius, ~0, QueryTriggerInteraction.Collide);
            IInteractable interactable = null;

            foreach (var collider in colliders)
            {
                var go = collider.attachedRigidbody ? collider.attachedRigidbody.gameObject : collider.gameObject;

                if (go.TryGetComponent<IInteractable>(out var component)) {
                    interactable = component;
                }
            }

            HoveringInteractable = interactable;
        }

        public CrystRigidbody GetRigidbody()
        {
            return rb;
        }

        public SimpleTransform GetGrabPoint()
        {
            return arm.PhysArm.Hand.GetPointOnPalm(Vector2.up);
        }
    }
}
