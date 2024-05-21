using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Logic
{
    public class ActiveNode : Node
    {
        [SerializeField]
        private Port _receiver;

        [SerializeField]
        [Range(-1f, 1f)]
        private float _minimumSignal = 1f;

        [SerializeField]
        private GameObject _targetObject = null;

        public override List<Port> Receivers => new()
        {
            _receiver,
        };

        public override List<Port> Outputs => null;

        public void Update()
        {
            bool active = _receiver.CurrentSignal.value >= _minimumSignal;

            _targetObject.SetActive(active);
        }
    }
}
