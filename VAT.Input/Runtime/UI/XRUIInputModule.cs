using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VAT.Input.UI
{
    public class XRUIInputModule : StandaloneInputModule
    {
        public Vector3 position;
        public Vector3 forward;
        public Vector3 end;

        private PointerEventData _pointerData;

        public override void Process()
        {
            _pointerData ??= new PointerEventData(eventSystem);

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(end);

            //_pointerData.selectedObject = hitInfo.collider.gameObject;
            _pointerData.button = PointerEventData.InputButton.Left;
            _pointerData.delta = screenPoint - _pointerData.position;
            _pointerData.position = screenPoint;

            bool pressed = UnityEngine.InputSystem.Mouse.current.press.ReadValue() > 0.5f;
            var pressState = pressed ? PointerEventData.FramePressState.Pressed : PointerEventData.FramePressState.Released;

            var raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(_pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                var result = raycastResults[0];
                result.screenPosition = Vector2.zero;
                _pointerData.pointerCurrentRaycast = result;
            }
            else
            {
                _pointerData.pointerCurrentRaycast = default;
            }

            _pointerData.pointerPress = _pointerData.pointerPressRaycast.gameObject;
            _pointerData.pointerDrag = _pointerData.pointerPressRaycast.gameObject;

            var state = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;

            if (state == CursorLockMode.Locked)
            {
                Cursor.visible = false;
            }

            ProcessMousePress(new MouseButtonEventData()
            {
                buttonState = pressState,
                buttonData = _pointerData,
            });
            ProcessMove(_pointerData);
            ProcessDrag(_pointerData);

            Cursor.lockState = state;
        }
    }
}
