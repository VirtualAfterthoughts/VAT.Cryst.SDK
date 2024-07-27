using System.Collections.Generic;

using VAT.Cryst.Interfaces;

namespace VAT.Logic
{
    public class LogicUpdateManager : IUpdateable
    {
        private readonly HashSet<INode> _registeredNodes = new();

        public HashSet<INode> RegisteredNodes => _registeredNodes;

        public void RegisterNode(INode node)
        {
            _registeredNodes.Add(node);
        }

        public void UnregisterNode(INode node)
        {
            _registeredNodes.Remove(node);
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var node in RegisteredNodes)
            {
                OnNodeUpdate(node, deltaTime);
            }
        }

        private void OnNodeUpdate(INode node, float deltaTime)
        {
            node.OnLogicUpdate(deltaTime);

            if (node is IDonorNode donor)
            {
                OnDonorUpdate(donor, deltaTime);
            }

            if (node is IReceiverNode receiver)
            {
                OnReceiverUpdate(receiver, deltaTime);
            }
        }

        private void OnDonorUpdate(IDonorNode donor, float deltaTime)
        {
            var outputs = donor.Outputs;

            if (outputs == null)
            {
                return;
            }

            var signal = donor.ProcessedSignal;

            foreach (var output in outputs) 
            {
                output.ReceiveSignal(signal);
            }
        }

        private void OnReceiverUpdate(IReceiverNode receiver, float deltaTime)
        {
            // Nothing yet
        }
    }
}
