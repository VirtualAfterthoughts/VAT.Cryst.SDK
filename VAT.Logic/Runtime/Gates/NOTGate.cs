using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class NOTGate : MonoBehaviour, IReceiverNode, IDonorNode
    {
        [SerializeField]
        private List<Port> _inputs = new();

        [SerializeField]
        private List<Port> _outputs = new();

        private Signal _processedSignal = Signal.Identity;

        public List<Port> Inputs => _inputs;

        public List<Port> Outputs => _outputs;

        public Signal ProcessedSignal => _processedSignal;

        private void Awake()
        {
            LogicManager.UpdateManager.RegisterNode(this);
        }

        private void OnDestroy()
        {
            LogicManager.UpdateManager.UnregisterNode(this);
        }

        public void OnLogicUpdate(float deltaTime)
        {
            // If we have no inputs, we have no signal
            // Since this is a NOT gate, we can interpret it as a value of 1
            if (Inputs.Count <= 0)
            {
                _processedSignal = new Signal()
                {
                    value = 1f,
                };
                return;
            }

            float averageValue = 0f;

            foreach (var input in Inputs)
            {
                averageValue += input.ReceivedSignal.value;
            }

            averageValue /= Inputs.Count;

            float value = 1f - Mathf.Abs(averageValue);

            _processedSignal = new Signal()
            {
                value = value,
            };
        }
    }
}
