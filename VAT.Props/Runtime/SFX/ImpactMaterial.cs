using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Packaging;

namespace VAT.Props
{
    [CreateAssetMenu(menuName = "Cryst/Props/Impact Material")]
    public class ImpactMaterial : ScriptableObject
    {
        [Serializable]
        public struct ImpactLevel
        {
            public AudioClip[] audioClips;
        }

        [Serializable]
        public struct ImpactGroup
        {
            public ImpactLevel[] impactLevels;

            public ShardReferenceT<SurfaceMaterialShard> targetMaterial;
        }

        [SerializeField]
        private ImpactGroup[] _impactGroups = new ImpactGroup[0];
    }
}
