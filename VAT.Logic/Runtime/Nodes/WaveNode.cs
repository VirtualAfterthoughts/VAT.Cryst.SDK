using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class WaveNode : MonoBehaviour, IDonorNode
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

        public Signal ProcessedSignal
        {
            get
            {
                return new Signal()
                {
                    value = _value
                };
            }
        }

        public List<Port> Outputs => _outputs;

        private float _value = 0f;

        private void OnEnable()
        {
            LogicManager.UpdateManager.RegisterNode(this);
        }

        private void OnDisable()
        {
            LogicManager.UpdateManager.UnregisterNode(this);
        }

        public void OnLogicUpdate(float deltaTime)
        {
            float time = Time.time;

            _value = Mathf.Sin(time * _frequency + _phase) * _amplitude;
        }
    }
}
