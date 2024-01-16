using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

using VAT.Entities;
using VAT.Entities.PhysX;

using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

namespace VAT.Avatars.Muscular
{
    public class RigidbodyPhysBone : PhysBone
    {
        protected CrystRigidbody _rigidbody;
        public CrystRigidbody Rigidbody => _rigidbody;
        public override CrystBody Body => _rigidbody;

        protected CrystConfigurableJoint _configurableJoint;
        public CrystConfigurableJoint ConfigurableJoint => _configurableJoint;
        public override CrystJoint Joint => _configurableJoint;

        protected JointAngularLimits _limits;
        public JointAngularLimits Limits => _limits;

        protected readonly string _name = null;

        public RigidbodyPhysBone(string name) : this(name, null, default) { }

        public RigidbodyPhysBone(string name, PhysBone parent, JointAngularLimits limits = default) {
            _gameObject = new GameObject();
            _transform = _gameObject.transform;
            _name = name;
            _gameObject.name = name;

            _configurableJoint = _gameObject.AddComponent<CrystConfigurableJoint>();

            _rigidbody = _configurableJoint.Rigidbody;
            _rigidbody.CreateItem();
            _rigidbody.Rigidbody.solverIterations = 48;
            _rigidbody.Rigidbody.solverVelocityIterations = 24;
            _rigidbody.Rigidbody.maxAngularVelocity = 90f;

            _limits = limits;

            Parent = parent;
        }

        public override void ResetAnchors(float3 center)
        {
            if (Joint.HasJoint && Joint.ConnectedBody != null)
            {
                _configurableJoint.ConfigurableJoint.autoConfigureConnectedAnchor = false;

                _configurableJoint.ConfigurableJoint.SetWorldAnchor(center);
                _configurableJoint.ConfigurableJoint.SetWorldConnectedAnchor(center);
            }
        }

        public override void SetAnchor(float3 anchor)
        {
            _configurableJoint.ConfigurableJoint.SetWorldAnchor(anchor);
        }

        public override void SetConnectedAnchor(float3 connectedAnchor)
        {
            _configurableJoint.ConfigurableJoint.SetWorldConnectedAnchor(connectedAnchor);
        }

        public override void AttachJoint(PhysBone bone)
        {
            // Recreate the joint
            _configurableJoint.DestroyItem();
            _configurableJoint.CreateItem();

            if (bone is RigidbodyPhysBone rigidbodyBone)
            {
                _configurableJoint.ConnectedRigidbody = rigidbodyBone.Rigidbody;

                _configurableJoint.ConfigurableJoint.linearLimitSpring = new SoftJointLimitSpring()
                {
                    spring = 5e+06f,
                    damper = 1e+06f,
                };
                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Limited);

                _configurableJoint.ConfigurableJoint.SetAngularLimits(_limits);
                _configurableJoint.ConfigurableJoint.angularXMotion = _limits.IsFree(Axis.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
                _configurableJoint.ConfigurableJoint.angularYMotion = _limits.IsFree(Axis.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
                _configurableJoint.ConfigurableJoint.angularZMotion = _limits.IsFree(Axis.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
            }
            else
            {
                Joint.ConnectedBody = null;

                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Free);
            }

            _configurableJoint.RecalculateJointSpace();
        }

        public override void SetMass(float kg)
        {
            Rigidbody rigidbody = ConfigurableJoint.Rigidbody;
            rigidbody.mass = kg;

            rigidbody.inertiaTensor = 0.1f * kg * Vector3.one;
            rigidbody.inertiaTensorRotation = Quaternion.identity;

            rigidbody.ResetCenterOfMass();
        }

        public override void Solve(SimpleTransform target) {
            var space = Joint.JointSpace;
            var previousTarget = space.RawTargetRotation;
            var currentTarget = space.InverseTransformTargetRotation(target.rotation, CrystSpace.WORLD);

            space.RawTargetRotation = currentTarget;
            space.RawTargetAngularVelocity = PhysicsExtensions.GetAngularVelocity(previousTarget, currentTarget);
        }
    }
}
