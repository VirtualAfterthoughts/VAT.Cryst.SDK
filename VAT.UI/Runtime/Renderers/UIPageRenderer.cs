using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPageRenderer : MonoBehaviour
    {
        [SerializeField]
        private UIElementRenderer[] _elements = new UIElementRenderer[0];

        [ContextMenu("Collect Elements")]
        public void CollectElements()
        {
            _elements = GetComponentsInChildren<UIElementRenderer>();
        }

        private UIPage _currentPage = null;

        public void Render(UIPage page)
        {
            _currentPage = page;

            HideElements();

            for (var i = 0; i < _elements.Length && i < page.Children.Count; i++)
            {
                var element = _elements[i];
                element.gameObject.SetActive(true);

                element.Render(page.Children[i]);

                if (page.Children[i] is UIPage pageElement)
                {
                    pageElement.OnSelectPage += OnSelectPage;
                }
            }
        }

        private void OnSelectPage(UIPage page)
        {
            Render(page);
        }

        public void ClimbTree()
        {
            if (_currentPage != null && _currentPage.Parent != null)
            {
                Render(_currentPage.Parent);
            }
        }

        public void HideElements()
        {
            foreach (var element in _elements)
            {
                element.gameObject.SetActive(false);
            }
        }
    }
}
