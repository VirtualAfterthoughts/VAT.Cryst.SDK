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

            var packages = new TreeViewItem() { id = id++, depth = 0, displayName = "Packages" };
            foreach (var package in AssetPackager.Instance.GetPackages()) {
                var item = new ShippableTreeViewItem(package)
                {
                    id = id++,
                    depth = 1,
                };
                _items.Add(item.id, item);

                Dictionary<Type, TreeViewItem> crateTypes = new();

                foreach (var content in package.Contents) {

                    if (!crateTypes.TryGetValue(content.GetType(), out TreeViewItem parent))
                    {
                        parent = new TreeViewItem
                        {
                            id = id++,
                            depth = 2,
                            displayName = content.GetType().Name,
                            icon = EditorGUIUtility.GetIconForObject(content)
                        };
                        item.AddChild(parent);

                        crateTypes.Add(content.GetType(), parent);
                    }

                    var contentItem = new ShippableTreeViewItem(content)
                    {
                        id = id++,
                        depth = 3,
                    };

                    parent.AddChild(contentItem);

                    _items.Add(contentItem.id, contentItem);
                }

                packages.AddChild(item);
            }

            root.AddChild(packages);

            return root;
        }

        protected override void SingleClickedItem(int id)
        {
            if (_items.TryGetValue(id, out var shippable)) {
                Selection.activeObject = shippable.shippable;
            }
            else {
                Selection.activeObject = null;
            }
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search) {
            if (item is not ShippableTreeViewItem)
                return false;

            return base.DoesItemMatchSearch(item, search);
        }
    }
}
