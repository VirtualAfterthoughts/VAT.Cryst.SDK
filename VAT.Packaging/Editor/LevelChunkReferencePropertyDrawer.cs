using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using VAT.Shared.Math;

namespace VAT.Packaging.Editor
{
    [CustomPropertyDrawer(typeof(LevelChunkReference), true)]
    public class LevelChunkReferencePropertyDrawer : PropertyDrawer
    {
        private const int _outsideLines = 1;
        private const int _expandedLines = 2;

        private static Rect GetPropertyRect(Rect position, ref int lines)
        {
            return new Rect(position.min.x, position.min.y + lines++ * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.size.x, EditorGUIUtility.singleLineHeight);
        }

        private bool OnDrawChunk(Rect position, SerializedProperty contentIdProperty, SerializedProperty chunkNameProperty)
        {
            if (AssetPackager.IsReady && AssetPackager.Instance.TryGetContent<StaticLevelContent>(contentIdProperty.stringValue, out var content))
            {
                if (content.Chunks.Count > 0)
                {
                    string[] chunks = new string[content.Chunks.Count + 1];
                    chunks[0] = "None";
                    int selectedChunk = 0;

                    for (var i = 0; i < content.Chunks.Count; i++)
                    {
                        var name = content.Chunks[i].ChunkName;

                        chunks[i + 1] = name;

                        if (name == chunkNameProperty.stringValue)
                        {
                            selectedChunk = i + 1;
                        }
                    }

                    EditorGUI.BeginChangeCheck();

                    selectedChunk = EditorGUI.Popup(position, selectedChunk, chunks);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (selectedChunk <= 0)
                        {
                            chunkNameProperty.stringValue = string.Empty;
                        }
                        else
                        {
                            chunkNameProperty.stringValue = content.Chunks[selectedChunk - 1].ChunkName;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var contentProperty = property.FindPropertyRelative("_contentReference");
            var contentIdProperty = contentProperty.FindPropertyRelative("_address").FindPropertyRelative("_id");

            var chunkNameProperty = property.FindPropertyRelative("_chunkName");

            EditorGUI.BeginProperty(position, label, property);
            int lines = 0;
            Rect rectFoldout = GetPropertyRect(position, ref lines);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label, true);
            var indent = EditorGUI.indentLevel;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel = indent + 1;

                Rect rectType = GetPropertyRect(position, ref lines);
                EditorGUI.PropertyField(rectType, contentProperty);

                var chunkPosition = GetPropertyRect(position, ref lines);
                if (!OnDrawChunk(chunkPosition, contentIdProperty, chunkNameProperty))
                {
                    EditorGUI.LabelField(chunkPosition, "No Chunks");
                }
            }
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = _outsideLines;

            if (property.isExpanded)
            {
                lines += _expandedLines;
            }

            return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }
    }
}
