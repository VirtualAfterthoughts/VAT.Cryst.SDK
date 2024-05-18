using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class Port : MonoBehaviour
    {
        private Signal _currentSignal = default;
        private int _signalCounter = 0;

        public Signal CurrentSignal => _currentSignal;

        public void ReceiveSignal(Signal signal)
        {
            _currentSignal = signal;
            _signalCounter = 1;
        }

        private void LateUpdate()
        {
            if (_signalCounter > 0)
            {
                _signalCounter--;
            }
            else
            {
                _currentSignal = default;
            }
        }
    }
}
