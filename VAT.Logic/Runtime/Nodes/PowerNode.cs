using System.Collections.Generic;

using UnityEngine;

namespace VAT.Logic
{
    public class PowerNode : MonoBehaviour, IDonorNode
    {
        [SerializeField]
        [Range(-1f, 1f)]
        private float _value = 1f;

        [SerializeField]
        private List<Port> _outputs = new();

        public List<Port> Outputs => _outputs;

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
        }
    }
}
