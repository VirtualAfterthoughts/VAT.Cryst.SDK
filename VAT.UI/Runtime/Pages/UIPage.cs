using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPage
    {
        private List<UIElement> _pageElements = new();

        public List<UIElement> PageElements => _pageElements;

        public void AddElement(UIElement element)
        {
            _pageElements.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            _pageElements.Remove(element);
        }
    }
}
