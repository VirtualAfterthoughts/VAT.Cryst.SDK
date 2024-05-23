using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class NANDGate : Gate
    {
        [SerializeField]
        private List<Port> _receivers = new();

        public override List<Port> Receivers => _receivers;

        public override Signal GetSignal()
        {
            float minimum = 0f;
            bool hasInput = false;

            foreach (var input in Receivers)
            {
                if (!hasInput)
                {
                    minimum = input.CurrentSignal.value;
                    hasInput = true;
                    continue;
                }

                minimum = Mathf.Min(minimum, input.CurrentSignal.value);
            }

            var signal = new Signal()
            {
                value = 1f - Mathf.Abs(minimum)
            };

            return signal;
        }
    }
}
