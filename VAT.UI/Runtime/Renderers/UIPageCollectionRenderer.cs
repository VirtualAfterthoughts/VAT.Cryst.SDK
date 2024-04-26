using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.UI
{
    public class UIPageCollectionRenderer : MonoBehaviour
    {
        [SerializeField]
        private UIPageRenderer _pageRenderer = null;

        private List<UIPage> _pages = null;
        private int _currentPage = 0;

        public int CurrentPage => _currentPage + 1;
        public int PageCount => _pages != null ? _pages.Count : 0;

        public event Action<UIPage> OnPageChanged;

        public void Render(UIPageCollection collection)
        {
            _pages = collection.Pages;
            _currentPage = 0;

            OnRender();
        }

        private void OnRender(UIPage page)
        {
            _pageRenderer.Render(page);

            OnPageChanged?.Invoke(page);
        }

        private void OnRender()
        {
            if (_pages != null)
            {
                OnRender(_pages[_currentPage]);
            }
        }

        public void NextPage()
        {
            if (_pages == null)
            {
                return;
            }

            if (_currentPage < _pages.Count - 1)
            {
                _currentPage++;
            }

            OnRender();
        }

        public void PreviousPage()
        {
            if (_pages == null)
            {
                return;
            }

            if (_currentPage > 0)
            {
                _currentPage--;
            }

            OnRender();
        }
    }
}
