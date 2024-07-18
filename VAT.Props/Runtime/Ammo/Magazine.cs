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

        [SerializeField]
        private Transform[] _cartridgeTargets = new Transform[0]; 

        private readonly List<Cartridge> _cartridges = new();

        public List<Cartridge> Cartridges => _cartridges;

        private void Awake()
        {
            Refill();
        }

        private Transform FindTarget(Cartridge cartridge)
        {
            var index = Cartridges.IndexOf(cartridge);

            if (index < 0 || index >= _cartridgeTargets.Length)
            {
                return null;
            }

            return _cartridgeTargets[index];
        }

        private void ParentCartridge(Cartridge cartridge)
        {
            var target = FindTarget(cartridge);

            if (target != null)
            {
                var cartridgeTransform = cartridge.transform;
                cartridgeTransform.parent = target;
                cartridgeTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                cartridge.gameObject.SetActive(true);
            }
            else
            {
                cartridge.gameObject.SetActive(false);
            }
        }

        public Cartridge TakeCartridge()
        {
            var cartridge = PeekCartridge();

            if (cartridge == null)
            {
                return null;
            }

            UnloadCartridge(cartridge);

            return cartridge;
        }

        public Cartridge PeekCartridge()
        {
            if (Cartridges.Count <= 0)
            {
                return null;
            }

            return Cartridges[0];
        }

        public void LoadCartridge(Cartridge cartridge)
        {
            _cartridges.Add(cartridge);

            ParentCartridge(cartridge);
        }

        public void UnloadCartridge(Cartridge cartridge)
        {
            _cartridges.Remove(cartridge);

            cartridge.gameObject.SetActive(true);

            cartridge.transform.parent = null;

            foreach (var other in Cartridges)
            {
                ParentCartridge(other);
            }
        }

        public void SpawnCartridge(CartridgeDataReference cartridge)
        {
            var data = cartridge.Shard;

            if (data == null)
            {
                return;
            }

            var spawnable = new Spawnable(data.Spawnable);

            GlobalSpawner.Register(spawnable);

            GlobalSpawner.Spawn(new GlobalSpawner.SpawnRequestInfo()
            {
                spawnable = spawnable,
                spawnCallback = OnCartridgeSpawned
            });
        }

        private void OnCartridgeSpawned(GlobalSpawner.SpawnCallbackInfo info)
        {
            var cart = info.assetPoolable.transform;

            cart.parent = transform;
            cart.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            var cartridge = info.assetPoolable.GetComponent<Cartridge>();

            LoadCartridge(cartridge);
        }

        public void Refill()
        {
            foreach (var cart in Data.Shard.Inventory.cartridges)
            {
                SpawnCartridge(cart);
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var data = Data.Shard;

            if (data == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            for (var i = 0; i < _cartridgeTargets.Length && i < data.Inventory.cartridges.Length; i++)
            {
                var target = _cartridgeTargets[i];

                var spawnable = data.Inventory.cartridges[i].Shard.Spawnable;
                var previewMesh = spawnable.Shard.PreviewMesh;

                if (previewMesh == null || previewMesh.EditorAssetT == null)
                {
                    continue;
                }

                Gizmos.matrix = target.localToWorldMatrix;

                Gizmos.DrawMesh(previewMesh.EditorAssetT);
            }
        }
#endif
    }
}
