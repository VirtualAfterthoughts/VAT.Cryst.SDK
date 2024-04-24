using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace VAT.UI
{
    public class UIElementRenderer : MonoBehaviour
    {
        public Button button;

        public TMP_Text text;

        public void Awake()
        {
            button = GetComponentInChildren<Button>();
            text = GetComponentInChildren<TMP_Text>();
        }

        public void Render(UIElement element)
        {
            text.text = element.Text;

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(element.Press);
        }
    }
}
