using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Characters.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(IAvatarAbility), true)]
    public class AvatarAbilityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var ability = target as IAvatarAbility;
        }
    }
}
