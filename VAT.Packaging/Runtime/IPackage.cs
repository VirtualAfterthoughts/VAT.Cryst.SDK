using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Packaging
{
    public interface IPackage : IShippable
    {
        /// <summary>
        /// Whether or not this is a package built with the game itself.
        /// </summary>
        public bool IsInternal { get; }
    }
}
