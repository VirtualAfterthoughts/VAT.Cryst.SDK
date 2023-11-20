using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace VAT.Packaging.Editor {
    public class ShippableTreeViewItem : TreeViewItem {
        public Shippable shippable;

        public ShippableTreeViewItem(Shippable shippable) {
            this.shippable = shippable;
            this.displayName = shippable.Info.Title;
            this.icon = EditorGUIUtility.GetIconForObject(shippable);
        }
    }
}
