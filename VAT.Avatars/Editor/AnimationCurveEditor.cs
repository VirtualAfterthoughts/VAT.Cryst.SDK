using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace VAT.Avatars.Editor
{
    public sealed class AnimationCurveEditor : EditorWindow
    {
        private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        private readonly List<bool> _foldoutGroup = new();

        [MenuItem("VAT/Cryst SDK/Animation/Animation Curve Visualizer")]
        public static void Initialize() {
            AnimationCurveEditor window = GetWindow<AnimationCurveEditor>(true, "Animation Curve Visualizer");
            window.Show();
        }

        public void OnGUI() {
            EditorGUILayout.LabelField("Animation Curve", EditorStyles.whiteLargeLabel);

            _curve = EditorGUILayout.CurveField(_curve);

            var keys = _curve.keys;

            EditorGUILayout.LabelField("Keys", EditorStyles.whiteLargeLabel, GUILayout.Height(20f));

            for (var i = 0; i < _curve.length; i++) {
                EditorGUILayout.Space(5);

                DrawKey(i, keys[i], out var newKey);
                keys[i] = newKey;
            }

            _curve.keys = keys;

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("This tool allows you to preview an animation curve and each of its keys in editor so that it may be transferred to code.", MessageType.Info);
        }

        private void DrawKey(int index, Keyframe key, out Keyframe newKey) {
            while (_foldoutGroup.Count <= index) {
                _foldoutGroup.Add(false);
            }

            newKey = key;

            _foldoutGroup[index] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutGroup[index], $"Key {index}");
            if (_foldoutGroup[index]) {
                var time = EditorGUILayout.FloatField("Time", key.time);
                var value = EditorGUILayout.FloatField("Value", key.value);
                var inTangent = EditorGUILayout.FloatField("In Tangent", key.inTangent);
                var outTangent = EditorGUILayout.FloatField("Out Tangent", key.outTangent);
                var inWeight = EditorGUILayout.FloatField("In Weight", key.inWeight);
                var outWeight = EditorGUILayout.FloatField("Out Weight", key.outWeight);
                newKey = new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
