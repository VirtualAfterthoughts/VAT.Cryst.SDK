using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.UI
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField]
        private UIPanel[] _panels = new UIPanel[0];

        private UIPanel _defaultPanel = null;

        private void Awake()
        {
            foreach (var panel in _panels)
            {
                panel.controller = this;
            }

            _defaultPanel = _panels[0];

            HidePanels();
        }

        public void SwitchDefaultPanel(UIPanel panel)
        {
            _defaultPanel = panel;
        }

        public void ResetDefaultPanel()
        {
            _defaultPanel = _panels[0];
        }

        public void Show()
        {
            SwitchPanel(_defaultPanel);
        }

        public void SwitchPanel(UIPanel panel)
        {
            HidePanels();

            panel.Show();
        }

        public void HidePanels()
        {
            foreach (var panel in _panels)
            {
                panel.Hide();
            }
        }
    }
}
