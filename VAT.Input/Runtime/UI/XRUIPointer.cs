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

        private Vector3 _startPos = Vector3.zero;
        private Vector3 _middlePos = Vector3.zero;
        private Vector3 _endPos = Vector3.zero;

        private UIPlane _currentPlane = null;

        public void LateUpdate()
        {
            var colliders = Physics.OverlapSphere(transform.position, 0.01f);

            _currentPlane = null;
            float closestDot = float.NegativeInfinity;
            float closestDistance = float.PositiveInfinity;

            foreach (var collider in colliders)
            {
                if (UIPlane.Cache.TryGet(collider.gameObject, out var hber))
                {
                    var dot = Vector3.Dot(transform.forward, -hber.GetPlane().normal);
                    var distance = (transform.position - hber.GetCenter()).magnitude;

                    if (dot > closestDot || distance < closestDistance)
                    {
                        _currentPlane = hber;
                        closestDot = dot;
                        closestDistance = distance;
                    }
                }
            }

            if (_currentPlane == null)
            {
                LerpPositions();
                return;
            }

            var plane = _currentPlane.GetPlane();
            var normal = plane.normal;

            var extents = _currentPlane.GetSize() * 0.5f;
            var rotation = _currentPlane.GetRotation();

            var startPos = transform.position;
            var endPos = plane.ClosestPointOnPlane(startPos);

            if (plane.Raycast(new Ray(transform.position, transform.forward), out var enter))
            {
                endPos = transform.position + transform.forward * enter;
            }

            var prevEndPos = endPos;
            var inPlane = Quaternion.Inverse(rotation) * (endPos - _currentPlane.GetCenter());
            inPlane = Vector3.Min(Vector3.Max(inPlane, -extents), extents);

            endPos = (rotation * inPlane) + _currentPlane.GetCenter();

            if (endPos != prevEndPos)
            {
                LerpPositions();
                return;
            }

            end.rotation = Quaternion.LookRotation(-normal, transform.up);

            if (EventSystem.current.currentInputModule is XRUIInputModule module)
            {
                module.position = transform.position;
                module.forward = transform.forward;
                module.end = end.position;
            }

            _startPos = startPos;
            _endPos = endPos;
            _middlePos = (_startPos + _endPos) / 2f;

            LerpPositions();
        }

        private void LerpPositions()
        {
            start.position = Vector3.Slerp(_lastStartPos, _startPos, Time.deltaTime * 32f);
            _lastStartPos = start.position;

            middle.position = Vector3.Slerp(_lastMidPos, _middlePos, Time.deltaTime * 14f);
            _lastMidPos = middle.position;

            end.position = Vector3.Slerp(_lastEndPos, _endPos, Time.deltaTime * 6f);
            _lastEndPos = end.position;
        }

        private void OnDrawGizmos()
        {
            if (_currentPlane != null)
            {
                Gizmos.color = Color.green;

                var center = _currentPlane.GetCenter();
                //var end = center + _currentPlane.GetPlane().normal * 0.5f;
                var end = transform.position;

                Gizmos.DrawLine(center, end);
                Gizmos.DrawWireSphere(center, 0.05f);
            }
        }
    }
}
