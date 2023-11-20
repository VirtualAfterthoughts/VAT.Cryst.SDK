using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace VAT.Packaging.Editor
{
    [InitializeOnLoad]
    public static class PlayModeNotifier
    {
        static PlayModeNotifier()
        {
            EditorApplication.playModeStateChanged += Fire;
        }

        private static void Fire(PlayModeStateChange state)
        {
            switch (state)
            {
                // Refresh the AssetPackager after entering edit mode
                case PlayModeStateChange.EnteredEditMode:
                    Debug.Log("Entered Edit Mode, refreshing AssetPackager.");
                    AssetPackager.EditorForceRefresh();
                    break;
                // Before entering Play mode, pack all game assets
                case PlayModeStateChange.ExitingEditMode:
                    InternalAssetPacker.PackTextAssets(true);
                    break;
            }
        }
    }
}
