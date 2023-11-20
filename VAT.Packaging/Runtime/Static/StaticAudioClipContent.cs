using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [StaticContentIdentifier("Audio Clip", typeof(AudioClip))]
    public class StaticAudioClipContent : StaticContentT<AudioClip>, IAudioClipContent
    {
        [SerializeField]
        private StaticCrystAudioClip _mainAsset;

        public override StaticCrystAsset StaticAsset
        {
            get
            {
                return _mainAsset;
            }
            set
            {
                if (value != null && value.GetType() == typeof(StaticCrystAsset))
                {
                    _mainAsset = new StaticCrystAudioClip(value.AssetGUID);
                }
                else
                {
                    _mainAsset = value as StaticCrystAudioClip;
                }
            }
        }

        public StaticCrystAudioClip MainAudioClip { get { return _mainAsset; } set { _mainAsset = value; } }

#if UNITY_EDITOR
        public override string Group => "AudioClips";
#endif
    }
}
