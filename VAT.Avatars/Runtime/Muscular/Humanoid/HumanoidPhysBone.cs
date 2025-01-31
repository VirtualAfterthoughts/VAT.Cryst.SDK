using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;

using VAT.Entities;

using VAT.Shared.Data;
using VAT.Shared.Extensions;

namespace VAT.Avatars.Muscular
{
    public sealed class HumanoidPhysBone : RigidbodyPhysBone
    {
        private MeshCollider _meshCollider = null;
        private Mesh _mesh = null;

        public HumanoidPhysBone(string name) : base(name) { }

        public HumanoidPhysBone(string name, PhysBone parent, JointAngularLimits limits = default)
            : base(name, parent, limits) { }

        public void SetTransformRoot(Transform root)
        {
            _transform.parent = root;
        }

        public override void Destroy()
        {
            _gameObject.TryDestroy();
            _mesh.TryDestroy();
        }

        public void SetMesh(Mesh mesh)
        {
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

            InsertCollider(_meshCollider);
        }

        public void ConfigureJoint()
        {
            // Setup positional forces
            JointDrive positionDrive = default;

            var joint = _configurableJoint.ConfigurableJoint;
            joint.xDrive = joint.yDrive = joint.zDrive = positionDrive;

            var mass = ConfigurableJoint.Rigidbody.Rigidbody.mass;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive()
            {
                positionSpring = 5e+06f,
                positionDamper = 2e+05f,
                maximumForce = 1200f * mass,
            };
        }

        public void ConfigureJoint(float newtons)
        {
            // Setup positional forces
            JointDrive positionDrive = default;

            var joint = _configurableJoint.ConfigurableJoint;
            joint.xDrive = joint.yDrive = joint.zDrive = positionDrive;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            joint.slerpDrive = new JointDrive()
            {
                positionSpring = newtons * 40f,
                positionDamper = newtons,
                maximumForce = newtons,
            };
        }

        public override void AttachJoint(PhysBone bone = null)
        {
            // Recreate the joint
            _configurableJoint.DestroyItem();
            _configurableJoint.CreateItem();

            if (bone is HumanoidPhysBone rigidbodyBone)
            {
                _configurableJoint.ConnectedBody = rigidbodyBone.Joint.Body;

                _configurableJoint.ConfigurableJoint.linearLimitSpring = new SoftJointLimitSpring()
                {
                    spring = 5e+06f,
                    damper = 1e+06f,
                };
                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Limited, ConfigurableJointMotion.Free);
            }
            else
            {
                Joint.ConnectedBody = null;

                _configurableJoint.ConfigurableJoint.SetJointMotion(ConfigurableJointMotion.Free);
            }

            _configurableJoint.RecalculateJointSpace();
        }
    }
}
