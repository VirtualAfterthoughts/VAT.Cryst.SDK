using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace VAT.UI
{
    public class UIPageElementRenderer : MonoBehaviour
    {
        public Button button;

        public TMP_Text text;

        public void Awake()
        {
            button = GetComponentInChildren<Button>();
            text = GetComponentInChildren<TMP_Text>();
        }

        public void Render(UIPageElement element)
        {
            text.text = element.DisplayName;

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() => 
            {
                element.OnPressed?.Invoke();
            });
        }
    }
}
