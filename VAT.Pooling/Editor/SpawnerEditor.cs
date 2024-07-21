using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Pooling.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(Spawner))]
    [CanEditMultipleObjects]
    public class SpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
