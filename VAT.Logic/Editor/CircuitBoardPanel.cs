using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VAT.Logic.Editor
{
    [Overlay(typeof(SceneView), "Circuit Board", true)]
    public class CircuitBoardPanel : Overlay
    {
        private Toggle _nodeToggle = null;
        private Toggle _portToggle = null;

        private ObjectField _nodeField = null;
        private ObjectField _portField = null;

        public override void OnCreated()
        {
            base.OnCreated();

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!displayed)
            {
                return;
            }

            if (_nodeToggle.value)
            {
                DrawNodes(sceneView);
            }

            if (_portToggle.value)
            {
                DrawPorts(sceneView);
            }
        }

        private void SelectNode(Node node)
        {
            _nodeField.value = node;
        }

        private void SelectPort(Port port)
        {
            _portField.value = port;
        }

        private void DrawNodes(SceneView sceneView)
        {
            var nodes = Object.FindObjectsOfType<Node>();

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
            };

            style.normal.textColor = Color.black;

            foreach (var node in nodes)
            {
                var name = node.name;

                GUIContent content = new(name);

                bool button = Handles.Button(node.transform.position, node.transform.rotation, 0.03f, 0.06f, Handles.SphereHandleCap);

                Handles.Label(node.transform.position, content, style);

                if (button)
                {
                    SelectNode(node);
                    break;
                }
            }
        }

        private void DrawPorts(SceneView sceneView)
        {
            var ports = Object.FindObjectsOfType<Port>();

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
            };

            style.normal.textColor = Color.magenta;

            foreach (var port in ports)
            {
                var name = port.name;

                GUIContent content = new(name);

                bool button = Handles.Button(port.transform.position, port.transform.rotation, 0.03f, 0.06f, Handles.SphereHandleCap);

                Handles.Label(port.transform.position, content, style);

                if (button)
                {
                    SelectPort(port);
                    break;
                }
            }
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Circuit Board" };

            _nodeToggle = new("Show Nodes");
            _nodeToggle.SetValueWithoutNotify(true);
            root.Add(_nodeToggle);

            _portToggle = new("Show Ports");
            _portToggle.SetValueWithoutNotify(true);
            root.Add(_portToggle);

            _nodeField = new("Selected Node")
            {
                objectType = typeof(Node)
            };
            root.Add(_nodeField);

            _portField = new("Selected Port")
            {
                objectType = typeof(Port)
            };
            root.Add(_portField);

            return root;
        }
    }
}
