using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Math;

namespace VAT.Interaction
{
    public class GenericGripJoint : IGripJoint
    {
        private Grip _grip;
        private IInteractor _interactor;
        private ConfigurableJoint _joint = null;

        private JointSpace _jointSpace = null;

        private bool _isFree = false;

        public void AttachJoints(IInteractor interactor, Grip grip)
        {
            _grip = grip;

            _interactor = interactor;

            var rb = interactor.GetRigidbody();

            var palm = interactor.GetPalm();
            var grabPoint = palm.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(palm, grip.GetDefaultPose()));

            var target = grip.GetTargetInHost(interactor);
            var hostTransform = grip.GetHostGameObject().transform;

            // Match grab rotation, so that the joint initializes with proper target
            // Since we can't set anchorRotation in Unity
            var grabPointRotation = hostTransform.TransformRotation(target.Rotation);
            var initialRotation = rb.transform.rotation;
            rb.transform.rotation = grabPointRotation * (grabPoint.InverseTransformRotation(rb.transform.rotation));

            var joint = rb.gameObject.AddComponent<ConfigurableJoint>();
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            joint.angularXLimitSpring = joint.angularYZLimitSpring = new SoftJointLimitSpring() { spring = 5000000f, damper = 10000f };

            var host = grip.GetHost();

            if (host != null)
            {
                joint.connectedBody = host.Rigidbody;
            }

            joint.enableCollision = true;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = grip.GetPivotInInteractor(interactor.GetPalm(), grip.GetClosedPose(interactor).data).Position;
            joint.SetWorldConnectedAnchor(grip.GetPivotInWorld(interactor.GetPalm(), grip.GetClosedPose(interactor).data).Position);

            joint.swapBodies = true;

            _joint = joint;

            _jointSpace = new JointSpace(joint);

            rb.transform.rotation = initialRotation;
        }

        public void DetachJoints()
        {
            GameObject.Destroy(_joint);
            _joint = null;
        }

        public void UpdateJoints(float friction)
        {
            if (_isFree)
            {
                _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = Mathf.Lerp(_joint.xDrive.positionSpring, 5000f, Time.deltaTime * 0.5f), positionDamper = 0f, maximumForce = float.PositiveInfinity };
                _joint.slerpDrive = new JointDrive() { positionSpring = 0f, positionDamper = Mathf.Lerp(_joint.slerpDrive.positionDamper, 900f, Time.deltaTime * 0.5f), maximumForce = float.PositiveInfinity };
            }
            else
            {
                float force = Mathf.LerpUnclamped(0f, 1000f, friction);

                _joint.slerpDrive = new JointDrive()
                {
                    positionSpring = force * 10f,
                    positionDamper = force * 0.05f,
                    maximumForce = force * 0.1f
                };
            }

            var palm = _interactor.GetPalm();
            var target = GrabTargetHelper.GetTargetInWorld(_grip, _interactor);
            var selfTarget = palm.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(palm, _grip.GetDefaultPose()));

            target = target.Transform(selfTarget.InverseTransform(palm.GetHostTransform()));

            var lastTargetRotation = _joint.targetRotation;
            _jointSpace.SetTargetRotationWorld(target.Rotation);

            _joint.targetAngularVelocity = Derivatives.GetAngularVelocity(lastTargetRotation, _joint.targetRotation);

            UpdateAnchors();
        }

        private void UpdateAnchors()
        {
            _joint.swapBodies = false;

            var palm = _interactor.GetPalm();
            var pose = _grip.GetClosedPose(_interactor).data;
            var palmPoint = palm.GetHostTransform().Transform(GrabTargetHelper.GetTargetInInteractor(palm, pose));

            var pivotInWorld = _grip.GetPivotInWorld(palm, pose);

            Quaternion grabPointRotation = pivotInWorld.Rotation;
            var initialRotation = _joint.transform.rotation;
            _joint.transform.rotation = grabPointRotation * (palmPoint.InverseTransformRotation(_joint.transform.rotation));

            _joint.anchor = _grip.GetPivotInInteractor(palm, pose).Position;
            _joint.SetWorldConnectedAnchor(pivotInWorld.Position);

            _joint.swapBodies = true;

            _joint.transform.rotation = initialRotation;
        }

        public void FreeJoints()
        {
            _joint.SetMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);

            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 5f, positionDamper = 0f, maximumForce = float.MaxValue };

            _joint.linearLimit = new SoftJointLimit() { limit = Vector3.Distance(_joint.GetWorldAnchor(), _joint.GetWorldConnectedAnchor()) };

            _isFree = true;
        }

        public void LockJoints()
        {
            _joint.SetMotion(ConfigurableJointMotion.Locked, ConfigurableJointMotion.Limited);

            _joint.lowAngularXLimit = new SoftJointLimit() { limit = -40f };
            _joint.highAngularXLimit = _joint.angularYLimit = _joint.angularZLimit = new SoftJointLimit() { limit = 40f };

            _joint.linearLimit = new SoftJointLimit() { limit = 0.05f };
            _joint.xDrive = _joint.yDrive = _joint.zDrive = new JointDrive() { positionSpring = 500000f, positionDamper = 1000f, maximumForce = 500000f };
            _joint.slerpDrive = new JointDrive() { positionSpring = 9000f, positionDamper = 500f, maximumForce = 1200f };

            _isFree = false;
        }
    }
}
