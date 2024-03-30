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
                grip.OnAttached += OnAttached;
                grip.OnDetached += OnDetached;
            }
        }

        private void OnDisable()
        {
            foreach (var grip in _grips)
            {
                grip.OnAttached -= OnAttached;
                grip.OnDetached -= OnDetached;
            }
        }

        private void OnAttached(IInteractor interactor)
        {
            onAttached?.Invoke();
        }

        private void OnDetached(IInteractor interactor)
        {
            onDetached?.Invoke();
        }
    }
}
