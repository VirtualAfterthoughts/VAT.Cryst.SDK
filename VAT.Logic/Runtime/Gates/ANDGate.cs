using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class ANDGate : MonoBehaviour, IReceiverNode, IDonorNode
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
            float minimum = 0f;
            bool hasInput = false;

            foreach (var input in Inputs)
            {
                if (!hasInput)
                {
                    minimum = input.ReceivedSignal.value;
                    hasInput = true;
                    continue;
                }

                minimum = Mathf.Min(minimum, input.ReceivedSignal.value);
            }

            _processedSignal = new Signal()
            {
                value = minimum
            };
        }
    }
}
