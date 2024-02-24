using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    public static class HandPoseCreator
    {
        public static HandPoseData Lerp(HandPoseData a, HandPoseData b, float t)
        {
            HandPoseData data = new()
            {
                fingers = a.RemapFingers(b.fingers.Length),
                thumbs = a.RemapThumbs(b.thumbs.Length)
            };

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
                float percent = i / (float)remapLength;
                float originalValue = percent * originalLength;
                int originalIndex = Mathf.FloorToInt(originalValue);

                float blend = originalValue - originalIndex;

                var original = fingers[originalIndex];
                var next = originalIndex + 1 < originalLength ? fingers[originalIndex + 1] : original;

                data[i].splay = Mathf.Lerp(original.splay, next.splay, blend);

                RemapPhalanges(data[i].phalanges, original.phalanges, next.phalanges, blend);
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
                float percent = i / (float)remapLength;
                float originalValue = percent * originalLength;
                int originalIndex = Mathf.FloorToInt(originalValue);

                float blendPhalanx = originalValue - originalIndex;

                var originalPhalanx = original[originalIndex];
                var nextPhalanx = originalIndex + 1 < originalLength ? original[originalIndex + 1] : originalPhalanx;

                // Next
                float nextPercent = i / (float)remapLength;
                float nextOriginalValue = nextPercent * nextLength;
                int nextOriginalIndex = Mathf.FloorToInt(nextOriginalValue);

                float nextBlendPhalanx = nextOriginalValue - nextOriginalIndex;

                var nextOriginalPhalanx = next[nextOriginalIndex];
                var nextNextPhalanx = nextOriginalIndex + 1 < nextLength ? next[nextOriginalIndex + 1] : nextOriginalPhalanx;

                // Calculate curl
                float originalBlend = Mathf.Lerp(originalPhalanx.curl, nextPhalanx.curl, blendPhalanx);
                float nextBlend = Mathf.Lerp(nextOriginalPhalanx.curl, nextNextPhalanx.curl, nextBlendPhalanx);

                data[i].curl = Mathf.Lerp(originalBlend, nextBlend, blend);
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
                var original = thumbs[i];

                data[i].stretched = original.stretched;
                data[i].spread = original.spread;
                data[i].twist = original.twist;

                for (var j = 0; j < data[i].phalanges.Length; j++)
                {
                    data[i].phalanges[j].curl = original.phalanges[j].curl;
                }
            }
        }

        public readonly FingerPoseData[] RemapFingers(int fingerCount = 4)
        {
            FingerPoseData[] data = new FingerPoseData[fingerCount];

            for (var i = 0; i < fingerCount; i++)
            {
                PhalanxPoseData[] phalanges = new PhalanxPoseData[fingers[i].phalanges.Length];

                for (var j = 0; j < phalanges.Length; j++)
                {
                    phalanges[j] = new PhalanxPoseData()
                    {
                        curl = fingers[i].phalanges[j].curl,
                    };
                }

                var pose = new FingerPoseData
                {
                    splay = fingers[i].splay,
                    phalanges = phalanges
                };

                data[i] = pose;
            }

            return data;
        }

        public readonly ThumbPoseData[] RemapThumbs(int thumbCount = 1)
        {
            ThumbPoseData[] data = new ThumbPoseData[thumbCount];

            for (var i = 0; i < thumbCount; i++)
            {
                PhalanxPoseData[] phalanges = new PhalanxPoseData[thumbs[i].phalanges.Length];

                for (var j = 0; j < phalanges.Length; j++)
                {
                    phalanges[j] = new PhalanxPoseData()
                    {
                        curl = thumbs[i].phalanges[j].curl,
                    };
                }

                var pose = new ThumbPoseData
                {
                    spread = thumbs[i].spread,
                    twist = thumbs[i].twist,
                    stretched = thumbs[i].stretched,
                    phalanges = phalanges
                };

                data[i] = pose;
            }

            return data;
        }
    }
}
