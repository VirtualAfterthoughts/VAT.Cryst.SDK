using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using VAT.Packaging;

using VAT.Serialization.JSON;

using VAT.Shared.Extensions;

namespace VAT.Audio
{
    [DataShardIdentifier("Audio Collection")]
    public class AudioCollection : DataShard
    {
        [SerializeField]
        private AudioClipShardReference[] _audioClipReferences; 

        public AudioClipShardReference GetRandomAudioClip()
        {
            return _audioClipReferences.GetRandom();
        }

#if UNITY_EDITOR
        public override void OnEditorInspectorGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_audioClipReferences)));
        }
#endif

        protected override void OnPack(JSONPacker packer, JObject json)
        {
            json.Add("audioClips", JArray.FromObject(_audioClipReferences));
        }

        protected override void OnUnpack(JSONUnpacker unpacker, JObject json)
        {
            if (json.TryGetValue("audioClips", out var audioToken))
            {
                _audioClipReferences = audioToken.ToObject<AudioClipShardReference[]>();
            }
        }
    }
}
