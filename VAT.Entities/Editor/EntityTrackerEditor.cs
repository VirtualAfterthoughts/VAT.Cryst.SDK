using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAT.Entities.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(EntityTracker))]
    public class EntityTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tracker = (EntityTracker)target;

            var entity = tracker.GetComponentInParent<CrystEntity>(true);
            if (!entity)
            {
                EditorGUILayout.HelpBox("This EntityTracker is not part of a CrystEntity! Please add a CrystEntity to the root GameObject.", MessageType.Error);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Entity", entity, typeof(CrystEntity), true);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
