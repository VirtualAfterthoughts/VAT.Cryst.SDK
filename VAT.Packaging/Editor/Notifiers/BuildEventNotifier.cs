using System.Collections;
using System.Collections.Generic;

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace VAT.Packaging.Editor
{
    public class BuildEventNotifier : IPreprocessBuildWithReport
    {
        // This should be called first. We want all assets to be packed and ready.
        public int callbackOrder => -10000;

        public void OnPreprocessBuild(BuildReport report) {
            // We want to make sure game assets are packed before built
            InternalAssetPacker.PackTextAssets();
        }
    }
}
