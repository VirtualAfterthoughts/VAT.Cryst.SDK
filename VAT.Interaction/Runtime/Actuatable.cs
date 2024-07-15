using UnityEngine;

namespace VAT.Interaction
{
    public abstract class Actuatable : MonoBehaviour
    {
        private bool _isActuated = false;
        public bool IsActuated => _isActuated;

        public void Actuate(bool isActuated = true)
        {
            // If this is already the same state, no need to reinvoke events
            if (this._isActuated == isActuated)
            {
                return;
            }

            this._isActuated = isActuated;

            OnActuated(isActuated);
        }

        protected abstract void OnActuated(bool isActuated);
    }
}
