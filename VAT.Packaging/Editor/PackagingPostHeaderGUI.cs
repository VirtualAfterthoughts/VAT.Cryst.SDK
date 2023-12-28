using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace VAT.Packaging.Editor
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class PackagingPostHeaderGUI
    {
        private struct ContentIdentifier
        {
            public StaticContentIdentifierAttribute attribute;
            public Type contentType;

            public ContentIdentifier(StaticContentIdentifierAttribute attribute, Type contentType)
            {
                this.attribute = attribute;
                this.contentType = contentType;
            }
        }

        private static Dictionary<Type, List<ContentIdentifier>> _assetTypeToIdentifier;
        private static Dictionary<Object, StaticContent> _assetToContent;

        private static bool _ready = false;

        static PackagingPostHeaderGUI()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        private static void OnReady()
        {
            _ready = true;

            _package = AssetPackager.Instance.GetPackages().FirstOrDefault();

            _assetTypeToIdentifier = new();
            _assetToContent = new();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(typeof(StaticContent)))
                    {
                        var attribute = type.GetCustomAttribute<StaticContentIdentifierAttribute>();

                        if (attribute != null)
                        {
                            if (!_assetTypeToIdentifier.ContainsKey(attribute.mainAssetType))
                                _assetTypeToIdentifier[attribute.mainAssetType] = new();

                            _assetTypeToIdentifier[attribute.mainAssetType].Add(new ContentIdentifier(attribute, type));
                        }
                    }
                }
            }

            foreach (var content in AssetPackager.Instance.GetContents())
            {
                if (content is StaticContent staticContent && staticContent.StaticAsset.EditorAsset != null)
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
                    if (editor.targets.Length > 0)
                    {
                        foreach (var target in editor.targets)
                        {
                            OnDrawPersistentObject(target);
                        }
                    }
                }
            }
        }

        private static Package _package;

        private static void OnDrawPersistentObject(Object obj)
        {
            if (_assetToContent.TryGetValue(obj, out var content))
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.ObjectField(content.StaticPackage, content.StaticPackage.GetType(), false);

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
                    _package = (Package)EditorGUILayout.ObjectField(_package, typeof(Package), false);
                    _drawnPackage = true;
                }

                foreach (var group in pair.Value)
                {
                    if (GUILayout.Button($"Add {group.attribute.displayName} To Package"))
                    {
                        if (_package != null)
                        {
                            StaticContentCreationWizard.Initialize(_package, group.attribute, group.contentType, obj);
                        }
                    }
                }
            }
        }
    }
}
