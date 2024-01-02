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
        private LevelContentReference _level;

        [SerializeField]
        private LevelContentReference _loadLevel;

        [SerializeField]
        private UnityEvent<ILevelContent> _onLoadLevel;

        private bool _hasLoadedLevel = false;

        public override void OnZoneEnabled()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            if (_hasLoadedLevel)
            {
                return;
            }

            if (_level.TryGetContent(out var content))
            {
                _onLoadLevel?.Invoke(content);

                var options = new SceneLoadOptions()
                {
                    loadLevel = _loadLevel,
                };

                CrystSceneManager.LoadLevel(_level, options);

                _hasLoadedLevel = true;
            }
            else
            {
                Debug.LogError("Failed to get level content for ZoneLevelLoader!", this);
            }
        }
    }
}
