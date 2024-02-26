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
    public class CrystInteractor : MonoBehaviour, IInteractor, IAvatarTrackingOverride
    {
        public List<InteractableHost> hosts = new List<InteractableHost>();
        public CrystRigidbody rb;
        public Handedness handedness;
        public IInputController controller;
        public IInputHand hand;
        public AvatarArm arm;
        public HandPoseData openPose;
        public HandPoseData closedPose;

        public float grabCurl = 0.9f;

        public float grabRadius = 0.086848f;

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

        private AvatarGrabberPoint _grabberPoint;

        private void Awake()
        {
            rb = GetComponent<CrystRigidbody>();
        }

        private void Start()
        {
            _grabberPoint = new AvatarGrabberPoint
            {
                arm = arm,
                radius = grabRadius,
            };

            arm.RegisterTrackingOverride(this);

            arm.DataArm.Hand.SetOpenPose(openPose);
            arm.DataArm.Hand.SetClosedPose(closedPose);

            _lastTarget = SimpleTransform.Create(transform);
        }

        private float _pinAmount = 0f;
        private SimpleTransform _lastTarget = SimpleTransform.Default;

        public SimpleTransform Solve(SimpleTransform rig, SimpleTransform targetInRig)
        {
            SimpleTransform result = targetInRig;
            foreach (var interactorOverride in _interactorOverrides)
            {
                result = interactorOverride.OnOverrideTarget(this, rig, result);
            }

            SimpleTransform target = rig.Transform(result);

            Vector3 velocity = PhysicsExtensions.GetLinearVelocity(_lastTarget.position, target.position);
            _pinAmount = Mathf.Lerp(_pinAmount, 0f, Mathf.Clamp01(velocity.magnitude * 0.3f - 0.05f));

            _lastTarget = target;

            var values = GetValues();

            var goal = values.Item1;
            goal.rotation = target.rotation;

            target = SimpleTransform.Lerp(target, values.Item1, values.Item2);

            return rig.InverseTransform(target);
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
                var target = _attachedGrip.GetTargetInWorld(GetGrabberPoint());
                target.lossyScale = 1f;
                var grabPoint = _attachedGrip.GetGrabPoint(this);
                grabPoint.rotation = target.rotation;

                var self = target.Transform(grabPoint.InverseTransform(SimpleTransform.Create(transform)));
                lastTar = self;
                _lerp = Mathf.Lerp(_lerp, 1f, Time.deltaTime * 12f);
                return (self, _lerp);
            }
            else
            {
                _lerp = Mathf.Lerp(_lerp, _pinAmount, Time.deltaTime * 32f);
                return (lastTar, _lerp);
            }
        }

        private bool _wasGripPose = false;

        public void LateUpdate()
        {
            var blendPose = hand.GetHandPose();
            arm.DataArm.Hand.SetBlendPose(blendPose);

            float maxCurl = 0f;

            foreach (var finger in blendPose.fingers)
            {
                maxCurl = Mathf.Max(maxCurl, finger.GetCurl());
            }

            bool gripPose = maxCurl > grabCurl;

            OnUpdateHover();

            if (!_attachedGrip)
            {
                if (HoveringInteractable is Grip garp && !gripPose && garp.GetClosedPose(this).valid)
                {
                    arm.DataArm.Hand.SetClosedPose(garp.GetClosedPose(this).data);
                }
                else
                {
                    arm.DataArm.Hand.SetClosedPose(closedPose);
                }
            }

            if (gripPose && !_wasGripPose && !_attachedGrip && HoveringInteractable is Grip grp)
            {
                AttachGrip(grp);
            }
            else if (maxCurl < grabCurl && _attachedGrip)
            {
                DetachGrips();
            }

            _wasGripPose = gripPose;

            if (_isSnatching)
            {
                float distance = math.length(_attachedGrip.GetTargetInWorld(GetGrabberPoint()).position - _attachedGrip.GetGrabPoint(this).position);

                var (valid, data) = _attachedGrip.GetClosedPose(this);
                if (valid)
                {
                    var targetPose = data;
                    var newPose = HandPoseCreator.Lerp(openPose, targetPose, Mathf.Pow(distance / grabRadius - 1f, 2f));

                    arm.DataArm.Hand.SetClosedPose(newPose);
                }

                _attachedGrip.OnAttachUpdate(this);

                if (distance <= 0.05f)
                {
                    _isSnatching = false;
                    _attachedGrip.OnAttachComplete(this);

                    if (valid)
                    {
                        arm.DataArm.Hand.SetClosedPose(data);
                    }

                    ResetPin();
                }
            }
            else if (_attachedGrip)
            {
                _attachedGrip.OnAttachUpdate(this);
            }
        }

        public void AttachGrip(Grip grip)
        {
            grip.OnAttachConfirm(this);
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
            _pinAmount = 1f;
            lastTar = SimpleTransform.Create(transform);
        }

        public void DetachGrip(Grip grip)
        {
            grip.OnDetachConfirm(this);

            ToggleCollsion(grip, false);

            _attachedGrip = null;
            _isSnatching = false;
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

            var grabCenter = _grabberPoint.GetGrabCenter();
            var colliders = Physics.OverlapSphere(grabCenter.position, grabRadius, ~0, QueryTriggerInteraction.Collide);
            
            IInteractable interactable = null;
            float lowestPriority = float.PositiveInfinity;

            foreach (var collider in colliders)
            {
                var component = collider.gameObject.GetComponentInParent<IInteractable>();

                if (component != null) {
                    var (valid, priority) = component.ValidateInteractable(this);

                    if (valid && priority < lowestPriority)
                    {
                        interactable = component;
                        lowestPriority = priority;
                    }
                }
            }

            HoveringInteractable = interactable;
        }

        public void OnDrawGizmosSelected()
        {
            if (_attachedGrip)
                return;

            var grabCenter = _grabberPoint.GetGrabCenter();
            Gizmos.DrawWireSphere(grabCenter.position, grabRadius);
        }

        public Rigidbody GetRigidbody()
        {
            return rb.Rigidbody;
        }

        public float GetGripForce()
        {
            if (controller.TryGetGrip(out var grip) && controller.TryGetTrigger(out var trigger))
                return (grip.GetForce() * 0.75f) + (trigger.GetForce() * 0.25f);

            return 0f;
        }

        public InteractorTargetData GetTargetData()
        {
            return new InteractorTargetData()
            {
                rig = arm.PhysRig.Transform,
                targetInRig = arm.DataRig.Transform.InverseTransform(arm.DataArm.Target)
            };
        }

        private List<IInteractorOverride> _interactorOverrides = new();

        public void RegisterOverride(IInteractorOverride interactorOverride)
        {
            _interactorOverrides.Add(interactorOverride);
        }

        public void UnregisterOverride(IInteractorOverride interactorOverride)
        {
            _interactorOverrides.Remove(interactorOverride);
        }

        public IGrabberPoint GetGrabberPoint()
        {
            return _grabberPoint;
        }
    }
}
