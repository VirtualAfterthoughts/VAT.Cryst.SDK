using UnityEngine;

namespace VAT.Props.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(Chamber))]
    [CanEditMultipleObjects]
    public class ChamberEditor : Editor
    {
        private void OnSceneGUI()
        {
            var chamber = (Chamber)target;

            // Direction arrows
            var start = chamber.CartridgeTarget.position;

            var direction = chamber.CartridgeTarget.rotation * chamber.EjectDirection;
            var rotation = Quaternion.LookRotation(direction, chamber.CartridgeTarget.up);

            var end = start + direction * 0.1f;

            Handles.color = Color.cyan;
            Handles.DrawLine(start, end, 0.15f);

            Handles.ConeHandleCap(0, end, rotation, 0.02f, EventType.Repaint);

            Handles.color = Color.black;
            Handles.DrawLine(start, end, 0.1f);

            Handles.ConeHandleCap(0, end, rotation, 0.017f, EventType.Repaint);

            EditorGUI.BeginChangeCheck();

            Handles.color = Color.cyan;

            var newRotation = Handles.FreeRotateHandle(rotation, start, 0.02f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(chamber, "Modified Eject Direction");

                chamber.EjectDirection = Quaternion.Inverse(chamber.CartridgeTarget.rotation) * (newRotation * Vector3.forward);
            }
        }
    }
}
