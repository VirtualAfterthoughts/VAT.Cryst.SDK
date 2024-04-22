using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPageRenderer : MonoBehaviour
    {
        [SerializeField]
        private UIPageElementRenderer[] _pageElements = new UIPageElementRenderer[0];

        [ContextMenu("Collect Page Elements")]
        public void CollectPageElements()
        {
            _pageElements = GetComponentsInChildren<UIPageElementRenderer>();
        }

        public void Render(UIPage page)
        {
            HideElements();

            for (var i = 0; i < _pageElements.Length && i < page.PageElements.Count; i++)
            {
                var element = _pageElements[i];
                element.gameObject.SetActive(true);

                element.Render(page.PageElements[i]);
            }
        }

        public void HideElements()
        {
            foreach (var element in _pageElements)
            {
                element.gameObject.SetActive(false);
            }
        }
    }
}
