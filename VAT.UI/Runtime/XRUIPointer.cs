using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace VAT.UI
{
    public class XRUIPointer : MonoBehaviour, IXRUIInteractor
    {
        public Transform relativeParent;
        public GameObject pointerRoot;
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

        public Vector3 GetEndPosition()
        {
            return end.position;
        }

        private bool _isPressed = false;

        public bool IsPressed()
        {
            return _isPressed;
        }

        public void SetPressed(bool pressed)
        {
            _isPressed = pressed;
        }

        private void OnEnable()
        {
            Deactivate();
        }

        private void OnDisable()
        {
            Deactivate();
        }

        private bool _isActive = false;

        private void Activate()
        {
            _isActive = true;

            if (EventSystem.current.currentInputModule is XRUIInputModule module)
            {
                module.RegisterInteractor(this);
            }

            pointerRoot.SetActive(true);
        }

        private void Deactivate()
        {
            _isActive = false;

            if (EventSystem.current.currentInputModule is XRUIInputModule module)
            {
                module.DeregisterInteractor(this);
            }

            pointerRoot.SetActive(false);
        }

        private bool _isValid = false;

        public void Update()
        {
            if (_isValid && !_isActive)
            {
                Activate();
            }
            else if (!_isValid && _isActive)
            {
                Deactivate();
            }
        }

        public void LateUpdate()
        {
            _isValid = false;

            var colliders = Physics.OverlapSphere(transform.position, 0.01f);

            UIPlane targetPlane = null;
            float closestDistance = float.PositiveInfinity;

            foreach (var collider in colliders)
            {
                if (UIPlane.Cache.TryGet(collider.gameObject, out var hber))
                {
                    var distance = (transform.position - hber.GetCenter()).magnitude;

                    if (distance < closestDistance)
                    {
                        targetPlane = hber;
                        closestDistance = distance;
                    }
                }
            }

            _currentPlane = targetPlane;

            if (_currentPlane == null)
            {
                return;
            }

            var plane = _currentPlane.GetPlane();
            var normal = plane.normal;

            var extents = _currentPlane.GetSize() * 0.5f;
            var rotation = _currentPlane.GetRotation();

            var startPos = transform.position;
            Vector3 endPos;

            if (plane.Raycast(new Ray(transform.position, transform.forward), out var enter))
            {
                endPos = transform.position + transform.forward * enter;
            }
            else
            {
                return;
            }

            var prevEndPos = endPos;
            var inPlane = Quaternion.Inverse(rotation) * (endPos - _currentPlane.GetCenter());
            inPlane = Vector3.Min(Vector3.Max(inPlane, -extents), extents);

            endPos = (rotation * inPlane) + _currentPlane.GetCenter();

            if (endPos != prevEndPos)
            {
                return;
            }

            end.rotation = Quaternion.LookRotation(-normal, transform.up);

            _startPos = startPos;
            _endPos = endPos;
            _middlePos = (_startPos + _endPos) / 2f;

            LerpPositions();

            _isValid = true;
        }

        private void LerpPositions()
        {
            start.position = Vector3.Slerp(relativeParent.TransformPoint(_lastStartPos), _startPos, Time.deltaTime * 32f);
            _lastStartPos = relativeParent.InverseTransformPoint(start.position);

            middle.position = Vector3.Slerp(relativeParent.TransformPoint(_lastMidPos), _middlePos, Time.deltaTime * 14f);
            _lastMidPos = relativeParent.InverseTransformPoint(middle.position);

            end.position = Vector3.Slerp(relativeParent.TransformPoint(_lastEndPos), _endPos, Time.deltaTime * 6f);
            _lastEndPos = relativeParent.InverseTransformPoint(end.position);

            if (_currentPlane != null) 
            {
                end.position = _currentPlane.GetPlane().ClosestPointOnPlane(end.position);
            }
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
