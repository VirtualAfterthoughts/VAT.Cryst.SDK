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

        public void Render(UIPage page)
        {
            HideElements();

            for (var i = 0; i < _elements.Length && i < page.PageElements.Count; i++)
            {
                var element = _elements[i];
                element.gameObject.SetActive(true);

                element.Render(page.PageElements[i]);
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
