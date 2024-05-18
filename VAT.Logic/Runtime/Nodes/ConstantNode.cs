using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class ConstantNode : Node
    {
        [SerializeField]
        [Range(-1f, 1f)]
        private float _value = 1f;

        [SerializeField]
        private List<Port> _outputs = new();

        public override List<Port> Receivers => null;

        public override List<Port> Outputs => _outputs;

        public override Signal GetOutputSignal()
        {
            return new Signal()
            {
                value = _value
            };
        }
    }
}
