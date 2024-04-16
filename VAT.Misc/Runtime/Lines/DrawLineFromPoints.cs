using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Misc
{
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("Virtual Afterthoughts/Lines/Draw Line From Points")]
    public class DrawLineFromPoints : LineDrawer
    {
        [SerializeField]
        private Transform[] _points = new Transform[0];

        protected override void OnRenderLine(LineRenderer renderer)
        {
            renderer.positionCount = _points.Length;

            bool useWorldSpace = renderer.useWorldSpace;

            for (var i = 0; i < _points.Length; i++)
            {
                SetPosition(i, _points[i].position, renderer, useWorldSpace);
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
