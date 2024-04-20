using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VAT.Input.UI
{
    public class XRUIInputModule : StandaloneInputModule
    {
        private List<IXRUIInteractor> _interactors = new();
        private Dictionary<IXRUIInteractor, PointerEventData> _eventDataCache = new();

        public void RegisterInteractor(IXRUIInteractor interactor)
        {
            _interactors.Add(interactor);
            _eventDataCache[interactor] = new PointerEventData(eventSystem);
        }

        public void DeregisterInteractor(IXRUIInteractor interactor) 
        { 
            _interactors.Remove(interactor);
            _eventDataCache.Remove(interactor);
        }

        private void ProcessInteractor(IXRUIInteractor interactor)
        {
            var data = _eventDataCache[interactor];

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(interactor.GetEndPosition());

            data.button = PointerEventData.InputButton.Left;
            data.delta = screenPoint - data.position;
            data.position = screenPoint;

            bool pressed = interactor.IsPressed();
            var pressState = pressed ? PointerEventData.FramePressState.Pressed : PointerEventData.FramePressState.Released;

            var raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(data, raycastResults);

            if (raycastResults.Count > 0)
            {
                var result = raycastResults[0];
                result.screenPosition = Vector2.zero;
                data.pointerCurrentRaycast = result;
            }
            else
            {
                data.pointerCurrentRaycast = default;
            }

            data.pointerPress = data.pointerPressRaycast.gameObject;
            data.pointerDrag = data.pointerPressRaycast.gameObject;

            var state = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;

            if (state == CursorLockMode.Locked)
            {
                Cursor.visible = false;
            }

            ProcessMousePress(new MouseButtonEventData()
            {
                buttonState = pressState,
                buttonData = data,
            });
            ProcessMove(data);
            ProcessDrag(data);
        }

        public override void Process()
        {
            var state = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;

            if (state == CursorLockMode.Locked)
            {
                Cursor.visible = false;
            }

            foreach (var interactor in _interactors)
            {
                ProcessInteractor(interactor);
            }

            Cursor.lockState = state;
        }
    }
}
