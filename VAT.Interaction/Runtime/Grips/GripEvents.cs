using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace VAT.Interaction
{
    public sealed class GripEvents : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Grip[] _grips = new Grip[0];

        [Header("Events")]
        [SerializeField]
        private UnityEvent onAttached;

        [SerializeField]
        private UnityEvent onDetached;

        private void OnEnable()
        {
            foreach (var grip  in _grips)
            {
                grip.AttachCompleteEvent += OnAttachComplete;
                grip.DetachCompleteEvent += OnDetachComplete;
            }
        }

        private void OnDisable()
        {
            foreach (var grip in _grips)
            {
                grip.AttachCompleteEvent -= OnAttachComplete;
                grip.DetachCompleteEvent -= OnDetachComplete;
            }
        }

        private void OnAttachComplete(IInteractor interactor)
        {
            onAttached?.Invoke();
        }

        private void OnDetachComplete(IInteractor interactor)
        {
            onDetached?.Invoke();
        }
    }
}
