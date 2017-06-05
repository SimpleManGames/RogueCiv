using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    private UpdatableData data;
    private UpdatableData Data
    {
        get
        {
            if (data == null)
                data = (UpdatableData)target;

            return data;
        }

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update"))
        {
            Data.NotifyOfUpdateValues();
            EditorUtility.SetDirty(target);
        }
    }
}