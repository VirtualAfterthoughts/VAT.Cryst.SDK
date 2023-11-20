using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public static class AssetUnlocker
    {
        public static bool IsUnlocked(IContent content)
        {
            return !content.ContentInfo.Unlockable;
        }

        public static void UnlockContent(IContent content)
        {
        }
        
        public static void LockContent(IContent content)
        {
        }
    }
}
