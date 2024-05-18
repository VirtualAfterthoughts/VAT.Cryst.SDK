using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VAT.Logic
{
    public abstract class Node : MonoBehaviour
    {
        public virtual List<Port> Receivers { get; } = null;

        public virtual List<Port> Outputs { get; } = null;

        public virtual Signal GetOutputSignal()
        {
            return Signal.Identity;
        }

        private void Update()
        {
            ProcessOutputs();
        }

        private void ProcessOutputs()
        {
            if (Outputs == null)
            {
                return;
            }

            var signal = GetOutputSignal();

            foreach (var output in Outputs)
            {
                output.ReceiveSignal(signal);
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            OnDrawOutputs();
            OnDrawReceivers();
        }
        
        private void OnDrawOutputs()
        {
            if (Outputs == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            foreach (var output in Outputs)
            {
                if (output == null)
                {
                    continue;
                }

                Gizmos.DrawLine(transform.position, output.transform.position);
            }
        }

        private void OnDrawReceivers()
        {
            if (Receivers == null)
            {
                return;
            }

            Gizmos.color = Color.red;

            foreach (var receiver in Receivers)
            {
                if (receiver == null)
                {
                    continue;
                }

                Gizmos.DrawLine(transform.position, receiver.transform.position);
            }
        }
#endif
    }
}
