using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Cryst.Utilities
{
    public class CollisionEnterReceiver : MonoBehaviour
    {
        public event Action<Collision> OnEnter, OnExit;

        private void OnCollisionEnter(Collision collision)
        {
            OnEnter?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnExit?.Invoke(collision);
        }
    }
}
