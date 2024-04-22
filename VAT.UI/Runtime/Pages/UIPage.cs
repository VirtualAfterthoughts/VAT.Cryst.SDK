using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPage
    {
        private List<UIPageElement> _pageElements = new();

        public List<UIPageElement> PageElements => _pageElements;

        public void AddElement(UIPageElement element)
        {
            _pageElements.Add(element);
        }

        public void RemoveElement(UIPageElement element)
        {
            _pageElements.Remove(element);
        }
    }
}
