using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Entities.PhysX
{
    /// <summary>
    /// A data structure containing a snapshot of a Configurable Joint.
    /// </summary>
    [Serializable]
    public struct SimpleConfigurableJoint
    {
        public static readonly SimpleConfigurableJoint Default = new() { };

        public static SimpleConfigurableJoint Create(ConfigurableJoint joint) {
            return new() {

            };
        }

        public void Apply(ConfigurableJoint joint) {

        }
    }
}
