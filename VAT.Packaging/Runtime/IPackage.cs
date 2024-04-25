using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Packaging
{
    public interface ICrystal : IShippable
    {
        /// <summary>
        /// Whether or not this is a crystal built with the game itself.
        /// </summary>
        public bool IsInternal { get; }
    }
}
