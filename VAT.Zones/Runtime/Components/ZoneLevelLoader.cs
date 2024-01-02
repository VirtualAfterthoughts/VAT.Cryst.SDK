using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using VAT.Packaging;
using VAT.Scene;

namespace VAT.Zones
{
    public sealed class ZoneLevelLoader : ZoneComponent
    {
        [SerializeField]
        private LevelContentReference _levelContentReference;

        [SerializeField]
        private LevelContentReference _loadLevelContentReference;

        [SerializeField]
        private UnityEvent<ILevelContent> _onLoadLevel;

        public void LoadLevel()
        {
            if (_levelContentReference.TryGetContent(out var content))
            {
                _onLoadLevel?.Invoke(content);

                CrystSceneManager.LoadLevel(_levelContentReference, _loadLevelContentReference);
            }
            else
            {
                Debug.LogError("Failed to get level content for ZoneLevelLoader!", this);
            }
        }
    }
}
