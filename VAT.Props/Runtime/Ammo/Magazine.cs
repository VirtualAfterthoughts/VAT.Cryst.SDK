using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Pooling;

namespace VAT.Props
{
    public class Magazine : MonoBehaviour
    {
        [SerializeField]
        private MagazineDataReference _data;

        public MagazineDataReference Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        private void Awake()
        {
            Refill();
        }

        public void Refill()
        {
            foreach (var cart in Data.Shard.Inventory.cartridges)
            {
                var spawnable = new Spawnable(cart.Shard.Spawnable);

                GlobalSpawner.Register(spawnable);

                GlobalSpawner.Spawn(new GlobalSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    spawnCallback = OnCartridgeSpawn,
                });
            }
        }

        private void OnCartridgeSpawn(GlobalSpawner.SpawnCallbackInfo info)
        {
            var cart = info.assetPoolable.transform;

            cart.parent = transform;
            cart.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
