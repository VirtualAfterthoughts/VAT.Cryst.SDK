using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public abstract class Gate : Node
    {
        [SerializeField]
        private List<Port> _outputs = new();

        public override List<Port> Outputs => _outputs;
    }
}
