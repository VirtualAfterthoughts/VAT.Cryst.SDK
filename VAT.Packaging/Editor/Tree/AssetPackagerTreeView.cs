using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging.Editor
{
    public class AssetPackagerTreeView : TreeView
    {
        private Dictionary<int, ShippableTreeViewItem> _items;

        public AssetPackagerTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {

            treeViewState.expandedIDs.TryAdd(1);

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            _items = new Dictionary<int, ShippableTreeViewItem>();

            var id = 0;
            var root = new TreeViewItem { id = id++, depth = -1, displayName = "Root" };

            var crystals = new TreeViewItem() { id = id++, depth = 0, displayName = "Crystals" };
            foreach (var package in AssetPackager.Instance.GetCrystals())
            {
                var item = new ShippableTreeViewItem(package)
                {
                    id = id++,
                    depth = 1,
                };
                _items.Add(item.id, item);

                Dictionary<Type, TreeViewItem> shardTypes = new();

                foreach (var shard in package.Shards)
                {
                    if (!shardTypes.TryGetValue(shard.GetType(), out TreeViewItem parent))
                    {
                        parent = new TreeViewItem
                        {
                            id = id++,
                            depth = 2,
                            displayName = shard.GetType().Name,
                            icon = EditorGUIUtility.GetIconForObject(shard)
                        };
                        item.AddChild(parent);

                        shardTypes.Add(shard.GetType(), parent);
                    }

                    var shardItem = new ShippableTreeViewItem(shard)
                    {
                        id = id++,
                        depth = 3,
                    };

                    parent.AddChild(shardItem);

                    _items.Add(shardItem.id, shardItem);
                }

                crystals.AddChild(item);
            }

            root.AddChild(crystals);

            return root;
        }

        protected override void SingleClickedItem(int id)
        {
            if (_items.TryGetValue(id, out var shippable))
            {
                Selection.activeObject = shippable.shippable;
            }
            else
            {
                Selection.activeObject = null;
            }
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            if (item is not ShippableTreeViewItem)
                return false;

            return base.DoesItemMatchSearch(item, search);
        }
    }
}
