using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace VAT.Interaction
{
    public sealed class InteractableEvents : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onHoverBegin;

        [SerializeField]
        private UnityEvent _onHoverEnd;

        [SerializeField]
        private Interactable[] _interactables;

        private void Awake()
        {
            foreach (var interactable in _interactables)
            {
                interactable.OnHoverBeginEvent += OnHoverBegin;
                interactable.OnHoverEndEvent += OnHoverEnd;
            }
        }

        private void OnDestroy()
        {
            foreach (var interactable in _interactables)
            {
                interactable.OnHoverBeginEvent -= OnHoverBegin;
                interactable.OnHoverEndEvent -= OnHoverEnd;
            }
        }

        private void OnHoverBegin(IHoverer hoverer)
        {
            _onHoverBegin.Invoke();
        }

        private void OnHoverEnd(IHoverer hoverer)
        {
            _onHoverEnd.Invoke();
        }
    }
}
