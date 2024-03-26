using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;
using VAT.Cryst.Interfaces;
using VAT.Input;

namespace VAT.Characters
{
    public interface ICrystRig : IUpdateable, IFixedUpdateable, ILateUpdateable
    {
        void OnRegisterManager(ICrystRigManager rigManager);

        void OnDeregisterManager(ICrystRigManager rigManager);

        bool TryGetArm(Handedness handedness, out IArm result);
    }
}
