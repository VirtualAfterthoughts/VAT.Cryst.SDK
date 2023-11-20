using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystScene : StaticCrystAsset
    {
        public StaticCrystScene(string guid) : base(guid) { }

#if UNITY_EDITOR
        public SceneAsset EditorScene { get { return _editorAsset as SceneAsset; } set { _editorAsset = value; } }
        public override Type AssetType => typeof(SceneAsset);
#else
        public override Type AssetType => typeof(Scene);
#endif
    }
}
