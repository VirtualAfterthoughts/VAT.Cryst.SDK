using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VAT.Cryst.Game;

namespace VAT.Logic.Editor
{
    [Overlay(typeof(SceneView), "Circuit Board")]
    public class CircuitBoardPanel : Overlay
    {
        private Toggle _nodeToggle = null;
        private Toggle _portToggle = null;

        private ObjectField _nodeField = null;
        private ObjectField _portField = null;

        private VisualElement _wiringElement = null;
        private VisualElement _createElement = null;

        private Button _wiringMode = null;
        private Button _createMode = null;

        private Button _wireOutputButton = null;
        private Button _wireInputButton = null;
        private Button _wireDisconnectButton = null;

        private Button _newNodeButton = null;
        private Label _newPortLabel = null;

        private bool _isCreateMenu = false;

        public const float WIRE_THICKNESS = 5f;

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

            return style;
        }

        private GUIStyle GetPortStyle()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            style.normal.textColor = Color.yellow;

            return style;
        }

        public Node GetSelectedNode()
        {
            if (_isCreateMenu)
            {
                return null;
            }

            return _nodeField.value as Node;
        }

        public Port GetSelectedPort()
        {
            if (_isCreateMenu)
            {
                return null;
            }

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

            if (hasSingle && !_isCreateMenu)
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

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
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

            if (!_isCreateMenu)
            {
                DrawWiring();
            }
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

            bool hasBoth = node && port;
            bool hasPort = false;

            if (hasBoth)
            {
                hasPort = node.HasReceiver(port) || node.HasOutput(port);
            }

            bool outputActive = hasBoth && node.CanOutput() && !hasPort;
            bool inputActive = hasBoth && node.CanReceive() && !hasPort;
            bool disconnectActive = hasPort;

            _wireOutputButton.style.display = outputActive ? DisplayStyle.Flex : DisplayStyle.None;
            _wireInputButton.style.display = inputActive ? DisplayStyle.Flex : DisplayStyle.None;
            _wireDisconnectButton.style.display = disconnectActive ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SelectNode(Node node)
        {
            _nodeField.value = node;

            Selection.activeObject = node;
        }

        private void SelectPort(Port port)
        {
            _portField.value = port;

            Selection.activeObject = port;
        }

        private void DrawNodes(SceneView sceneView)
        {
            var nodes = Object.FindObjectsOfType<Node>();

            var style = GetNodeStyle();

            var selectedNode = GetSelectedNode();

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
                        if (receiver == null)
                        { 
                            continue;
                        }

                        Handles.DrawLine(node.transform.position, receiver.transform.position, WIRE_THICKNESS);
                    }

                    Handles.color = Color.white;
                }

                if (selectedNode != null && selectedNode != node)
                {
                    continue;
                }

                var icon = EditorGUIUtility.GetIconForObject(node);

                GUIContent content = new(name, icon);

                bool button = DrawHandleButton(node.transform.position, content, style);

                if (button)
                {
                    if (selectedNode == node)
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

            var selectedPort = GetSelectedPort();

            foreach (var port in ports)
            {
                if (selectedPort != null && port != selectedPort)
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

            _wireOutputButton = ui.Q<Button>("WireOutputButton");
            _wireInputButton = ui.Q<Button>("WireInputButton");
            _wireDisconnectButton = ui.Q<Button>("WireDisconnectButton");

            _wireOutputButton.clicked += OnWireOutputClick;
            _wireInputButton.clicked += OnWireInputClick;
            _wireDisconnectButton.clicked += OnWireDisconnectClick;

            _createMode = ui.Q<Button>("CreateMode");
            _createMode.clicked += OnCreateMode;

            _wiringMode = ui.Q<Button>("WiringMode");
            _wiringMode.clicked += OnWiringMode;

            _createElement = ui.Q<VisualElement>("CreateElement");
            _wiringElement = ui.Q<VisualElement>("WiringElement");

            _newNodeButton = ui.Q<Button>("NewNode");
            _newPortLabel = ui.Q<Label>("NewPortLabel");

            _newNodeButton.clicked += () =>
            {
                NodeCreatorPanel.Instance.displayed = !NodeCreatorPanel.Instance.displayed;
            };

            _newPortLabel.RegisterCallback<MouseDownEvent>((e) =>
            {
                DragAndDrop.PrepareStartDrag();

                var dragPrefab = GetPortPrefab();

                DragAndDrop.StartDrag("Drag Port");

                DragAndDrop.objectReferences = new Object[] { dragPrefab };
            });

            _newPortLabel.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });

            Selection.selectionChanged += () =>
            {
                var selected = Selection.activeGameObject;

                if (selected != null && PrefabUtility.IsOutermostPrefabInstanceRoot(selected) && selected.name.StartsWith("Port (Template)") && selected.GetComponent<Port>())
                {
                    selected.name = "Port";
                    PrefabUtility.UnpackPrefabInstance(selected, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    EditorUtility.SetDirty(selected);
                }
            };

            root.Add(ui);

            OnCreateMode();

            return root;
        }

        private static readonly Color _selectedColor = new(0.27f, 0.37f, 0.5f);

        private void OnCreateMode()
        {
            _createElement.style.display = DisplayStyle.Flex;
            _wiringElement.style.display = DisplayStyle.None;

            _createMode.style.backgroundColor = _selectedColor;
            _wiringMode.style.backgroundColor = new StyleColor(StyleKeyword.Initial);

            _isCreateMenu = true;
        }

        private void OnWiringMode()
        {
            _createElement.style.display = DisplayStyle.None;
            _wiringElement.style.display = DisplayStyle.Flex;

            _wiringMode.style.backgroundColor = _selectedColor;
            _createMode.style.backgroundColor = new StyleColor(StyleKeyword.Initial);

            _isCreateMenu = false;
        }

        private void OnWireOutputClick()
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
        }

        private void OnWireInputClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();


            if (node.CanReceive() && !node.HasReceiver(port))
            {
                node.AddReceiver(port);

                EditorUtility.SetDirty(node);

                SelectNode(null);
                SelectPort(null);
            }
        }

        private void OnWireDisconnectClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            if (node.HasOutput(port))
            {
                node.RemoveOutput(port);

                EditorUtility.SetDirty(node);

                SelectNode(null);
                SelectPort(null);
            }
            else if (node.HasReceiver(port))
            {
                node.RemoveReceiver(port);

                EditorUtility.SetDirty(node);

                SelectNode(null);
                SelectPort(null);
            }
        }

        private GameObject GetPortPrefab()
        {
            string name = $"Port (Template)";

            var folder = "Editor/Templates";
            var path = CrystAssetManager.GetCrystRelativePath($"{folder}/{name}.prefab");

            CrystAssetManager.EnsureCrystFolderExists(CrystAssetManager.GetCrystRelativePath(folder));

            var loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (loadedGameObject != null)
            {
                return loadedGameObject;
            }

            loadedGameObject = CreatePortPrefab(name, path);
            return loadedGameObject;

        }

        private GameObject CreatePortPrefab(string name, string path)
        {
            GameObject instance = new(name);
            instance.AddComponent<Port>();

            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
            GameObject.DestroyImmediate(instance);

            return prefab;
        }
    }
}
