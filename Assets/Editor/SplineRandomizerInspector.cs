using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SplineRandomizer))]
public class SplineRanzomizerInspector : Editor
{
    private SplineRandomizer splineRandom;
    private BezierSpline spline;

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
        if (GUILayout.Button("Randomize Keep Start"))
        {
            Undo.RecordObject(splineRandom, "Randomize Keep Start");
            splineRandom.Randomize(true);
            EditorUtility.SetDirty(splineRandom);
        }
    }
}
