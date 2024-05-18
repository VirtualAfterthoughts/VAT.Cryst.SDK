using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class NOTGate : Gate
    {
        [SerializeField]
        private Port _receiver;

        public override List<Port> Receivers => new() { _receiver };

        public override Signal GetOutputSignal()
        {
            float value = 1f - Mathf.Abs(_receiver.CurrentSignal.value);

            return new Signal()
            {
                value = value,
            };
        }
    }
}
