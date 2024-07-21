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
            float pulledPercent = _bolt.PulledPercent;

            if (_bolt.State == BoltState.OPENING)
            {
                _animator.SetBool("Returning", false);
            }
            else if (_bolt.State == BoltState.CLOSING)
            {
                _animator.SetBool("Returning", true);
            }

            if (_animator.GetBool("Returning"))
            {
                pulledPercent = 1f - pulledPercent;
            }

            _animator.SetFloat(_percentParameter, Mathf.Clamp(pulledPercent, 0f, 0.99f));
        }
    }
}
