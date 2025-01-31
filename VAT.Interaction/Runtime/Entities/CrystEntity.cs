using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace VAT.Interaction.Entities
{
    [SelectionBase]
    public class CrystEntity : MonoBehaviour
    {
        [SerializeField]
        private GameObject _root = null;

        [SerializeField]
        private CrystBody[] _bodies = new CrystBody[0];

        [SerializeField]
        private CrystJoint[] _joints = new CrystJoint[0];

        private readonly List<CrystBody> _runtimeBodies = new();
        private readonly List<CrystJoint> _runtimeJoints = new();

        public IReadOnlyList<CrystBody> DefaultBodies => _bodies;

        public IReadOnlyList<CrystJoint> DefaultJoints => _joints;

        public IReadOnlyList<CrystBody> Bodies => _runtimeBodies;

        public IReadOnlyList<CrystJoint> Joints => _runtimeJoints;

        public GameObject Root
        {
            get
            {
                if (_root == null)
                {
                    _root = gameObject;
                }

                return _root;
            }
            set
            {
                _root = value;
            }
        }

        private void Awake()
        {
            foreach (var body in DefaultBodies)
            {
                AddBody(body);
            }

            foreach (var joint in DefaultJoints)
            {
                AddJoint(joint);
            }
        }

        private void OnDestroy()
        {
            foreach (var body in Bodies.ToArray())
            {
                RemoveBody(body);
            }

            foreach (var joint in Joints.ToArray())
            {
                RemoveJoint(joint);
            }
        }

        public void AddBody(CrystBody body)
        {
            _runtimeBodies.Add(body);
            body.Entity = this;
        }

        public void RemoveBody(CrystBody body)
        {
            _runtimeBodies.Remove(body);
            body.Entity = null;
        }

        public void AddJoint(CrystJoint joint)
        {
            _runtimeJoints.Add(joint);
            joint.Entity = this;
        }

        public void RemoveJoint(CrystJoint joint)
        {
            _runtimeJoints.Remove(joint);
            joint.Entity = null;
        }

        public void Freeze(bool frozen = true)
        {
            foreach (var body in Bodies)
            {
                body.Freeze(frozen);
            }
        }

        public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            foreach (var body in Bodies)
            {
                body.AddForce(force, mode);
            }
        }

        public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            foreach (var body in Bodies)
            {
                body.AddTorque(torque, mode);
            }
        }
    }
}
