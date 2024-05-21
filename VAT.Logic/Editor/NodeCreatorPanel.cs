using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace VAT.Logic.Editor
{
    [Overlay(typeof(SceneView), "Node Creator")]
    public class NodeCreatorPanel : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Node Creator" };

            var panelAsset = Resources.Load<VisualTreeAsset>("UXML/NodeCreatorPanel");
            VisualElement ui = panelAsset.Instantiate();
            
            root.Add(ui);

            return root;
        }
    }
}
