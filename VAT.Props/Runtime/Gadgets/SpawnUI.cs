using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using VAT.Packaging;
using VAT.Props;
using VAT.UI;

namespace VAT.Interaction
{
    public class SpawnUI : MonoBehaviour, IInteractorSpawnerModule
    {
        public UIPageCollectionRenderer pageCollection;

        public UIPageRenderer tabsPageRenderer;

        public ShardPreviewUI previewUI;

        private UIPage _tabsPage = null;
        private UIPageCollection _spawnablesPageCollection = null;
        private UIPageCollection _toolsPageCollection;

        public event Action<SpawnableShardReference> OnSpawnableSelected;

        private void Awake()
        {
            CreateTabs();
            CreateTools();

            AssetPackager.HookOnReady(OnPackagerReady);
        }

        private void CreateTools()
        {
            _toolsPageCollection = new UIPageCollection();
            UIPage toolsPage = new();

            toolsPage.AddChild(new UIButton()
            {
                Text = "Spawn",
                OnPressed = null
            });

            toolsPage.AddChild(new UIButton()
            {
                Text = "Remove",
                OnPressed = null
            });

            toolsPage.AddChild(new UIButton()
            {
                Text = "Weld",
                OnPressed = null
            });

            _toolsPageCollection.AddPage(toolsPage);
        }

        private void CreateTabs()
        {
            _tabsPage = new();

            _tabsPage.AddChild(new UIButton()
            {
                Text = "Spawnables",
                OnPressed = ShowSpawnables,
            });

            _tabsPage.AddChild(new UIButton()
            {
                Text = "Tools",
                OnPressed = ShowTools,
            });

            _tabsPage.AddChild(new UIButton()
            {
                Text = "Packages",
                OnPressed = null
            });

            _tabsPage.AddChild(new UIButton()
            {
                Text = "Authors",
                OnPressed = null
            });

            var depth0 = new UIPage()
            {
                Text = "Test Depth 0"
            };

            var depth10 = new UIPage()
            {
                Text = "Test Depth 1, 0"
            };

            var depth11 = new UIPage()
            {
                Text = "Test Depth 1, 1"
            };

            var depth20 = new UIButton()
            {
                Text = "Test Depth 2, 0"
            };

            var depth21 = new UIButton()
            {
                Text = "Test Depth 2, 1"
            };

            depth10.AddChild(depth20);
            depth11.AddChild(depth21);

            depth0.AddChild(depth10);
            depth0.AddChild(depth11);


            _tabsPage.AddChild(depth0);
        }

        private void ShowTools()
        {
            pageCollection.Render(_toolsPageCollection);
        }

        private void ShowSpawnables()
        {
            pageCollection.Render(_spawnablesPageCollection);
        }

        private void OnPackagerReady()
        {
            _spawnablesPageCollection = new UIPageCollection();

            var contents = AssetPackager.Instance.GetShards<ISpawnableShard>();

            AddSpawnablePages(contents, _spawnablesPageCollection);

            RenderAllPages();
        }

        private void AddSpawnablePages(IEnumerable<ISpawnableShard> contents, UIPageCollection collection)
        {
            int maxElements = 9;
            int addedElements = 0;

            UIPage currentPage = new();
            collection.AddPage(currentPage);

            for (var i = 0; i < contents.Count(); i++)
            {
                if (addedElements >= maxElements)
                {
                    currentPage = new();
                    collection.AddPage(currentPage);
                    addedElements = 0;
                }

                var content = contents.ElementAt(i);
                var address = content.Address;

                currentPage.AddChild(new UIButton()
                {
                    Text = content.Info.Title,
                    OnPressed = () =>
                    {
                        SelectSpawnable(address);
                    }
                });

                addedElements++;
            }
        }

        private void RenderAllPages()
        {
            pageCollection.Render(_spawnablesPageCollection);
            tabsPageRenderer.Render(_tabsPage);
        }

        private void SelectSpawnable(Address address)
        {
            var reference = new SpawnableShardReference(address);
            OnSpawnableSelected?.Invoke(reference);

            if (reference.TryGetShard(out var shard)) 
            {
                previewUI.SetShard(shard);
            }
        }

        public void SetSpawningActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
