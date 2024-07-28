using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using VAT.Cryst.Game;

namespace VAT.Logic.Editor
{
    [Overlay(typeof(SceneView), "Circuit Board")]
    [Icon(CIRCUIT_BOARD_ICON_PATH)]
    public class CircuitBoardPanel : Overlay
    {
        private const string ICON_FOLDER = CrystAssetManager.PROJECT_RELATIVE_FOLDER + "/Editor/Icons";
        private const string CIRCUIT_BOARD_ICON_PATH = ICON_FOLDER + "/circuit-board-icon.png";

        private VisualElement _panelElement = null;

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

            ValidateIcon();
        }

        private void ValidateIcon()
        {
            CrystAssetManager.EnsureCrystFolderExists(ICON_FOLDER);

            if (AssetDatabase.LoadAssetAtPath<Texture2D>(CIRCUIT_BOARD_ICON_PATH))
            {
                return;
            }

            var icon = Resources.Load<Texture2D>("Icons/circuit-board-icon");

            if (icon == null)
            {
                Debug.LogWarning("Circuit Board Panel tried copying its icon to a project path, but the icon was missing!");
                return;
            }

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(icon), CIRCUIT_BOARD_ICON_PATH);
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

        public INode GetSelectedNode()
        {
            if (_isCreateMenu)
            {
                return null;
            }

            return _nodeField.value as INode;
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
            if (!ShouldShow())
            {
                return;
            }

            if (_panelElement == null)
            {
                return;
            }

            var node = GetSelectedNode();
            var port = GetSelectedPort();

            bool hasSingle = !(node != null && port != null) && (node != null || port != null);

            if (hasSingle && !_isCreateMenu)
            {
                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!ShouldShow())
            {
                return;
            }

            if (_panelElement == null)
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

            if (node != null && port != null)
            {
                Handles.DrawLine(((MonoBehaviour)node).transform.position, port.transform.position, WIRE_THICKNESS);
            }
            else if (node != null)
            {
                Handles.DrawLine(((MonoBehaviour)node).transform.position, mouseInWorld, WIRE_THICKNESS);
            }
            else if (port)
            {
                Handles.DrawLine(port.transform.position, mouseInWorld, WIRE_THICKNESS);
            }

            Handles.color = Color.white;

            bool hasBoth = node != null && port != null;
            bool hasPort = false;

            if (hasBoth)
            {
                hasPort = (node is IReceiverNode receiver && receiver.Inputs.Contains(port)) || (node is IDonorNode donor && donor.Outputs.Contains(port));
            }

            bool outputActive = hasBoth && node is IDonorNode && !hasPort;
            bool inputActive = hasBoth && node is IReceiverNode && !hasPort;
            bool disconnectActive = hasPort;

            _wireOutputButton.style.display = outputActive ? DisplayStyle.Flex : DisplayStyle.None;
            _wireInputButton.style.display = inputActive ? DisplayStyle.Flex : DisplayStyle.None;
            _wireDisconnectButton.style.display = disconnectActive ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SelectNode(INode node)
        {
            var monoNode = (MonoBehaviour)node;

            _nodeField.value = monoNode;

            if (_isCreateMenu)
            {
                Selection.activeObject = monoNode;
            }
        }

        private void SelectPort(Port port)
        {
            _portField.value = port;

            if (_isCreateMenu)
            {
                Selection.activeObject = port;
            }
        }

        private void DrawNodes(SceneView sceneView)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();

            MonoBehaviour[] behaviours;

            if (currentStage != null)
            {
                behaviours = currentStage.FindComponentsOfType<MonoBehaviour>();
            }
            else
            {
                behaviours = Object.FindObjectsOfType<MonoBehaviour>();
            }

            List<INode> nodes = new();

            foreach (var behaviour in behaviours)
            {
                if (behaviour is INode node && behaviour is not Port)
                {
                    nodes.Add(node);
                }
            }

            var style = GetNodeStyle();

            var selectedNode = GetSelectedNode();

            foreach (var node in nodes)
            {
                var monoNode = (MonoBehaviour)node;
                var name = monoNode.name;

                if (node is IDonorNode donor)
                {
                    Handles.color = Color.yellow;
                    foreach (var output in donor.Outputs)
                    {
                        Handles.DrawLine(monoNode.transform.position, output.transform.position, WIRE_THICKNESS);
                    }
                    Handles.color = Color.white;
                }

                if (node is IReceiverNode receiver)
                {
                    Handles.color = Color.red;

                    foreach (var input in receiver.Inputs)
                    {
                        if (receiver == null)
                        { 
                            continue;
                        }

                        Handles.DrawLine(monoNode.transform.position, input.transform.position, WIRE_THICKNESS);
                    }

                    Handles.color = Color.white;
                }

                if (selectedNode != null && selectedNode != node)
                {
                    continue;
                }

                var icon = EditorGUIUtility.GetIconForObject(monoNode);

                GUIContent content = new(name, icon);

                bool button = DrawHandleButton(monoNode.transform.position, content, style);

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
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();

            Port[] ports;

            if (currentStage != null)
            {
                ports = currentStage.FindComponentsOfType<Port>();
            }
            else
            {
                ports = Object.FindObjectsOfType<Port>();
            }

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

        private bool ShouldShow()
        {
            return displayed;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Circuit Board" };

            var panelAsset = Resources.Load<VisualTreeAsset>("UXML/CircuitBoardPanel");
            VisualElement ui = panelAsset.Instantiate();

            _panelElement = ui;

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

            OnSwitchMode();
        }

        private void OnWiringMode()
        {
            _createElement.style.display = DisplayStyle.None;
            _wiringElement.style.display = DisplayStyle.Flex;

            _wiringMode.style.backgroundColor = _selectedColor;
            _createMode.style.backgroundColor = new StyleColor(StyleKeyword.Initial);

            _isCreateMenu = false;

            OnSwitchMode();
        }

        private void OnSwitchMode()
        {
            SelectNode(null);
            SelectPort(null);
        }

        private void OnWireOutputClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            if (node is IDonorNode donor && !donor.Outputs.Contains(port))
            {
                donor.AddOutput(port);

                EditorUtility.SetDirty((MonoBehaviour)node);

                SelectNode(null);
                SelectPort(null);
            }
        }

        private void OnWireInputClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            if (node is IReceiverNode receiver && !receiver.Inputs.Contains(port))
            {
                receiver.AddInput(port);

                EditorUtility.SetDirty((MonoBehaviour)node);

                SelectNode(null);
                SelectPort(null);
            }
        }

        private void OnWireDisconnectClick()
        {
            var node = GetSelectedNode();
            var port = GetSelectedPort();

            if (node is IDonorNode donor && donor.Outputs.Contains(port))
            {
                donor.RemoveOutput(port);

                EditorUtility.SetDirty((MonoBehaviour)node);

                SelectNode(null);
                SelectPort(null);
            }
            else if (node is IReceiverNode receiver && receiver.Inputs.Contains(port))
            {
                receiver.RemoveInput(port);

                EditorUtility.SetDirty((MonoBehaviour)node);

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
