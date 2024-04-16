using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Misc
{
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("Virtual Afterthoughts/Lines/Draw Bezier Curve")]
    public class DrawBezierCurve : LineDrawer
    {
        [SerializeField]
        private Transform[] _points = new Transform[0];

        private void LateUpdate()
        {
            RenderLine();
        }

        protected override void OnRenderLine(LineRenderer renderer)
        {
            //get smoothed values
            var linePositions = GetPoints();
            Vector3[] smoothedPoints = SmoothLine(linePositions, 0.05f);

            //set line settings
            renderer.positionCount = smoothedPoints.Length;

            bool useWorldSpace = renderer.useWorldSpace;

            for (var i = 0; i < smoothedPoints.Length; i++)
            {
                SetPosition(i, smoothedPoints[i], renderer, useWorldSpace);
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

        private Vector3[] GetPoints()
        {
            //add positions
            var linePositions = new Vector3[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                linePositions[i] = _points[i].position;
            }

            return linePositions;
        }

        public static Vector3[] SmoothLine(Vector3[] inputPoints, float segmentSize)
        {
            //create curves
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            //create keyframe sets
            Keyframe[] keysX = new Keyframe[inputPoints.Length];
            Keyframe[] keysY = new Keyframe[inputPoints.Length];
            Keyframe[] keysZ = new Keyframe[inputPoints.Length];

            //set keyframes
            for (int i = 0; i < inputPoints.Length; i++)
            {
                keysX[i] = new Keyframe(i, inputPoints[i].x);
                keysY[i] = new Keyframe(i, inputPoints[i].y);
                keysZ[i] = new Keyframe(i, inputPoints[i].z);
            }

            //apply keyframes to curves
            curveX.keys = keysX;
            curveY.keys = keysY;
            curveZ.keys = keysZ;

            //smooth curve tangents
            for (int i = 0; i < inputPoints.Length; i++)
            {
                curveX.SmoothTangents(i, 0);
                curveY.SmoothTangents(i, 0);
                curveZ.SmoothTangents(i, 0);
            }

            //list to write smoothed values to
            List<Vector3> lineSegments = new List<Vector3>();

            //find segments in each section
            for (int i = 0; i < inputPoints.Length; i++)
            {
                //add first point
                lineSegments.Add(inputPoints[i]);

                //make sure within range of array
                if (i + 1 < inputPoints.Length)
                {
                    //find distance to next point
                    float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);

                    //number of segments
                    int segments = (int)(distanceToNext / segmentSize);

                    //add segments
                    for (int s = 1; s < segments; s++)
                    {
                        //interpolated time on curve
                        float time = ((float)s / (float)segments) + (float)i;

                        //sample curves to find smoothed position
                        Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));

                        //add to list
                        lineSegments.Add(newSegment);
                    }
                }
            }

            return lineSegments.ToArray();
        }
    }
}
