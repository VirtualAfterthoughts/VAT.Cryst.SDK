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

namespace VAT.Characters
{
    public class CrystInteractor : MonoBehaviour, IInteractor
    {
        public CrystRigidbody rb;
        public Handedness handedness;
        public IInputController controller;
        public IInputHand hand;
        public AvatarArm arm;
        public HandPoseData openPose;
        public HandPoseData closedPose;

        private Grip _attachedGrip;

        private bool _isSnatching = false;
        private float _snatchTime = 0f;

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
        }

        private SimpleTransform OnProcessTarget(in SimpleTransform target)
        {
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
                _lerp = Mathf.Lerp(_lerp, 0f, Time.deltaTime * 12f);
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
                _snatchTime += Time.deltaTime;

                float distance = math.length(_attachedGrip.GetTargetInWorld(this).position - GetGrabPoint().position);

                _attachedGrip.UpdateJoint(this);

                if (distance <= 0.05f)
                {
                    _isSnatching = false;
                    _attachedGrip.OnAttachEnd(this);

                    _snatchTime = 0.2f;
                }
            }
            else
            {
                _snatchTime -= Time.deltaTime;
            }
        }

        public void AttachGrip(Grip grip)
        {
            if (grip.pose != null)
            {
                arm.DataArm.Hand.SetClosedPose(grip.pose.data);
            }

            grip.OnAttachBegin(this);
            _attachedGrip = grip;
            _isSnatching = true;
            _snatchTime = 0f;

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
        }

        public void DetachGrip(Grip grip)
        {
            grip.OnDetached(this);

            _attachedGrip = null;
        }

        protected void OnUpdateHover()
        {
            if (_attachedGrip)
                return;

            var grabPoint = GetGrabPoint();
            var colliders = Physics.OverlapSphere(grabPoint.position, 0.2f, ~0, QueryTriggerInteraction.Collide);
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
