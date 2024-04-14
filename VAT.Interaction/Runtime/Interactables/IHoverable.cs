using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    [Flags]
    public enum HoverFlags
    {
        NEAR = 1 << 0,
        FAR = 1 << 2,
    }

    public interface IHoverable
    {
        event InteractorDelegate OnHoverBegin, OnHoverEnd;

        void BeginHover(IInteractor interactor);

        void EndHover(IInteractor interactor);

        HoverFlags GetHoverFlags();
    }
}
