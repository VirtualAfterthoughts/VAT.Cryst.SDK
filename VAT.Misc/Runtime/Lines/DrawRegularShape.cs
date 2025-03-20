using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Shared.Extensions;

namespace VAT.Misc
{
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("Virtual Afterthoughts/Lines/Draw Regular Shape")]
    public class DrawRegularShape : LineDrawer
    {
        [SerializeField]
        [Min(0f)]
        private float _radius = 0.5f;

        [SerializeField]
        [Range(3, 128)]
        private int _sides = 8;

        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = Mathf.Max(0f, _radius);
                RenderLine();
            }
        }

        public int Sides
        {
            get
            {
                return _sides;
            }
            set
            {
                _sides = Mathf.Max(3, _sides);
                RenderLine();
            }
        }

        protected override void OnRenderLine(LineRenderer renderer)
        {
            float scale = transform.lossyScale.Max();
            float radius = _radius * scale;

            var up = transform.up;
            var forward = transform.forward;

            float interiorAngle = 180f - ((_sides - 2) * 180f / _sides);
            Vector3[] points = new Vector3[_sides];
            for (var i = 0; i < _sides; i++)
            {
                float angle = i * interiorAngle;
                points[i] = transform.position + Quaternion.AngleAxis(angle, forward) * up * radius;
            }

            int length = _sides > 2 ? points.Length + 2 : points.Length;

            renderer.positionCount = length;

            bool useWorldSpace = renderer.useWorldSpace;

            for (var i = 0; i < points.Length; i++)
            {
                SetPosition(i, points[i], renderer, useWorldSpace);
            }

            if (_sides > 2)
            {
                SetPosition(points.Length, points[0], renderer, useWorldSpace);
                SetPosition(points.Length + 1, points[1], renderer, useWorldSpace);
            }
        }

        private void SetPosition(int index, Vector3 worldPosition, LineRenderer renderer, bool useWorldSpace)
        {
            if (useWorldSpace)
            {
                renderer.SetPosition(index, worldPosition);
            }
            else
            {
                renderer.SetPosition(index, renderer.transform.InverseTransformPoint(worldPosition));
            }
        }
    }
}
