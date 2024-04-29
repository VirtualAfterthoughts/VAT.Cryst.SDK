using Newtonsoft.Json.Linq;


using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Audio;
using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Props
{
    [DataShardIdentifier("Impact Material")]
    public class ImpactMaterial : DataShard
    {
        [Serializable]
        public struct ImpactLevel
        {
            public ShardReferenceT<AudioCollection> audioCollection;
        }

        [Serializable]
        public struct ImpactGroup
        {
            public ImpactLevel[] impactLevels;

            public ShardReferenceT<SurfaceMaterial> targetMaterial;
        }

        [SerializeField]
        private ImpactGroup[] _impactGroups = new ImpactGroup[0];

        public ImpactGroup[] ImpactGroups => _impactGroups;

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_impactGroups)));
        }
#endif

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("impactGroups", JArray.FromObject(_impactGroups));
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("impactGroups", out var groupToken)) 
            {
                _impactGroups = groupToken.ToObject<ImpactGroup[]>();
            }
        }
    }
}
