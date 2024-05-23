using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class NOTGate : Gate
    {
        [SerializeField]
        private List<Port> _receivers = new();

        public override List<Port> Receivers => _receivers;

        public override Signal GetSignal()
        {
            float averageValue = 0f;
            
            foreach (var receiver in Receivers)
            {
                averageValue += receiver.CurrentSignal.value;
            }

            averageValue /= Receivers.Count;

            float value = 1f - Mathf.Abs(averageValue);

            return new Signal()
            {
                value = value,
            };
        }
    }
}
