using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using VAT.Packaging;
using VAT.Shared;

namespace VAT.Pooling
{
    public sealed class LootTablePlacer : MonoBehaviour, ITriggerable
    {
        [SerializeField]
        [Tooltip("The loot table to grab an item from.")]
        private ScriptableObjectContentReference _lootTableReference;

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
            if (_lootTableReference.TryGetContent(out var content))
            {
                content.MainAssetT.LoadAsset(OnLootTableLoaded);
            }
        }

        private void OnLootTableLoaded(ScriptableObject asset)
        {
            if (asset is LootTable lootTable)
            {
                var item = lootTable.GetLootItem();

                AssetSpawner.Register(item.spawnable);

                var info = new AssetSpawner.SpawnRequestInfo()
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    scale = transform.lossyScale,
                    spawnable = item.spawnable,
                };

                AssetSpawner.Spawn(info);
            }
        }
    }
}
