using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPanel : MonoBehaviour
    {
        public UIPanelController controller;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
