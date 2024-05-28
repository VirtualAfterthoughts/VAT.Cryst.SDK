using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;

using UnityEditor;
using UnityEngine;

using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Interaction
{
    [Serializable]
    public class SlotShapeReference : ShardReferenceT<SlotShape> { }

    [DisplayName("Slot Shape")]
    public class SlotShape : DataShard
    {
        public bool MatchesShape(SlotShape other)
        {
            return this == other;
        }

        protected override void OnPack(JSONPacker packer, JObject json)
        {
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
        }

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
        }
#endif
    }
}
