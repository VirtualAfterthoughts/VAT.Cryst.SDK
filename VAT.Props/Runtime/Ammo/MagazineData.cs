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
    public class MagazineDataReference : ShardReferenceT<MagazineData> { }

    [Serializable]
    public struct MagazineInventory
    {
        public CartridgeDataReference[] cartridges;
    }

    [DisplayName("Magazine Data")]
    public class MagazineData : DataShard
    {
        [SerializeField]
        private SpawnableShardReference _spawnable;

        [SerializeField]
        private MagazineInventory _inventory;

        public SpawnableShardReference Spawnable => _spawnable;

        public MagazineInventory Inventory => _inventory;

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("spawnable", _spawnable.Address.ID);

            json.Add("inventory", JObject.FromObject(_inventory));
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("spawnable", out var spawnable))
            {
                _spawnable = new SpawnableShardReference(new(spawnable.ToString()));
            }

            if (json.TryGetValue("inventory", out var inventory))
            {
                _inventory = inventory.ToObject<MagazineInventory>();
            }
        }

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_spawnable)));

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_inventory)));
        }
#endif
    }
}
