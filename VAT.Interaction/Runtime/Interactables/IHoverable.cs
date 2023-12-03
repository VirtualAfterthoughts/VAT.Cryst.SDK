using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public interface IHoverable
    {
        /// <summary>
        /// Invoked when a Hoverer starts hovering on this object.
        /// </summary>
        event Action<IHoverer> OnHoverBeginEvent;

        /// <summary>
        /// Invoked when a Hoverer stops hovering on this object.
        /// </summary>
        event Action<IHoverer> OnHoverEndEvent;

        /// <summary>
        /// Returns if this Hoverable can be hovered by this Hoverer.
        /// </summary>
        /// <param name="hoverer"></param>
        /// <returns></returns>
        public bool CanHover(IHoverer hoverer);

        void OnHoverBegin(IHoverer hoverer);

        void OnHoverEnd(IHoverer hoverer);
    }
}
