using System.Collections;
using System.Collections.Generic;

namespace VAT.Entities.Editor
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using VAT.Entities.PhysX;

    [InitializeOnLoad]
    public static class CrystallizationWizard
    {
        static CrystallizationWizard()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        private static void OnPostHeaderGUI(Editor editor)
        {
            using (new GUILayout.VerticalScope())
            {
                if (editor.targets.Length > 0)
                {
                    foreach (var target in editor.targets)
                    {
                        if (target is not GameObject go)
                            continue;

                        if (PrefabStageUtility.GetPrefabStage(go) == null && !EditorUtility.IsPersistent(go))
                            continue;

                        if (go.transform.parent != null)
                            continue;

                        if (IsCrystallized(go, out var entity))
                        {
                            if (GUILayout.Button("Dissolve"))
                            {
                                Dissolve(entity);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Crystallize"))
                            {
                                Crystallize(go);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsCrystallized(GameObject go, out CrystEntity entity)
        {
            entity = go.GetComponentInChildren<CrystEntity>(true);
            return entity != null;
        }

        private static void Crystallize(GameObject go)
        {
            var activeEntity = go.GetComponentInChildren<CrystEntity>(true);
            if (activeEntity != null)
            {
                Debug.LogWarning($"GameObject {go.name} is already Crystallized, dissolving...");
                Dissolve(activeEntity);
            }

            Debug.Log($"Crystallizing {go.name}");

            Undo.RegisterFullObjectHierarchyUndo(go, $"Crystallized {go.name}");

            go.AddComponent<CrystEntity>();

            foreach (var rigidbody in go.GetComponentsInChildren<Rigidbody>(true))
            {
                rigidbody.gameObject.AddComponent<CrystRigidbody>();
            }

            foreach (var joint in go.GetComponentsInChildren<ConfigurableJoint>(true))
            {
                joint.gameObject.AddComponent<CrystConfigurableJoint>();
            }
        }

        private static void Dissolve(CrystEntity entity)
        {
            var go = entity.gameObject;

            Debug.Log($"Dissolving {go.name}");

            Undo.RegisterFullObjectHierarchyUndo(go, $"Dissolved {go.name}");

            GameObject.DestroyImmediate(entity);

            foreach (var joint in go.GetComponentsInChildren<CrystJoint>(true))
            {
                GameObject.DestroyImmediate(joint);
            }

            foreach (var body in go.GetComponentsInChildren<CrystBody>(true))
            {
                GameObject.DestroyImmediate(body);
            }
        }
    }
}
