using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Interaction
{
    public partial class Interactable : MonoBehaviour, IInteractable
    {
        public event Action<IHoverer> OnHoverBeginEvent;
        public event Action<IHoverer> OnHoverEndEvent;

        private readonly List<IHoverer> _hoveringHoverers = new();

        public IReadOnlyList<IHoverer> HoveringHoverers => _hoveringHoverers;

        public virtual bool CanHover(IHoverer hoverer)
        {
            return true;
        }

        public virtual void OnHoverBegin(IHoverer hoverer)
        {
            _hoveringHoverers.Add(hoverer);

            OnHoverBeginEvent?.Invoke(hoverer);
        }

        public virtual void OnHoverEnd(IHoverer hoverer)
        {
            _hoveringHoverers.Remove(hoverer);

            OnHoverEndEvent?.Invoke(hoverer);
        }
    }
}
