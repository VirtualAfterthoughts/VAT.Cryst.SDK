using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Cryst.Delegates
{
    /// <summary>
    /// A delegate that takes in a target, processes it, and returns the new target.
    /// </summary>
    /// <param name="target">The input target.</param>
    /// <returns>The processed target.</returns>
    public delegate SimpleTransform TargetProcessorCallback(in SimpleTransform target);
}
