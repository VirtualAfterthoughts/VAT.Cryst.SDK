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
            float openedPercent = _bolt.OpenedPercent;

            bool returning = _animator.GetBool("Returning");

            if (_bolt.State == BoltState.OPENING && returning)
            {
                _animator.SetBool("Returning", false);
                returning = false;
            }
            else if (_bolt.State == BoltState.CLOSING && !returning)
            {
                _animator.SetBool("Returning", true);
                returning = true;
            }

            if (returning)
            {
                openedPercent = 1f - openedPercent;
            }

            _animator.SetFloat(_percentParameter, Mathf.Clamp(openedPercent, 0f, 0.99f));
        }
    }
}
