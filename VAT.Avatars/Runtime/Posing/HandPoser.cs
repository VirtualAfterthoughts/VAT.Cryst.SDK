using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars.Posing {
    [ExecuteAlways]
    public abstract class HandPoser : MonoBehaviour {
        public bool Generated => _generated;

        [SerializeField]
        [HideInInspector]
        private bool _generated = false;

        public bool Initiated => _initiated;
        private bool _initiated = false;

        public void Initiate() {
            _generated = true;

            if (_initiated)
                Uninitiate();

            OnInitiate();
            _initiated = true;
        }

        public void Uninitiate() {
            if (_initiated) {
                OnUninitiate();
                _initiated = false;
            }
        }

        private void OnEnable() {
            if (!_generated)
                return;

            Initiate();
        }

        private void OnDisable() {
            if (!_generated)
                return;

            Uninitiate();
        }

        private void Update() {
            OnUpdate();
        }

        public abstract void WriteArtOffsets();

        protected virtual void OnInitiate() { }

        protected virtual void OnUninitiate() { }

        protected virtual void OnUpdate() { }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            DrawGizmos();
        }

        protected virtual void DrawGizmos() { }
#endif
    }
}
