using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Data;
using VAT.Shared.Utilities;

namespace VAT.Interaction
{
    /// <summary>
    /// The interface to be extended by any interactable object.
    /// <para>
    /// The job of an interactable is to define how an object reacts to different inputs.
    /// For example, a door should open and close when interacted with.
    /// In the case of gripping an object, interactables automatically implement the IGrippable interface.
    /// At the very least, the grip should implement getting the target as well as creating the grip joint.
    /// </para>
    /// </summary>
    public interface IInteractable : IGrippable, IHoverable
    {
        public static ComponentCache<IInteractable> Cache = new();
    }
}
