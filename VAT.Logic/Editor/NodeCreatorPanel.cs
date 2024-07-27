using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Overlays;

using UnityEngine.UIElements;

using VAT.Cryst.Game;

namespace VAT.Logic.Editor
{
    using UnityEngine;

    [Overlay(typeof(SceneView), "Node Creator")]
    public class NodeCreatorPanel : Overlay
    {
        public static NodeCreatorPanel Instance { get; private set; }

        private ScrollView _scrollView = null;
        private string _searchQuery = string.Empty;

        public override void OnCreated()
        {
            base.OnCreated();

            Instance = this;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            Instance = null;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "Node Creator" };

            var panelAsset = Resources.Load<VisualTreeAsset>("UXML/NodeCreatorPanel");
            VisualElement ui = panelAsset.Instantiate();

            root.Add(ui);

            var searchBar = ui.Q<TextField>("SearchBar");

            searchBar.RegisterValueChangedCallback((v) =>
            {
                ApplySearchQuery(v.newValue);
            });

            _scrollView = ui.Q<ScrollView>("NodeScrollView");

            PopulateScrollView(_scrollView);

            Selection.selectionChanged += OnSelectionChanged;

            return root;
        }

        private void OnSelectionChanged()
        {
            var activeObject = Selection.activeGameObject;

            if (activeObject == null)
            {
                return;
            }

            if (!activeObject.scene.IsValid())
            {
                return;
            }

            if (PrefabUtility.IsOutermostPrefabInstanceRoot(activeObject) && activeObject.GetComponent<INode>() != null && activeObject.name.Contains("Template"))
            {
                activeObject.name = activeObject.name.Replace("(Template)", string.Empty);
                PrefabUtility.UnpackPrefabInstance(activeObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                EditorUtility.SetDirty(activeObject);
            }
        }

        private void ApplySearchQuery(string query)
        {
            var labels = _scrollView.Query<Label>();
            _searchQuery = query;

            labels.ForEach(ValidateLabel);
        }

        private void ValidateLabel(Label label)
        {
            if (string.IsNullOrWhiteSpace(label.tooltip))
            {
                return;
            }

            var tooltip = label.tooltip.ToLower();
            bool isValid = tooltip.Contains(_searchQuery.ToLower());

            label.parent.style.display = isValid ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private List<MonoScript> GetNodeScripts()
        {
            var monoScripts = MonoImporter.GetAllRuntimeMonoScripts();
            List<MonoScript> nodeScripts = new();

            var nodeType = typeof(INode);
            var portType = typeof(Port);

            foreach (var script in monoScripts)
            {
                var type = script.GetClass();

                if (type == null)
                {
                    continue;
                }

                // Don't add ports
                if (portType.IsAssignableFrom(type))
                {
                    continue;
                }

                if (!type.IsAbstract && nodeType.IsAssignableFrom(type)) 
                {
                    nodeScripts.Add(script);
                }
            }

            return nodeScripts;
        }

        private void PopulateScrollView(ScrollView scrollView)
        {
            var nodeBox = scrollView.Q<GroupBox>("NodeBox");

            scrollView.Add(nodeBox);

            var nodeScripts = GetNodeScripts();

            nodeScripts.Sort((x, y) => x.name.CompareTo(y.name));

            for (var i = 0; i < nodeScripts.Count; i++)
            {
                var label = CreateNodeLabel(nodeScripts[i]);

                nodeBox.Add(label);
            }
        }

        private Label CreateNodeLabel(MonoScript nodeScript)
        {
            var backgroundLabel = new Label();

            backgroundLabel.style.height = 50;
            backgroundLabel.style.width = 50;
            backgroundLabel.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            backgroundLabel.style.marginBottom = 2;
            backgroundLabel.style.marginLeft = 2;
            backgroundLabel.style.marginRight = 2;
            backgroundLabel.style.marginTop = 2;
            backgroundLabel.style.alignItems = Align.Center;
            backgroundLabel.pickingMode = PickingMode.Ignore;

            var label = new Label();
            label.style.height = 32;
            label.style.width = 32;
            label.style.alignContent = Align.Center;
            label.style.marginBottom = 8;
            label.style.marginLeft = 8;
            label.style.marginRight = 8;
            label.style.marginTop = 8;

            var icon = EditorGUIUtility.GetIconForObject(nodeScript);
            label.style.backgroundImage = icon;

            var name = ObjectNames.NicifyVariableName(nodeScript.name);

            label.tooltip = name;

            label.RegisterCallback<MouseDownEvent>((e) =>
            {
                DragAndDrop.PrepareStartDrag();

                var dragPrefab = GetNodePrefab(nodeScript.GetClass());

                DragAndDrop.StartDrag("Drag Node");

                DragAndDrop.objectReferences = new Object[] { dragPrefab };
            });

            label.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });

            backgroundLabel.Add(label);

            return backgroundLabel;
        }

        private GameObject GetNodePrefab(Type type)
        {
            string name = $"{ObjectNames.NicifyVariableName(type.Name)} (Template)";

            var folder = "Editor/Templates";
            var path = CrystAssetManager.GetCrystRelativePath($"{folder}/{name}.prefab");

            CrystAssetManager.EnsureCrystFolderExists(CrystAssetManager.GetCrystRelativePath(folder));
            
            var loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (loadedGameObject != null)
            {
                return loadedGameObject;
            }

            loadedGameObject = CreateNodePrefab(type, name, path);
            return loadedGameObject;

        }

        private GameObject CreateNodePrefab(Type type, string name, string path)
        {
            GameObject instance = new(name);
            instance.AddComponent(type);

            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
            GameObject.DestroyImmediate(instance);

            return prefab;
        }
    }
}
