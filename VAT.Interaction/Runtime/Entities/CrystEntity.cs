using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Entities
{
    [SelectionBase]
    public class CrystEntity : MonoBehaviour, IEntity
    {
        [SerializeField]
        private GameObject _root = null;

        [SerializeField]
        private CrystBody[] _bodies = new CrystBody[0];

        [SerializeField]
        private CrystJoint[] _joints = new CrystJoint[0];

        public CrystBody[] Bodies => _bodies;

        public CrystJoint[] Joints => _joints;

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
            foreach (var body in Bodies)
            {
                body.ParentEntity = this;
            }

            foreach (var joint in Joints)
            {
                joint.ParentEntity = this;
            }
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
