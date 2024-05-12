using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    using Unity.Mathematics;
    using UnityEngine.XR;
    using VAT.Avatars;
    using VAT.Avatars.Integumentary;
    using VAT.Interaction;
    using VAT.Shared.Data;
    using VAT.Shared.Extensions;

    public class ForcePullAbility : IAvatarAbility
    {
        public class ForcePullTracker
        {
            private IInteractor _interactor;

            private IInteractorHoverModule _hoverModule;
            private IInteractorFarHoverModule _farHoverModule;

            private IGrippable _pullingGrip;

            private ConfigurableJoint _joint;

            public ForcePullTracker(IInteractor interactor)
            {
                _interactor = interactor;

                _hoverModule = interactor.GetModule<IInteractorHoverModule>();
                _farHoverModule = interactor.GetModule<IInteractorFarHoverModule>();

                var state = _interactor.GetInputHand().GetInputController().GetActions();
                state.AbilityGrabAction.OnStateChanged += OnActionGrabStateChanged;
                state.GrabAction.OnStateChanged += OnGrabStateChanged;
            }

            public void Cleanup()
            {
                var state = _interactor.GetInputHand().GetInputController().GetActions();
                state.AbilityGrabAction.OnStateChanged -= OnActionGrabStateChanged;
                state.GrabAction.OnStateChanged -= OnGrabStateChanged;

                _interactor = null;
            }

            public void OnGrabStateChanged(bool state)
            {
                if (!state && _pullingGrip != null)
                {
                    CancelPull();
                }
            }

            public void OnActionGrabStateChanged(bool state)
            {
                if (state && _pullingGrip == null)
                {
                    var near = _hoverModule.GetHoveringInteractable();
                    var far = _farHoverModule.GetFarHoveringInteractable();

                    if (near == null && far != null && far is IGrippable grip && grip.IsInteractable())
                    {
                        BeginPull(grip);
                    }
                }
            }

            private void ApplyDrag(Rigidbody grip, Rigidbody interactor)
            {
                var gripVel = grip.velocity;
                var interactorVel = interactor.velocity;

                var force = 50f * grip.mass * (interactorVel - gripVel);
                force = Vector3.ClampMagnitude(force, 1000f);
                grip.AddForce(force, ForceMode.Force);

                var gripAngVel = grip.angularVelocity;
                var interactorAngVel = interactor.angularVelocity;

                var torque = (interactorAngVel - gripAngVel) * 50f;
                torque = Vector3.ClampMagnitude(torque, 1000f);
                grip.AddTorque(torque, ForceMode.Acceleration);
            }

            private void UpdatePull()
            {
                ApplyDrag(_pullingGrip.GetHost().GetRigidbody(), _interactor.GetRigidbody());

                var grabberPoint = _interactor.GetPalm();
                var worldTarget = _pullingGrip.GetTargetInWorld(grabberPoint, _pullingGrip.GetDefaultPose());
                var interactorTarget = grabberPoint.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(grabberPoint, _pullingGrip.GetDefaultPose()));

                float distance = math.length(worldTarget.position - interactorTarget.position);

                if (distance <= 0.05f)
                {
                    _interactor.AttachGrip(_pullingGrip);
                    CancelPull();
                }
            }

            private void BeginPull(IGrippable grip)
            {
                var host = grip.GetHost();

                if (host == null || host.GetRigidbody() == null || host.GetRigidbody().isKinematic)
                {
                    return;
                }

                _pullingGrip = grip;

                var rb = host.GetRigidbody();
                var grabPoint = _interactor.GetPalm();
                var targetInInteractor = GrabTargetHelper.GetTargetInInteractor(grabPoint, grip.GetDefaultPose());

                var targetInWorld = grip.GetTargetInWorld(grabPoint, grip.GetDefaultPose());
                var targetInHost = SimpleTransform.Create(rb.position, rb.rotation, rb.transform.localScale).InverseTransform(targetInWorld);

                var interactorInHost = GrabTargetHelper.GetTargetInHost(grip, grabPoint);
                var worldInteractor = rb.transform.TransformRotation(interactorInHost.rotation) * grabPoint.GetHostTransform().Transform(targetInInteractor).InverseTransformRotation(_interactor.GetRigidbody().transform.rotation);

                _joint = _interactor.GetRigidbody().gameObject.AddComponent<ConfigurableJoint>();

                _joint.connectedBody = rb;
                var drive = new JointDrive() { positionSpring = 1000f, positionDamper = 0f, maximumForce = 1000f };
                _joint.xDrive = _joint.yDrive = _joint.zDrive = drive;
                _joint.rotationDriveMode = RotationDriveMode.Slerp;
                _joint.slerpDrive = new JointDrive() { positionSpring = 1000f, positionDamper = 50f, maximumForce = 1000f };
                _joint.autoConfigureConnectedAnchor = false;
                _joint.anchor = targetInInteractor.position;
                _joint.connectedAnchor = targetInHost.position;

                _joint.UpdateRotation(_joint.transform, worldInteractor);

                grip.DisableInteraction();
            }

            private void CancelPull()
            {
                if (_pullingGrip == null)
                {
                    return;
                }

                _pullingGrip.EnableInteraction();

                var rb = _pullingGrip.GetHost().GetRigidbody();
                var interactorRb = _interactor.GetRigidbody();

                rb.velocity = interactorRb.velocity;
                rb.angularVelocity = interactorRb.angularVelocity;

                _pullingGrip = null;
                Object.Destroy(_joint);
            }

            public void OnFixedUpdate(float deltaTime)
            {
                if (_pullingGrip != null)
                {
                    UpdatePull();
                    return;
                }
            }
        }

        private readonly List<ForcePullTracker> _trackers = new();

        public void OnInitiateAvatar(Avatar avatar, IAvatarRig rig)
        {
            var interactors = rig.GetCurrentInteractors();
            foreach (var interactor in interactors)
            {
                _trackers.Add(new ForcePullTracker(interactor));
            }

            rig.RigManager.OnManagerFixedUpdate += OnFixedUpdate;
        }

        public void OnDeinitiateAvatar(Avatar avatar, IAvatarRig rig)
        {
            rig.RigManager.OnManagerFixedUpdate -= OnFixedUpdate;

            foreach (var tracker in _trackers)
            {
                tracker.Cleanup();
            }

            _trackers.Clear();
        }

        private void OnFixedUpdate(float deltaTime)
        {
            foreach (var tracker in _trackers)
            {
                tracker.OnFixedUpdate(deltaTime);
            }
        }
    }
}
