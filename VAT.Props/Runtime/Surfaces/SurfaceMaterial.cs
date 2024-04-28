using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Props
{
    [CreateAssetMenu(menuName = "Cryst/Props/Surface Material")]
    public class SurfaceMaterial : ScriptableObject
    {
        [SerializeField]
        [Min(0f)]
        [Tooltip("The density of the material.")]
        private float _density = 1.0f;

        public float Density
        {
            get
            {
                return _density;
            }
            set
            {
                _density = value;
            }
        }
    }
}
