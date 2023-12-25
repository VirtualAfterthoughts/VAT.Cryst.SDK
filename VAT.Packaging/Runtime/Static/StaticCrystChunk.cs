using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class StaticCrystChunk
    {
        [SerializeField]
        private string _name = string.Empty;

        [SerializeField]
        private StaticCrystScene _scene;

        public StaticCrystScene Scene => _scene;

        public string ChunkName => _name;

        public StaticCrystChunk(string name, StaticCrystScene scene)
        {
            _name = name;
            _scene = scene;
        }

        public void GenerateName()
        {
#if UNITY_EDITOR
            if (Scene.EditorScene != null)
                _name = Scene.EditorScene.name;
            else
#endif
                _name = Guid.NewGuid().ToString();
        }
    }
}
