using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Input;

namespace VAT.Avatars
{
    public static class HandPoseCreator
    {
        public static HandPoseData Clone(HandPoseData pose)
        {
            FingerPoseData[] fingers = new FingerPoseData[pose.fingers.Length];
            for (var i = 0; i < fingers.Length; i++)
            {
                fingers[i].splay = pose.fingers[i].splay;
                fingers[i].pressure = pose.fingers[i].pressure;

                fingers[i].phalanges = new PhalanxPoseData[pose.fingers[i].phalanges.Length];
                pose.fingers[i].phalanges.CopyTo(fingers[i].phalanges, 0);
            }

            ThumbPoseData[] thumbs = new ThumbPoseData[pose.thumbs.Length];
            for (var i = 0; i < thumbs.Length; i++)
            {
                thumbs[i].spread = pose.thumbs[i].spread;
                thumbs[i].stretched = pose.thumbs[i].stretched;
                thumbs[i].twist = pose.thumbs[i].twist;
                thumbs[i].pressure = pose.thumbs[i].pressure;

                thumbs[i].phalanges = new PhalanxPoseData[pose.thumbs[i].phalanges.Length];
                pose.thumbs[i].phalanges.CopyTo(thumbs[i].phalanges, 0);
            }

            HandPoseData result = new HandPoseData()
            {
                fingers = fingers,
                thumbs = thumbs,
                centerOfPressure = pose.centerOfPressure,
                rotationOffset = pose.rotationOffset,
            };

            return result;
        }

        public static HandPoseData Lerp(HandPoseData a, HandPoseData b, float t)
        {
            HandPoseData data = new()
            {
                fingers = CreateFingers(b.fingers.Length),
                thumbs = CreateThumbs(b.thumbs.Length)
            };

            HandPoseRemapper.RemapFingers(a.fingers, data.fingers);
            HandPoseRemapper.RemapThumbs(a.thumbs, data.thumbs);

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

        public Vector2 centerOfPressure;

        public Quaternion rotationOffset;
    }
}
