using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Cryst.Interfaces;

using VAT.Input.Skeleton;
using VAT.Input;

namespace VAT.Characters
{
    public interface ICrystRig
    {
        ICrystRigManager RigManager { get; }

        void OnRegister(ICrystRigManager rigManager);

        void OnDeregister(ICrystRigManager rigManager);

        void OnRigEnable();

        void OnRigDisable();

        bool TryGetArm(Handedness handedness, out IArm result);
    }
}
