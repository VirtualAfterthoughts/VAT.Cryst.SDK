using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIElement
    {
        private string _displayName = string.Empty;

        private Action _onPressed = null;

        public virtual string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public virtual Action OnPressed
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
    }
}
