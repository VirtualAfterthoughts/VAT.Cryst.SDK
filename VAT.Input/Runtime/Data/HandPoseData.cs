using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Avatars
{
    public static class HandPoseCreator
    {
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

        public readonly FingerPoseData[] RemapFingers(int fingerCount = 4)
        {
            FingerPoseData[] data = new FingerPoseData[fingerCount];

            for (var i = 0; i < fingerCount; i++)
            {
                var pose = new FingerPoseData();

                pose.splay = fingers[i].splay;
                pose.phalanges = fingers[i].phalanges;

                data[i] = pose;
            }

            return data;
        }

        public readonly ThumbPoseData[] RemapThumbs(int thumbCount = 1)
        {
            ThumbPoseData[] data = new ThumbPoseData[thumbCount];

            for (var i = 0; i < thumbCount; i++)
            {
                var pose = new ThumbPoseData();

                pose.spread = thumbs[i].spread;
                pose.twist = thumbs[i].twist;
                pose.stretched = thumbs[i].stretched;
                pose.phalanges = thumbs[i].phalanges;

                data[i] = pose;
            }

            return data;
        }
    }
}
