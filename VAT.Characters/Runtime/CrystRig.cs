using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Input;

namespace VAT.Characters
{
    public abstract class CrystRig : MonoBehaviour, ICrystRig
    {
        public int RigIndex { get; set; } = -1;

        public CrystRig LastRig { get; set; } = null;

        private ICrystRigManager _rigManager = null;
        public ICrystRigManager RigManager => _rigManager;

        public void OnRegisterRig(ICrystRigManager rigManager)
        {
            _rigManager = rigManager;

            _rigManager.OnManagerUpdate += OnUpdate;
            _rigManager.OnManagerFixedUpdate += OnFixedUpdate;
            _rigManager.OnManagerLateUpdate += OnLateUpdate;
        }

        public void OnDeregisterRig(ICrystRigManager rigManager)
        {
            _rigManager.OnManagerUpdate -= OnUpdate;
            _rigManager.OnManagerFixedUpdate -= OnFixedUpdate;
            _rigManager.OnManagerLateUpdate -= OnLateUpdate;

            _rigManager = null;
        }

        public virtual void OnRigEnable()
        { 
        }

        public virtual void OnRigDisable() 
        {
        }

        public virtual void OnFixedUpdate(float deltaTime) { }
        public virtual void OnLateUpdate(float deltaTime) { }
        public virtual void OnUpdate(float deltaTime) { }

        public virtual bool TryGetHead(out IJoint head)
        {
            head = default;
            return false;
        }

        public virtual bool TryGetArm(Handedness handedness, out IArm arm)
        {
            arm = default;
            return false;
        }

        public virtual bool TryGetTrackedRig(out CrystRig rig)
        {
            if (RigIndex <= 0)
            {
                rig = null;
                return false;
            }

            return LastRig.TryGetTrackedRig(out rig);
        }

        public virtual bool TryGetInput(out IBasicInput input)
        {
            if (RigIndex <= 0)
            {
                input = null;
                return false;
            }

            return LastRig.TryGetInput(out input);
        }
    }
}
