using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    public abstract class CrystCharacter : MonoBehaviour, ICrystRigManager
    {
        [SerializeField]
        private CharacterVitals _vitals = null;

        public CrystRig[] Rigs { get { return _rigs; } }
        public int Count { get; private set; }

        private CrystRig[] _rigs;

        public ICrystVitals GetVitalsOrDefault()
        {
            return _vitals;
        }

        // Events
        private void Awake()
        {
            OnCharacterAwake();

            RegisterRigs(GetComponentsInChildren<CrystRig>());

            for (var i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].OnAwake();
            }
        }

        private void Start()
        {
            OnCharacterStart();

            for (var i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].OnStart();
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            OnCharacterEarlyUpdate();

            for (var i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].OnUpdate(deltaTime);
            }

            OnCharacterUpdate();
        }

        private void FixedUpdate()
        {
            float fixedDelta = Time.fixedDeltaTime;

            OnCharacterEarlyFixedUpdate();

            for (var i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].OnFixedUpdate(fixedDelta);
            }

            OnCharacterFixedUpdate();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            OnCharacterEarlyLateUpdate();

            for (var i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].OnLateUpdate(deltaTime);
            }

            OnCharacterLateUpdate();
        }

        // Rig registration
        public void RegisterRigs(params CrystRig[] rigs)
        {
            _rigs = new CrystRig[rigs.Length];

            for (var i = 0; i < rigs.Length; i++)
            {
                Internal_RegisterRig(rigs[i], i);
            }

            for (var i = 0; i < rigs.Length; i++)
            {
                var rig = rigs[i];

                int lastIndex = i - 1;

                if (lastIndex < 0)
                    lastIndex = rigs.Length - 1;

                rig.LastRig = rigs[lastIndex];
            }

            Count = rigs.Length;
        }

        public void UnregisterRigs()
        {
            if (Count <= 0)
                return;

            for (var i = 0; i < _rigs.Length; i++)
            {
                Internal_UnregisterRig(i);
            }

            _rigs = null;
            Count = 0;
        }

        private void Internal_RegisterRig(CrystRig rig, int i)
        {
            _rigs[i] = rig;
            rig.RigIndex = i;

            rig.OnRegisterManager(this);
        }

        private void Internal_UnregisterRig(int i)
        {
            var rig = _rigs[i];
            _rigs[i] = null;

            rig.RigIndex = -1;
            rig.LastRig = null;

            rig.OnDeregisterManager(this);
        }

        // Virtual methods
        protected virtual void OnCharacterAwake() { }
        protected virtual void OnCharacterStart() { }
        protected virtual void OnCharacterEarlyUpdate() { }
        protected virtual void OnCharacterUpdate() { }
        protected virtual void OnCharacterEarlyFixedUpdate() { }
        protected virtual void OnCharacterFixedUpdate() { }
        protected virtual void OnCharacterEarlyLateUpdate() { }
        protected virtual void OnCharacterLateUpdate() { }
    }
}
