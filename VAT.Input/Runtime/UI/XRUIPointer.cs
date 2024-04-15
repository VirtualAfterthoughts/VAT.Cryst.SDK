using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace VAT.Input.UI
{
    public class XRUIPointer : MonoBehaviour
    {
        public Transform start;
        public Transform middle;
        public Transform end;

        private Vector3 _lastStartPos = Vector3.zero;
        private Vector3 _lastMidPos = Vector3.zero;
        private Vector3 _lastEndPos = Vector3.zero;

        public void LateUpdate()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + transform.forward * 10f;

            end.rotation = transform.rotation;

            if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 10f, ~0, QueryTriggerInteraction.Collide))
            {
                endPos = hitInfo.point;

                end.rotation = Quaternion.LookRotation(hitInfo.normal, transform.up);

                if (EventSystem.current.currentInputModule is XRUIInputModule module)
                {
                    module.position = transform.position;
                    module.forward = transform.forward;
                    module.end = end.position;
                }
            }

            Vector3 middlePos = (startPos + endPos) / 2f;

            start.position = Vector3.Slerp(_lastStartPos, startPos, Time.deltaTime * 32f);
            _lastStartPos = start.position;

            middle.position = Vector3.Slerp(_lastMidPos, middlePos, Time.deltaTime * 24f);
            _lastMidPos = middle.position;

            end.position = Vector3.Slerp(_lastEndPos, endPos, Time.deltaTime * 12f);
            _lastEndPos = end.position;
        }
    }
}
