using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VAT.UI
{
    public class UIPageNumber : MonoBehaviour
    {
        [SerializeField]
        private UIPageCollectionRenderer _pageCollection = null;

        [SerializeField]
        private TMP_Text _pageNumberText = null;

        private void OnEnable()
        {
            _pageCollection.OnPageChanged += OnPageChanged;

            if (_pageCollection != null)
            {
                SetText(_pageCollection.CurrentPage, _pageCollection.PageCount);
            }
        }

        private void OnDisable()
        {
            _pageCollection.OnPageChanged -= OnPageChanged;

            SetText(0, 0);
        }

        private void OnPageChanged(UIPage page)
        {
            SetText(_pageCollection.CurrentPage, _pageCollection.PageCount);
        }

        private void SetText(int pageNumber, int pageCount)
        {
            if (_pageNumberText != null)
            {
                _pageNumberText.text = $"{pageNumber} / {pageCount}";
            }
        }
    }
}
