using System.Collections;
using System.Collections.Generic;

using UnityEditor;
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

        private Button _wireButton = null;

        public const float WIRE_THICKNESS = 5f;

        public const float ICON_SCALE = 0.875f;

        public override void OnCreated()
        {
            base.OnCreated();

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += OnEditorUpdate;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.update -= OnEditorUpdate;
        }

        private GUIStyle GetNodeStyle()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            style.normal.textColor = Color.cyan;
            style.fontSize = (int)(style.fontSize * ICON_SCALE);

            return style;
        }

        private GUIStyle GetPortStyle()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            style.normal.textColor = Color.yellow;
            style.fontSize = (int)(style.fontSize * ICON_SCALE);

            return style;
        }

        public Node GetSelectedNode()
        {
            return _nodeField.value as Node;
        }

        public Port GetSelectedPort()
        {
            return _portField.value as Port;
        }

        private void OnEditorUpdate()
        {
            if (!displayed)
            {
                return;
            }

            var node = GetSelectedNode();
            var port = GetSelectedPort();

            bool hasSingle = !(node && port) && (node || port);

            if (hasSingle)
            {
                SceneView.RepaintAll();
            }
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

            DrawWiring();
        }

        private void DrawWiring()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            Handles.color = new Color(0f, 0f, 0f, 1f);

            var mouse = Event.current.mousePosition;
            var ray = HandleUtility.GUIPointToWorldRay(mouse);

            var mouseInWorld = ray.origin + ray.direction * 2f;

            if (node && port)
            {
                Handles.DrawLine(node.transform.position, port.transform.position, WIRE_THICKNESS);
            }
            else if (node)
            {
                Handles.DrawLine(node.transform.position, mouseInWorld, WIRE_THICKNESS);
            }
            else if (port)
            {
                Handles.DrawLine(port.transform.position, mouseInWorld, WIRE_THICKNESS);
            }

            Handles.color = Color.white;

            bool wireButtonActive = node && port;

            _wireButton.SetEnabled(wireButtonActive);

            if (wireButtonActive && node.Outputs.Contains(port as Port))
            {
                _wireButton.text = "Unwire";
            }
            else
            {
                _wireButton.text = "Wire";
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

            var style = GetNodeStyle();

            var selectedNode = GetSelectedNode();
            var selectedPort = GetSelectedPort();

            foreach (var node in nodes)
            {
                var name = node.name;

                if (node.CanOutput())
                {
                    Handles.color = Color.yellow;
                    foreach (var output in node.Outputs)
                    {
                        Handles.DrawLine(node.transform.position, output.transform.position, WIRE_THICKNESS);
                    }
                    Handles.color = Color.white;
                }

                if (node.CanReceive())
                {
                    Handles.color = Color.red;

                    foreach (var receiver in node.Receivers)
                    {
                        Handles.DrawLine(node.transform.position, receiver.transform.position, WIRE_THICKNESS);
                    }

                    Handles.color = Color.white;
                }

                if (selectedNode != null && selectedNode != node)
                {
                    continue;
                }

                if (selectedPort != null && node.HasReceiver(selectedPort))
                {
                    continue;
                }

                var icon = EditorGUIUtility.GetIconForObject(node);

                GUIContent content = new(name, icon);

                bool button = DrawHandleButton(node.transform.position, content, style);

                if (button)
                {
                    if (selectedNode)
                    {
                        SelectNode(null);
                    }
                    else
                    {
                        SelectNode(node);
                    }
                    break;
                }
            }
        }

        private bool DrawHandleButton(Vector3 position, GUIContent content, GUIStyle style)
        {
            if (!(HandleUtility.WorldToGUIPointWithDepth(position).z < 0f))
            {
                Handles.BeginGUI();

                var rect = HandleUtility.WorldPointToSizedRect(position, content, style);
                rect.size *= ICON_SCALE;

                bool button = GUI.Button(rect, content, style);
                Handles.EndGUI();

                return button;
            }

            return false;
        }

        private void DrawPorts(SceneView sceneView)
        {
            var ports = Object.FindObjectsOfType<Port>();

            var style = GetPortStyle();

            var node = _nodeField.value as Node;

            var selectedPort = GetSelectedPort();

            foreach (var port in ports)
            {
                if (selectedPort != null && port != selectedPort)
                {
                    continue;
                }

                if (node && node.Receivers != null && node.Receivers.Contains(port))
                {
                    continue;
                }

                var name = port.name;

                var icon = EditorGUIUtility.GetIconForObject(port);

                GUIContent content = new(name, icon);

                bool button = DrawHandleButton(port.transform.position, content, style);

                if (button)
                {
                    if (selectedPort)
                    {
                        SelectPort(null);
                    }
                    else
                    {
                        SelectPort(port);
                    }
                    break;
                }
            }
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Circuit Board" };

            var panelAsset = Resources.Load<VisualTreeAsset>("UXML/CircuitBoardPanel");
            VisualElement ui = panelAsset.Instantiate();

            _nodeToggle = ui.Q<Toggle>("NodeToggle");
            _portToggle = ui.Q<Toggle>("PortToggle");

            _nodeField = ui.Q<ObjectField>("SelectedNode");
            _portField = ui.Q<ObjectField>("SelectedPort");

            _wireButton = ui.Q<Button>("WireButton");

            _wireButton.clicked += OnWireClick;

            root.Add(ui);

            return root;
        }

        private void OnWireClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            if (node.CanOutput() && !node.HasOutput(port))
            {
                node.AddOutput(port);

                EditorUtility.SetDirty(node);

                SelectNode(null);
                SelectPort(null);
            }
            else if (node.HasOutput(port))
            {
                node.RemoveOutput(port);

                EditorUtility.SetDirty(node);

                SelectNode(null);
                SelectPort(null);
            }
        }
    }
}
