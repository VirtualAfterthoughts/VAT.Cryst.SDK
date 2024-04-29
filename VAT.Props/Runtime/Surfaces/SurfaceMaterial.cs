using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Props
{
    [DataShardIdentifier("Surface Material")]
    public class SurfaceMaterial : DataShard
    {
        [SerializeField]
        [Min(0f)]
        [Tooltip("The density of the surface material.")]
        private float _density = 1f;

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_density)));
        }
#endif

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("density", _density);
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("density", out var density))
            {
                _density = density.ToObject<float>();
            }
        }
    }
}
