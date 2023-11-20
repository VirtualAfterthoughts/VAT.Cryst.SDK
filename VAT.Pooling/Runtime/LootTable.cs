using System;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

namespace VAT.Pooling
{
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "Cryst/Pooling/Loot Table", order = 0)]
    public class LootTable : ScriptableObject
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