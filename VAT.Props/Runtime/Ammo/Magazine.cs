using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Pooling;

namespace VAT.Props
{
    public class Magazine : MonoBehaviour
    {
        [SerializeField]
        private MagazineDataReference _dataReference;

        public MagazineDataReference DataReference
        {
            get
            {
                return _dataReference;
            }
            set
            {
                _dataReference = value;
            }
        }

        private void Awake()
        {
            Refill();
        }

        public void Refill()
        {
            DataReference.TryGetShard(out var data);
            
            foreach (var cart in data.Inventory.cartridges)
            {
                var spawnable = new Spawnable(cart.Shard.Spawnable);

                AssetSpawner.Register(spawnable);

                AssetSpawner.Spawn(new AssetSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    spawnCallback = OnCartridgeSpawn,
                });
            }
        }

        private void OnCartridgeSpawn(AssetSpawner.SpawnCallbackInfo info)
        {
            var cart = info.assetPoolable.transform;

            cart.parent = transform;
            cart.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
