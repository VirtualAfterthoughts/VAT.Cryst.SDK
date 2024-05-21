using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class WaveNode : Node
    {
        [SerializeField]
        [Min(0f)]
        private float _amplitude = 1f;

        [SerializeField]
        private float _frequency = 1f;

        [SerializeField]
        private float _phase = 1f;

        [SerializeField]
        private List<Port> _outputs = new();

        public override List<Port> Receivers => null;

        public override List<Port> Outputs => _outputs;

        private float _value = 0f;

        protected override void OnNodeUpdate()
        {
            float time = Time.time;

            _value = Mathf.Sin(time * _frequency + _phase) * _amplitude;
        }

        public override Signal GetOutputSignal()
        {
            return new Signal()
            {
                value = _value
            };
        }
    }
}
