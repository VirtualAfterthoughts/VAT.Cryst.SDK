using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace VAT.Packaging.Editor
{
    using UnityEditor;
    using UnityEngine;
    using static UnityEngine.GraphicsBuffer;

    [InitializeOnLoad]
    public static class PackagingPostHeaderGUI
    {
        private struct ContentIdentifier
        {
            public StaticShardIdentifier attribute;
            public Type contentType;

            public ContentIdentifier(StaticShardIdentifier attribute, Type contentType)
            {
                this.attribute = attribute;
                this.contentType = contentType;
            }
        }

        private static Dictionary<Type, List<ContentIdentifier>> _assetTypeToIdentifier;
        private static Dictionary<Object, StaticShard> _assetToContent;

        private static bool _ready = false;

        static PackagingPostHeaderGUI()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        private static void OnReady()
        {
            _ready = true;

            _package = AssetPackager.Instance.GetCrystals().FirstOrDefault();

            _assetTypeToIdentifier = new();
            _assetToContent = new();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(typeof(StaticShard)))
                    {
                        var attribute = type.GetCustomAttribute<StaticShardIdentifier>();

                        if (attribute != null)
                        {
                            if (!_assetTypeToIdentifier.ContainsKey(attribute.mainAssetType))
                                _assetTypeToIdentifier[attribute.mainAssetType] = new();

                            _assetTypeToIdentifier[attribute.mainAssetType].Add(new ContentIdentifier(attribute, type));
                        }
                    }
                }
            }

            foreach (var content in AssetPackager.Instance.GetShards())
            {
                if (content is StaticShard staticContent && staticContent.StaticAsset.EditorAsset != null)
                {
                    var asset = staticContent.StaticAsset.EditorAsset;
                    _assetToContent[asset] = staticContent;
                }
            }
        }

        private static void OnPostHeaderGUI(Editor editor)
        {
            if (!AssetPackager.IsReady)
            {
                return;
            }
            else if (!_ready)
            {
                OnReady();
            }

            if (EditorUtility.IsPersistent(Selection.activeObject))
            {
                using (new GUILayout.VerticalScope())
                {
                    if (editor.targets.Length == 1)
                    {
                        OnDrawPersistentObject(editor.targets[0]);
                    }
                    else if (editor.targets.Length > 1)
                    {
                        OnDrawPersistentObjects(editor.targets);
                    }
                }
            }
        }

        private static Crystal _package;

        private static void OnDrawPersistentObjects(Object[] objects)
        {
            Type consistentType = null;

            foreach (var obj in objects)
            {
                var type = obj.GetType();

                if (consistentType == null)
                {
                    consistentType = type;
                }
                else if (consistentType != type)
                {
                    return;
                }

                if (type.IsSubclassOf(typeof(Shippable)))
                    return;
            }

            bool _drawnPackage = false;

            foreach (var pair in _assetTypeToIdentifier)
            {
                if (!consistentType.IsSubclassOf(pair.Key) && pair.Key != consistentType)
                    continue;

                if (!_drawnPackage)
                {
                    _package = (Crystal)EditorGUILayout.ObjectField(_package, typeof(Crystal), false);
                    _drawnPackage = true;
                }

                foreach (var group in pair.Value)
                {
                    if (GUILayout.Button($"Add {objects.Length} {group.attribute.displayName}s To Crystal"))
                    {
                        if (_package != null)
                        {
                            foreach (var obj in objects)
                            {
                                StaticShardCreationWizard.Initialize(_package, group.attribute, group.contentType, obj);
                            }
                        }
                    }
                }
            }
        }

        private static void OnDrawPersistentObject(Object obj)
        {
            if (_assetToContent.TryGetValue(obj, out var content))
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.ObjectField(content.StaticCrystal, content.StaticCrystal.GetType(), false);

                EditorGUILayout.ObjectField(content, content.GetType(), false);

                EditorGUI.EndDisabledGroup();

                return;
            }

            var type = obj.GetType();

            if (type.IsSubclassOf(typeof(Shippable)))
                return;

            bool _drawnPackage = false;

            foreach (var pair in _assetTypeToIdentifier)
            {
                if (!type.IsSubclassOf(pair.Key) && pair.Key != type)
                    continue;

                if (!_drawnPackage)
                {
                    _package = (Crystal)EditorGUILayout.ObjectField(_package, typeof(Crystal), false);
                    _drawnPackage = true;
                }

                foreach (var group in pair.Value)
                {
                    if (GUILayout.Button($"Add {group.attribute.displayName} To Crystal"))
                    {
                        if (_package != null)
                        {
                            StaticShardCreationWizard.Initialize(_package, group.attribute, group.contentType, obj);
                        }
                    }
                }
            }
        }
    }
}
