using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction.Entities
{
    public class CrystJoint : MonoBehaviour
    {
        private CrystEntity _entity = null;
        public CrystEntity Entity { get => _entity; set => _entity = value; }
    }
}
