using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIButton : UIElement
    {
        private Action _onPressed = null;

        public Action OnPressed
        {
            get
            {
                return _onPressed;
            }
            set
            {
                _onPressed = value;
            }
        }

        public override void Press()
        {
            _onPressed?.Invoke();
        }
    }
}
