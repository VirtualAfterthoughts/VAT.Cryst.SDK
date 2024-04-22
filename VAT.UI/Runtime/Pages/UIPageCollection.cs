using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPageCollection
    {
        private List<UIPage> _pages = new();

        public List<UIPage> Pages => _pages;

        public void AddPage(UIPage page)
        {
            _pages.Add(page);
        }

        public void RemovePage(UIPage page)
        {
            _pages.Remove(page);
        }
    }
}
