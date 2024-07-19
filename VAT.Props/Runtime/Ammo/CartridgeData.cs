using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Props
{
    [Serializable]
    public class CartridgeDataReference : ShardReferenceT<CartridgeData> { }

    [DisplayName("Cartridge Data")]
    public class CartridgeData : DataShard
    {
        [SerializeField]
        private SpawnableShardReference _spawnable;

        public SpawnableShardReference Spawnable => _spawnable;

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("spawnable", _spawnable.Address.ID);
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("spawnable", out var spawnable))
            {
                _spawnable = new SpawnableShardReference(new(spawnable.ToString()));
            }
        }

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_spawnable)));
        }
#endif
    }
}
