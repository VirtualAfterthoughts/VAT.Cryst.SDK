using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Entities
{
    [SelectionBase]
    public class CrystEntity : MonoBehaviour, IEntity
    {
        [SerializeField]
        private CrystBody[] _bodies = new CrystBody[0];

        [SerializeField]
        private CrystJoint[] _joints = new CrystJoint[0];

        public CrystBody[] Bodies => _bodies;

        public CrystJoint[] Joints => _joints;

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
    }
}
