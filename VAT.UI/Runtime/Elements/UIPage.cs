using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPage : UIElement
    {
        public event Action<UIPage> OnSelectPage;

        public new UIPage Parent => base.Parent as UIPage;

        public override void Press()
        {
            OnSelectPage?.Invoke(this);
        }
    }
}
