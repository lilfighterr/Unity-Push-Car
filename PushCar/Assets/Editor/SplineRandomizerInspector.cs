using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SplineRandomizer))]
public class SplineRanzomizerInspector : Editor
{
    private SplineRandomizer splineRandom;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        splineRandom = target as SplineRandomizer;

        if (GUILayout.Button("Randomize"))
        {
            Undo.RecordObject(splineRandom, "Randomize");
            splineRandom.Randomize();
            EditorUtility.SetDirty(splineRandom);
        }
    }
}
