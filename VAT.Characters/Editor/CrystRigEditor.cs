using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace VAT.Characters.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(CrystRig), true)]
    public class CrystRigEditor : Editor
    {
        private RequireRig[] _requiredRigs = new RequireRig[0];

        private void Awake()
        {
            _requiredRigs = target.GetType().GetCustomAttributes<RequireRig>().ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var rig = target as CrystRig;

            var rigManager = rig.GetComponentInParent<ICrystRigManager>(true);

            if (rigManager == null)
            {
                EditorGUILayout.HelpBox("This Rig is not a part of a RigManager! Please move it under one, otherwise it will not function!", MessageType.Error);
                return;
            }

            var rigs = rigManager.GetRigs();

            foreach (var required in _requiredRigs)
            {
                var requiredType = required.requiredRig;

                bool hasRig = false;

                foreach (var otherRig in rigs)
                {
                    var otherType = otherRig.GetType();

                    if (requiredType.IsAssignableFrom(otherType))
                    {
                        hasRig = true;
                        break;
                    }
                }

                if (!hasRig)
                {
                    EditorGUILayout.HelpBox($"This Rig requires the RigManager to have a Rig of type {required.requiredRig.Name}! Please add one to the RigManager!", MessageType.Warning);
                }
            }
        }
    }
}
