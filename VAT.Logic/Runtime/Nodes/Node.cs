using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public abstract class Node : MonoBehaviour
    {
        public virtual List<Port> Receivers { get; } = null;

        public virtual List<Port> Outputs { get; } = null;

        public bool CanReceive()
        {
            return Receivers != null;
        }

        public bool HasReceiver(Port port)
        {
            return CanReceive() && Receivers.Contains(port);
        }

        public void AddReceiver(Port port)
        {
            Receivers.Add(port);
        }

        public void RemoveReceiver(Port port)
        {
            Receivers.Remove(port);
        }

        public bool CanOutput()
        {
            return Outputs != null;
        }

        public bool HasOutput(Port port)
        {
            return CanOutput() && Outputs.Contains(port);
        }

        public void AddOutput(Port port)
        {
            Outputs.Add(port);
        }

        public void RemoveOutput(Port port)
        {
            Outputs.Remove(port);
        }

        public virtual Signal GetSignal()
        {
            return Signal.Identity;
        }

        private void OnEnable()
        {
            ValidatePorts();
        }

        private void ValidatePorts()
        {
            if (CanReceive())
            {
                Receivers.RemoveAll((p) => p == null);
            }

            if (CanOutput())
            {
                Outputs.RemoveAll((p) => p == null);
            }
        }

        private void Update()
        {
            OnNodeUpdate();

            ProcessOutputs();
        }

        protected virtual void OnNodeUpdate() { }

        private void ProcessOutputs()
        {
            if (Outputs == null)
            {
                return;
            }

            var signal = GetSignal();

            foreach (var output in Outputs)
            {
                output.ReceiveSignal(signal);
            }
        }
    }
}
