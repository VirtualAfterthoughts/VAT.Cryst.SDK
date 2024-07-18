using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using VAT.Packaging;
using VAT.Serialization.JSON;

using Random = UnityEngine.Random;

namespace VAT.Pooling
{
    public class LootTableReference : ShardReferenceT<LootTable>
    {
    }

    [DisplayName("Loot Table")]
    public class LootTable : DataShard
    {
        [Serializable]
        public struct LootItem
        {
            [Range(0f, 100f)]
            [Tooltip("The chance that this item will be selected.")]
            public float probability;

            [Tooltip("The spawnable that this item contains.")]
            public Spawnable spawnable;
        }

        [SerializeField]
        [Tooltip("The list of items in this table.")]
        private LootItem[] _lootItems = new LootItem[0];

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("lootItems", JArray.FromObject(_lootItems));
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("lootItems", out var lootItems))
            {
                _lootItems = lootItems.ToObject<LootItem[]>();
            }
        }

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_lootItems)));
        }
#endif

        /// <summary>
        /// Returns a random item in this table based on probability.
        /// </summary>
        /// <returns></returns>
        public LootItem GetLootItem()
        {
            float probability = _lootItems.Sum(p => p.probability);
            float value = Random.Range(0f, probability);

            float sum = 0;
            foreach (var item in _lootItems)
            {
                // We loop until the random number is less than our cumulative probability
                if (value <= (sum += item.probability))
                {
                    return item;
                }
            }

            // We shouldn't ever get here unless the list is empty
            return new LootItem();
        }
    }
}