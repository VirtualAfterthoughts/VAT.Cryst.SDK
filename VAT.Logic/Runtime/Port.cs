using UnityEngine;

namespace VAT.Logic
{
    public sealed class Port : MonoBehaviour, INode
    {
        private Signal _receivedSignal = default;
        private int _signalCounter = 0;

        public Signal ReceivedSignal => _receivedSignal;

        public void ReceiveSignal(Signal signal)
        {
            _receivedSignal = signal;
            _signalCounter = 1;
        }

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
            if (_signalCounter > 0)
            {
                _signalCounter--;
            }
            else
            {
                _receivedSignal = default;
            }
        }
    }
}
