using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using UnityEngine.Events;

namespace VAT.Cryst
{
    public class RepeatedEvent : MonoBehaviour
    {
        [Tooltip("The amount in seconds between each invocation of the event.")]
        [Min(1e-02f)]
        [SerializeField]
        private float _repeatRate;

        [Tooltip("The event to be called.")]
        [SerializeField]
        private UnityEvent _unityEvent;

        [Tooltip("Is the event disabled?")]
        [SerializeField]
        private bool _isDisabled = false;

        private float _timer;

        public bool IsDisabled 
        { 
            get 
            { 
                return _isDisabled; 
            } 
            set 
            { 
                _isDisabled = value; 
            } 
        }

        public float RepeatRate 
        { 
            get 
            { 
                return _repeatRate; 
            } set 
            { 
                _repeatRate = Mathf.Max(value, 1e-02f); 
            } 
        }

        public void Update()
        {
            if (IsDisabled)
                return;

            _timer += Time.deltaTime;

            while (_timer >= _repeatRate)
            {
                _timer -= _repeatRate;
                _unityEvent.Invoke();
            }
        }

        public void Toggle()
        {
            _isDisabled = !_isDisabled;
        }
    }
}
