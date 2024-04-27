using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VAT.UI
{
    public class UISelectBorder : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private Graphic _borderGraphic;

        private void OnEnable()
        {
            bool isSelected = false;

            if (EventSystem.current != null)
            {
                isSelected = EventSystem.current.currentSelectedGameObject == gameObject;
            }

            SetVisibility(isSelected, 0f);
        }

        private void SetVisibility(bool visible, float duration = 0.1f)
        {
            if (visible)
            {
                _borderGraphic.CrossFadeColor(Color.white, duration, true, true);
            }
            else
            {
                _borderGraphic.CrossFadeColor(Color.clear, duration, true, true);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetVisibility(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetVisibility(true);
        }
    }
}
