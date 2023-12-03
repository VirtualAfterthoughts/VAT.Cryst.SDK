using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Utilities;

namespace VAT.Interaction
{
    public interface IInteractor : IGrabber, IHoverer
    {
        public static ComponentCache<IInteractor> Cache = new();

        void AttachGrippable(IGrippable grippable);

        void DetachGrippable(IGrippable grippable);

        void DetachGrippables();
    }
}
