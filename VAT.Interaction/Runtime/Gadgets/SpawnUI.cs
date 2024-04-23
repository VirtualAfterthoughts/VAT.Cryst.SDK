using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using VAT.Packaging;
using VAT.UI;

namespace VAT.Interaction
{
    public class SpawnUI : MonoBehaviour
    {
        public static SpawnableContentReference SelectedSpawnable = null;

        public UIPageCollectionRenderer pageCollection;

        public UIPageRenderer tabsPageRenderer;

        private UIPage _tabsPage = null;
        private UIPageCollection _spawnablesPageCollection = null;
        private UIPageCollection _toolsPageCollection;

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

            toolsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Spawn",
                OnPressed = null
            });

            toolsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Remove",
                OnPressed = null
            });

            toolsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Weld",
                OnPressed = null
            });

            _toolsPageCollection.AddPage(toolsPage);
        }

        private void CreateTabs()
        {
            _tabsPage = new();

            _tabsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Spawnables",
                OnPressed = ShowSpawnables,
            });

            _tabsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Tools",
                OnPressed = ShowTools,
            });

            _tabsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Packages",
                OnPressed = null
            });

            _tabsPage.AddElement(new UIPageElement()
            {
                DisplayName = "Authors",
                OnPressed = null
            });
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

            var contents = AssetPackager.Instance.GetContents<ISpawnableContent>();

            AddSpawnablePages(contents, _spawnablesPageCollection);

            RenderAllPages();
        }

        private void AddSpawnablePages(IEnumerable<ISpawnableContent> contents, UIPageCollection collection)
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

                currentPage.AddElement(new UIPageElement()
                {
                    DisplayName = content.Info.Title,
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
            SelectedSpawnable = new SpawnableContentReference(address);
        } 
    }
}
