using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public abstract class UIElement
    {
        private UIElement _parent = null;

        private List<UIElement> _children = null;

        private string _text = string.Empty;

        public bool HasChildren => _children != null && _children.Count > 0;

        public List<UIElement> Children => _children;

        public UIElement Parent => _parent;

        public virtual string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public abstract void Press();

        public void AddChild(UIElement element)
        {
            _children ??= new List<UIElement>();

            _children.Add(element);
            element._parent = this;
        }

        public void RemoveChild(UIElement element)
        {
            _children.Remove(element);
            element._parent = null;
        }
    }
}
