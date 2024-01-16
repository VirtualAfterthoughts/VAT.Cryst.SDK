using System.Collections;
using System.Collections.Generic;

using static Unity.Mathematics.math;

using UnityEngine;

using VAT.Shared.Data;

namespace VAT.Avatars.Skeletal
{
    using Unity.Mathematics;
    using VAT.Shared.Math;

    public struct BodyRemapGroup {
        public SimpleTransform transform;
        public Ellipsoid ellipsoid;

        public BodyRemapGroup(SimpleTransform transform, Ellipsoid ellipsoid) {
            this.transform = transform;
            this.ellipsoid = ellipsoid;
        }
    }

    public static class BodyRemapping
    {
        public static BodyRemapGroup BlendClosest(float3 point, params BodyRemapGroup[] groups) {
            float distance = float.PositiveInfinity;
            BodyRemapGroup? closest = null;
            for (var i = 0; i < groups.Length; i++) {
                var group = groups[i];

                float dist = lengthsq(point - groups[i].transform.position);

                if (!closest.HasValue || dist < distance) {
                    distance = dist;
                    closest = group;
                }
            }

            float secondDistance = float.PositiveInfinity;
            BodyRemapGroup? secondClosest = null;
            for (var i = 0; i < groups.Length; i++) {
                var group = groups[i];
                if (group.GetHashCode() == closest.Value.GetHashCode())
                    continue;

                float dist = lengthsq(point - groups[i].transform.position);

                if (!secondClosest.HasValue || dist < secondDistance) {
                    secondDistance = dist;
                    secondClosest = group;
                }
            }

            var start = closest.Value.transform.position;
            var end = secondClosest.Value.transform.position;
            LineData line = new(start, end);
            float maxDist = distancesq(start, end);
            float lineDist = distancesq(start, line.ClosestPointOnLine(point));
            float lerp = lineDist / maxDist;

            return new BodyRemapGroup(
                SimpleTransform.Lerp(closest.Value.transform, secondClosest.Value.transform, lerp),
                Ellipsoid.Lerp(closest.Value.ellipsoid, secondClosest.Value.ellipsoid, lerp));
        }
    }
}
