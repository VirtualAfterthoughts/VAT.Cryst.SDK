using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Avatars;

namespace VAT.Input
{
    public static class HandPoseRemapper
    {
        public static void RemapThumbs(ThumbPoseData[] from, ThumbPoseData[] to)
        {
            int originalLength = from != null ? from.Length : 0;
            int remapLength = to.Length;

            bool hasValidThumbs = originalLength > 0 && remapLength > 0;
            if (!hasValidThumbs)
            {
                return;
            }

            for (var i = 0; i < to.Length; i++)
            {
                var (previous, next, blend) = ArrayRemapper.RemapElements(i, from, to);

                to[i].stretched = Mathf.Lerp(previous.stretched, next.stretched, blend);
                to[i].spread = Mathf.Lerp(previous.spread, next.spread, blend);
                to[i].twist = Mathf.Lerp(previous.twist, next.twist, blend);

                RemapPhalanges(to[i].phalanges, previous.phalanges, next.phalanges, blend);
            }
        }

        public static void RemapFingers(FingerPoseData[] from, FingerPoseData[] to)
        {
            int originalLength = from != null ? from.Length : 0;
            int remapLength = to.Length;

            bool hasValidFingers = originalLength > 0 && remapLength > 0;
            if (!hasValidFingers)
            {
                return;
            }

            for (var i = 0; i < remapLength; i++)
            {
                var (previous, next, blend) = ArrayRemapper.RemapElements(i, from, to);

                to[i].splay = Mathf.Lerp(previous.splay, next.splay, blend);

                RemapPhalanges(to[i].phalanges, previous.phalanges, next.phalanges, blend);
            }
        }

        private static void RemapPhalanges(PhalanxPoseData[] data, PhalanxPoseData[] original, PhalanxPoseData[] next, float blend)
        {
            int originalLength = original.Length;
            int nextLength = next.Length;
            int remapLength = data.Length;

            bool hasValidPhalanges = originalLength > 0 && nextLength > 0 && remapLength > 0;
            if (!hasValidPhalanges)
            {
                return;
            }

            for (var i = 0; i < remapLength; i++)
            {
                // Original
                var (previousOriginal, nextOriginal, blendOriginal) = ArrayRemapper.RemapElements(i, original, data);

                // Next
                var (previousNext, nextNext, blendNext) = ArrayRemapper.RemapElements(i, next, data);

                // Calculate curl
                float first = Mathf.Lerp(previousOriginal.curl, nextOriginal.curl, blendOriginal);
                float second = Mathf.Lerp(previousNext.curl, nextNext.curl, blendNext);

                data[i].curl = Mathf.Lerp(first, second, blend);
            }
        }
    }
}
