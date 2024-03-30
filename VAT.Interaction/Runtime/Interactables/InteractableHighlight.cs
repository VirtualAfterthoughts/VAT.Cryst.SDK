using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Interaction
{
    public class InteractableHighlight : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject _highlight = null;

        private IInteractable _interactable = null;

        private int _hoverCount = 0;

        private void Awake()
        {
            if (!TryGetComponent(out _interactable))
            {
                Debug.LogError($"InteractableHighlight {name} is not on an Interactable!", this);
                enabled = false;
                return;
            }

            if (_highlight == null)
            {
                Debug.LogError($"InteractableHighlight {name} is missing a highlight object!", this);
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            _interactable.OnHoverBegin += OnHoverBegin;
            _interactable.OnHoverEnd += OnHoverEnd;

            _highlight.SetActive(false);

            _hoverCount = 0;
        }

        private void OnDisable()
        {
            _interactable.OnHoverBegin -= OnHoverBegin;
            _interactable.OnHoverEnd -= OnHoverEnd;

            _highlight.SetActive(false);

            _hoverCount = 0;
        }

        private void OnHoverBegin(IInteractor interactor)
        {
            _highlight.SetActive(true);

            _hoverCount++;
        }

        private void OnHoverEnd(IInteractor interactor)
        {
            _hoverCount--;

            if (_hoverCount <= 0)
            {
                _highlight.SetActive(false);
            }
        }
    }
}
