using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel;

using UnityEditor;

using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Props
{
    [Serializable]
    public class CartridgeDataReference : ShardReferenceT<CartridgeData> { }

    [DisplayName("Cartridge Data")]
    public class CartridgeData : DataShard
    {
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
