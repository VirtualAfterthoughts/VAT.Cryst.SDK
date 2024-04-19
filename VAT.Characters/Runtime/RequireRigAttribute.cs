using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters
{
    /// <summary>
    /// The RequireRig attribute can be placed on a CrystRig to warn in editor when the RigManager needs a specific Rig type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireRig : Attribute
    {
        public Type requiredRig;

        public RequireRig(Type requiredRig)
        {
            this.requiredRig = requiredRig;
        }
    }
}
