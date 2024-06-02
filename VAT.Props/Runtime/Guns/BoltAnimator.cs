using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Props
{
    public class BoltAnimator : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private GunBolt _bolt = null;

        [SerializeField]
        private string _percentParameter = "BoltPercent";

        private void Update()
        {
            _animator.SetFloat(_percentParameter, Mathf.Clamp(_bolt.PulledPercent, 0f, 0.99f));
        }
    }
}
