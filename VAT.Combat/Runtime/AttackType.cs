using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using UnityEditor;
using UnityEngine;

using VAT.Packaging;
using VAT.Serialization.JSON;

namespace VAT.Combat
{
    [DisplayName("Attack Type")]
    public class AttackType : DataShard
    {
#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            throw new System.NotImplementedException();
        }
#endif

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            throw new System.NotImplementedException();
        }
    }
}
