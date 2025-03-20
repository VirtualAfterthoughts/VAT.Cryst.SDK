using UnityEngine;

using VAT.Shared;

namespace VAT.Pooling
{
    public sealed class LootTablePlacer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The loot table to grab an item from.")]
        private LootTableReference _lootTable;

        [SerializeField]
        [Tooltip("Leave false if an item in the loot table should be placed when the level loads. If you set this to true, manually call Trigger to place the item.")]
        private bool _manualSpawning = false;

        private void Awake()
        {
            if (!_manualSpawning)
                Trigger();
        }

        public void Trigger()
        {
            if (_lootTable.TryGetShard(out var content))
            {
                var item = content.GetLootItem();

                GlobalSpawner.Register(item.spawnable);

                var info = new GlobalSpawner.SpawnRequestInfo()
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    scale = transform.lossyScale,
                    spawnable = item.spawnable,
                };

                GlobalSpawner.Spawn(info);
            }
        }
    }
}
