using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#if UNITY_EDITOR

using UnityEditor.AddressableAssets.Settings;

#endif
using UnityEngine;

namespace VAT.Cryst.Addressables
{
#if UNITY_EDITOR
    [DisplayName("Persistent Group")]
    public class PersistentGroupSchema : AddressableAssetGroupSchema
    {
    }

#endif
}
