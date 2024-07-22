using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Entities
{
    public class CrystJoint : MonoBehaviour, IEntityChild
    {
        private IEntity _parentEntity = null;
        public IEntity ParentEntity { get => _parentEntity; set => _parentEntity = value; }
    }
}
