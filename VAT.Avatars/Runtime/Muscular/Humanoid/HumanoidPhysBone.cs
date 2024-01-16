using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;

using VAT.Avatars.Skeletal;
using VAT.Entities;
using VAT.Entities.PhysX;

using VAT.Shared.Data;
using VAT.Shared.Extensions;
using VAT.Shared.Utilities;

namespace VAT.Avatars.Muscular
{
    public sealed class HumanoidPhysBone : RigidbodyPhysBone {
        private MeshCollider _meshCollider = null;
        private Mesh _mesh = null;

        public float spring = 500f;
        public float damper = 10f;

        public Float3DerivativeTracker positionTracker = new(2);

        public HumanoidPhysBone(string name) : base(name) { }

        public HumanoidPhysBone(string name, PhysBone parent, JointAngularLimits limits = default) 
            : base(name, parent, limits) { }

        public void SetTransformRoot(Transform root) {
            _transform.parent = root;
        }

        public override void Destroy() {
            _gameObject.TryDestroy();
            _mesh.TryDestroy();
        }

        public void SetMesh(Mesh mesh) {
            if (_meshCollider != null)
                RemoveCollider(_meshCollider);

            _meshCollider.TryDestroy();

            _mesh = mesh;

            if (mesh == null)
                return;

            mesh.name = _name;

            _meshCollider = _gameObject.AddComponent<MeshCollider>();
            _meshCollider.convex = true;
            _meshCollider.cookingOptions = (MeshColliderCookingOptions)~0;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.contactOffset *= 4f;

            InsertCollider(_meshCollider);
        }

        public override void SetMass(float kg) {
            Rigidbody rigidbody = ConfigurableJoint.Rigidbody;
            rigidbody.mass = kg;

            rigidbody.inertiaTensor = 0.1f * kg * Vector3.one;
            rigidbody.inertiaTensorRotation = Quaternion.identity;

            rigidbody.ResetCenterOfMass();
        }

        public void ConfigureJoint() {
            // Setup positional forces
            JointDrive positionDrive = default;

            var joint = _configurableJoint.ConfigurableJoint;
            joint.xDrive = joint.yDrive = joint.zDrive = positionDrive;

            var mass = ConfigurableJoint.Rigidbody.Rigidbody.mass;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive()
            {
                positionSpring = 5e+06f,
                positionDamper = 1e+05f,
                maximumForce = 12000f * mass,
            };
        }

        public override void Solve(SimpleTransform target) {
            var space = Joint.JointSpace;
            var previousTarget = space.RawTargetRotation;
            var currentTarget = space.InverseTransformTargetRotation(target.rotation, CrystSpace.WORLD);

            space.RawTargetRotation = currentTarget;
            space.RawTargetAngularVelocity = PhysicsExtensions.GetAngularVelocity(previousTarget, currentTarget);
        }

        public override void AttachJoint(PhysBone bone = null)
        {
            // Recreate the joint
            _configurableJoint.DestroyItem();
            _configurableJoint.CreateItem();

            if (bone is HumanoidPhysBone rigidbodyBone) {
                _configurableJoint.ConnectedBody = rigidbodyBone.Joint.Body;

                _configurableJoint.ConfigurableJoint.linearLimitSpring = new SoftJointLimitSpring() {
                    spring = 5e+06f,
                    damper = 1e+06f,
                };
                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Limited);

                _configurableJoint.ConfigurableJoint.SetAngularLimits(_limits);
                _configurableJoint.ConfigurableJoint.angularXMotion = _limits.IsFree(Axis.X) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
                _configurableJoint.ConfigurableJoint.angularYMotion = _limits.IsFree(Axis.Y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
                _configurableJoint.ConfigurableJoint.angularZMotion = _limits.IsFree(Axis.Z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited;
            }
            else {
                Joint.ConnectedBody = null;

                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Free);
            }

            _configurableJoint.RecalculateJointSpace();
        }
    }
}
