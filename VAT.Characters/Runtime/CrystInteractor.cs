using System.Collections;
using System.Collections.Generic;

using Unity.Mathematics;
using UnityEngine;

using VAT.Audio;
using VAT.Avatars;
using VAT.Avatars.Integumentary;

using VAT.Entities.PhysX;

using VAT.Input;
using VAT.Input.Haptic;
using VAT.Input.Skeleton;
using VAT.Interaction;
using VAT.Input.Data;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Characters
{
    public class CrystInteractor : MonoBehaviour, IInteractor, IAvatarTrackingOverride, IInteractorHoverModule, IInteractorFarHoverModule
    {
        public List<InteractableHost> hosts = new();
        public CrystRigidbody rb;
        public Handedness handedness;
        public IInputController controller;
        public IInputHand hand;
        public AvatarArm arm;
        public HandPoseData openPose;
        public HandPoseData closedPose;

        public AudioClip[] grabSounds = new AudioClip[0];

        public HapticImpulse grabHaptic = new(0.5f, 0.01f);

        public float grabCurl = 0.9f;

        public float grabRadius = 0.1f;

        private IGrippable _attachedGrip;

        private bool _isSnatching = false;

        private HoverHolder _hoverHolder = null;
        public HoverHolder HoverHolder => _hoverHolder;

        private HoverHolder _farHoverHolder = null;
        public HoverHolder FarHoverHolder => _farHoverHolder;

        private bool _isInteractionLocked = false;

        private AvatarGrabberPoint _grabberPoint;

        private InteractableHostGroup _armGroup;

        private void Awake()
        {
            rb = GetComponent<CrystRigidbody>();
            _hoverHolder = new HoverHolder(this);
            _farHoverHolder = new HoverHolder(this);

            RegisterModule(this);
        }

        private void Start()
        {
            _grabberPoint = new AvatarGrabberPoint
            {
                hand = arm.PhysArm.Hand,
                radius = grabRadius,
            };

            arm.RegisterTrackingOverride(this);

            ResetPose();

            _lastTarget = SimpleTransform.Create(transform.position, transform.rotation);

            var actions = hand.GetInputController().GetActions();
            actions.GrabAction.OnStateChanged += OnGrabStateChange;

            _armGroup = new InteractableHostGroup(hosts);
        }

        private void OnDisable()
        {
            ResetHover();
            DetachGrips();
        }

        private void OnGrabStateChange(bool state)
        {
            if (state && _attachedGrip == null && _hoverHolder.HoveringInteractable is IGrippable hoveringGrip)
            {
                // Validate grip
                if (!hoveringGrip.ValidateInteraction(this).valid)
                {
                    return;
                }

                AttachGrip(hoveringGrip);
            }
            else if (!state && _attachedGrip != null)
            {
                DetachGrips();
            }
        }

        private float _pinAmount = 0f;
        private SimpleTransform _lastTarget = SimpleTransform.Default;

        public IInteractable GetHoveringInteractable()
        {
            return HoverHolder.HoveringInteractable;
        }

        public IInteractable GetFarHoveringInteractable()
        {
            return FarHoverHolder.HoveringInteractable;
        }

        public SimpleTransform Solve(SimpleTransform rig, SimpleTransform targetInRig)
        {
            SimpleTransform result = targetInRig;

            SimpleTransform target = rig.Transform(result);
            var worldLastTarget = rig.Transform(_lastTarget);

            Vector3 velocity = PhysicsExtensions.GetLinearVelocity(worldLastTarget.position, target.position);
            _pinAmount = Mathf.Lerp(_pinAmount, 0f, Mathf.Clamp01(velocity.magnitude * 0.3f - 0.05f));

            _lastTarget = rig.InverseTransform(target);

            var values = GetValues(rig);

            var goal = values.Item1;
            goal.rotation = target.rotation;

            target = SimpleTransform.Lerp(target, goal, Mathf.Pow(values.Item2, 2f));

            result = rig.InverseTransform(target);

            if (!_isSnatching)
            {
                foreach (var interactorOverride in _interactorOverrides)
                {
                    result = interactorOverride.OnOverrideTarget(this, rig, result);
                }
            }

            _latestTar = rig.InverseTransform(SimpleTransform.Create(transform.position, transform.rotation));

            return result;
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

        private SimpleTransform _latestTar = SimpleTransform.Default;
        private SimpleTransform lastTar = SimpleTransform.Default;

        private float _lerp;

        public (SimpleTransform, float) GetValues(SimpleTransform rig)
        {
            if (_isSnatching)
            {
                var grabberPoint = GetPalm();
                var target = GrabTargetHelper.GetTargetInWorld(_attachedGrip, this);
                var grabPoint = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, _attachedGrip.GetDefaultPose()));
                grabPoint.rotation = target.rotation;

                var self = target.Transform(grabPoint.InverseTransform(SimpleTransform.Create(transform.position, transform.rotation)));
                lastTar = rig.InverseTransform(self);
                _lerp = Mathf.Lerp(_lerp, 1f, Time.deltaTime * 12f);
                return (self, _lerp);
            }
            else
            {
                _lerp = Mathf.Lerp(_lerp, _pinAmount, Time.deltaTime * 32f);
                return (rig.Transform(lastTar), _lerp);
            }
        }


        public void LateUpdate()
        {
            var controller = hand.GetInputController();
            var blendPose = controller.GetHandPose();
            arm.DataArm.Hand.SetBlendPose(blendPose);

            var actions = controller.GetActions();

            OnUpdateHover();

            if (_attachedGrip == null)
            {
                if (_hoverHolder.HoveringInteractable is IGrippable hoveringGrip && !actions.GrabAction.State && hoveringGrip.GetClosedPose(this).valid)
                {
                    arm.DataArm.Hand.SetClosedPose(hoveringGrip.GetClosedPose(this).data);
                }
                else
                {
                    ResetPose();
                }
            }

            if (_isSnatching)
            {
                var grabberPoint = GetPalm();
                var worldTarget = GrabTargetHelper.GetTargetInWorld(_attachedGrip, this);
                var interactorTarget = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, _attachedGrip.GetDefaultPose()));

                float distance = math.length(worldTarget.position - interactorTarget.position);

                var (valid, data) = _attachedGrip.GetClosedPose(this);
                if (valid)
                {
                    var targetPose = data;
                    var newPose = HandPoseCreator.Lerp(openPose, targetPose, grabRadius / distance);

                    SetClosedPose(newPose);
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

                    SendGripHaptic();

                    PlayGrabSound();
                }
            }
            else
            {
                _attachedGrip?.OnAttachUpdate(this);
            }
        }

        public void AttachGrip(IGrippable grip)
        {
            grip.OnAttachConfirm(this);
            _attachedGrip = grip;
            _isSnatching = true;

            grip.GetHost()?.AttachGroup(_armGroup);

            ResetHover();
        }

        private void PlayGrabSound()
        {
            if (grabSounds != null && grabSounds.Length > 0)
            {
                var settings = AudioPlaySettings.Default;
                settings.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

                var info = new AudioSpawner.AudioRequestInfo()
                {
                    clip = grabSounds[0],
                    position = transform.position,
                    settings = settings,
                };

                AudioSpawner.Spawn(info);
            }
        }

        private void ResetHover()
        {
            _hoverHolder.HoveringInteractable = null;
            _farHoverHolder.HoveringInteractable = null;
        }

        private void SendGripHaptic()
        {
            var haptor = controller?.GetHaptor();

            if (haptor != null)
            {
                HapticHelper.SendSoftHaptic(haptor, grabHaptic);
            }
        }

        public void DetachGrips()
        {
            ResetPose();

            if (_attachedGrip != null)
            {
                DetachGrip(_attachedGrip);
                _attachedGrip = null;
                _isSnatching = false;

                SendGripHaptic();
            }

            ResetPin();
        }

        private void ResetPin()
        {
            _pinAmount = 1f;
            lastTar = _latestTar;
        }

        public void DetachGrip(IGrippable grip)
        {
            grip.OnDetachConfirm(this);

            grip.GetHost()?.DetachGroup(_armGroup);

            _attachedGrip = null;
            _isSnatching = false;
        }

        private Vector3 GetFarOrigin()
        {
            return Camera.main.transform.position;
        }

        private Vector3 GetFarForward()
        {
            Vector3 farForward = math.normalize(arm.PhysArm.Hand.Hand.Transform.position - arm.PhysArm.UpperArm.Transform.position);
            Vector3 cameraForward = math.normalize((Vector3)arm.PhysArm.Hand.Hand.Transform.position - Camera.main.transform.position);
            farForward = Vector3.Lerp(farForward, cameraForward, 0.5f).normalized;
            return farForward;
        }

        protected void OnUpdateHover()
        {
            if (_attachedGrip != null)
                return;

            var grabCenter = _grabberPoint.GetProximityCenter();
            var colliders = Physics.OverlapSphere(grabCenter.position, grabRadius, ~0, QueryTriggerInteraction.Collide);
            
            var nearHover = GetInteractableFromColliders(colliders, HoverFlags.NEAR);

            _hoverHolder.HoveringInteractable = nearHover;

            IInteractable farHover = null;

            if (nearHover == null)
            {
                var farForward = GetFarForward();
                float maxDistance = 7f;
                var farOrigin = GetFarOrigin();
                var endPosition = farOrigin + farForward * maxDistance;

                if (Physics.Raycast(farOrigin, farForward, out var hitInfo, maxDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    endPosition = hitInfo.point;
                }

                float radius = Mathf.Lerp(0f, 1f, Vector3.Distance(farOrigin, endPosition) / maxDistance);

                var farColliders = Physics.OverlapCapsule(farOrigin, endPosition, radius);
                farHover = GetInteractableFromColliders(farColliders, HoverFlags.FAR);
            }

            _farHoverHolder.HoveringInteractable = farHover;
        }

        private IInteractable GetInteractableFromColliders(Collider[] colliders, HoverFlags flags)
        {
            IInteractable interactable = null;
            float lowestPriority = float.PositiveInfinity;

            foreach (var collider in colliders)
            {
                var component = collider.gameObject.GetComponentInParent<IInteractable>();

                if (component != null && (component.GetHoverFlags() & flags) != 0)
                {
                    var (valid, priority) = component.ValidateInteraction(this);

                    if (valid && priority < lowestPriority)
                    {
                        interactable = component;
                        lowestPriority = priority;
                    }
                }
            }

            return interactable;
        }

        public void OnDrawGizmosSelected()
        {
            if (_attachedGrip != null)
                return;

            var grabCenter = _grabberPoint.GetProximityCenter();
            Gizmos.DrawWireSphere(grabCenter.position, grabRadius);
        }

        public void OnDrawGizmos()
        {
            var grabCenter = GetFarOrigin();
            var forward = GetFarForward();

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(grabCenter, grabCenter + forward * 4f);
        }

        public Rigidbody GetRigidbody()
        {
            return rb.Rigidbody;
        }

        public IInputHand GetInputHand()
        {
            return hand;
        }

        public InteractorTargetData GetTargetData()
        {
            return new InteractorTargetData()
            {
                rig = arm.PhysRig.Transform,
                targetInRig = arm.DataRig.Transform.InverseTransform(arm.DataArm.EndTarget)
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

        public PalmPoint GetPalm()
        {
            return _grabberPoint;
        }

        private readonly List<IInteractorModule> _modules = new();

        public TModule GetModule<TModule>() where TModule : IInteractorModule
        {
            foreach (var module in _modules)
            {
                if (module is TModule genericModule)
                {
                    return genericModule;
                }
            }

            return default;
        }

        public void RegisterModule(IInteractorModule module)
        {
            _modules.Add(module);
        }

        public void DeregisterModule(IInteractorModule module)
        {
            _modules.Remove(module);
        }

        public void ResetPose()
        {
            arm.DataArm.Hand.SetOpenPose(openPose);
            arm.DataArm.Hand.SetClosedPose(closedPose);
        }

        public void SetClosedPose(HandPoseData pose)
        {
            arm.DataArm.Hand.SetClosedPose(pose);
        }

        public void SetOpenPose(HandPoseData pose)
        {
            arm.DataArm.Hand.SetOpenPose(pose);
        }
    }
}
