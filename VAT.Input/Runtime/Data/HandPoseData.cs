using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;

namespace VAT.Avatars
{
    public static class HandPoseCreator
    {
        public static HandPoseData Lerp(HandPoseData a, HandPoseData b, float t)
        {
            HandPoseData data = new()
            {
                fingers = CreateFingers(b.fingers.Length),
                thumbs = CreateThumbs(b.thumbs.Length)
            };

            a.RemapFingers(data.fingers);
            a.RemapThumbs(data.thumbs);

            for (var i = 0; i < data.fingers.Length; i++)
            {
                for (var j = 0; j < data.fingers[i].phalanges.Length; j++)
                {
                    data.fingers[i].phalanges[j].curl = Mathf.Lerp(data.fingers[i].phalanges[j].curl, b.fingers[i].phalanges[j].curl, t);
                }
            }

            for (var i = 0; i < data.thumbs.Length; i++)
            {
                for (var j = 0; j < data.thumbs[i].phalanges.Length; j++)
                {
                    data.thumbs[i].phalanges[j].curl = Mathf.Lerp(data.thumbs[i].phalanges[j].curl, b.thumbs[i].phalanges[j].curl, t);
                }
            }

            return data;
        }

        public static void SetCurls(PhalanxPoseData[] phalanges, in float curl)
        {
            for (var i = 0; i < phalanges.Length; i++)
            {
                phalanges[i].curl = curl;
            }
        }

        public static ThumbPoseData[] CreateThumbs(int thumbCount = 1, int phalanxCount = 2)
        {
            var thumbs = new ThumbPoseData[thumbCount];

            for (var i = 0; i < thumbCount; i++)
            {
                ThumbPoseData data = new()
                {
                    phalanges = new PhalanxPoseData[phalanxCount]
                };

                for (var j = 0; j < phalanxCount; j++)
                {
                    data.phalanges[j] = new PhalanxPoseData();
                }

                thumbs[i] = data;
            }

            return thumbs;
        }

        public static FingerPoseData[] CreateFingers(int fingerCount = 4, int phalanxCount = 3)
        {
            var fingers = new FingerPoseData[fingerCount];

            for (var i = 0; i < fingerCount; i++)
            {
                FingerPoseData data = new()
                {
                    phalanges = new PhalanxPoseData[phalanxCount]
                };

                for (var j = 0; j < phalanxCount; j++)
                {
                    data.phalanges[j] = new PhalanxPoseData();
                }

                fingers[i] = data;
            }

            return fingers;
        }
    }

    [Serializable]
    public struct HandPoseData
    {
        public ThumbPoseData[] thumbs;

        public FingerPoseData[] fingers;

        public readonly void RemapFingers(FingerPoseData[] data)
        {
            int originalLength = fingers != null ? fingers.Length : 0;
            int remapLength = data.Length;

            bool hasValidFingers = originalLength > 0 && remapLength > 0;
            if (!hasValidFingers)
            {
                return;
            }

            for (var i = 0; i < remapLength; i++)
            {
                var (previous, next, blend) = ArrayRemapper.RemapElements(i, fingers, data);

                data[i].splay = Mathf.Lerp(previous.splay, next.splay, blend);

                RemapPhalanges(data[i].phalanges, previous.phalanges, next.phalanges, blend);
            }
        }

        private readonly void RemapPhalanges(PhalanxPoseData[] data, PhalanxPoseData[] original, PhalanxPoseData[] next, float blend)
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

        public readonly void RemapThumbs(ThumbPoseData[] data)
        {
            int originalLength = thumbs != null ? thumbs.Length : 0;
            int remapLength = data.Length;

            bool hasValidThumbs = originalLength > 0 && remapLength > 0;
            if (!hasValidThumbs)
            {
                return;
            }

            for (var i = 0; i < data.Length; i++)
            {
                var (previous, next, blend) = ArrayRemapper.RemapElements(i, thumbs, data);

                data[i].stretched = Mathf.Lerp(previous.stretched, next.stretched, blend);
                data[i].spread = Mathf.Lerp(previous.spread, next.spread, blend);
                data[i].twist = Mathf.Lerp(previous.twist, next.twist, blend);

                RemapPhalanges(data[i].phalanges, previous.phalanges, next.phalanges, blend);
            }
        }
    }
}
