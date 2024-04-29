using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace VAT.Packaging.Editor
{
    public class ShippableTreeViewItem : TreeViewItem
    {
        public Shippable shippable;

        public ShippableTreeViewItem(Shippable shippable)
        {
            this.shippable = shippable;
            this.displayName = shippable.Info.Title;

            Texture2D icon;
            if (shippable is IAssetShard assetShard && assetShard.MainAsset.EditorAsset != null)
            {
                var obj = assetShard.MainAsset.EditorAsset;

                if (obj is GameObject go)
                {
                    icon = AssetPreview.GetAssetPreview(go);
                }
                else
                {
                    icon = AssetPreview.GetMiniThumbnail(obj);
                }
            }
            else
            {
                icon = EditorGUIUtility.GetIconForObject(shippable);
            }

            this.icon = icon;
        }
    }
}
