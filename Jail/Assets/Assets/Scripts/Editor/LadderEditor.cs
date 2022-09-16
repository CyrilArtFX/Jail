using UnityEngine;
using UnityEditor;


//[CustomEditor(typeof(Ladder))]
public class LadderEditor : Editor
{
    Ladder ladder;

    public override void OnInspectorGUI()
    {
        ladder = (Ladder)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Build Ladder"))
        {
            ladder.BuildLadder();
        }
    }
}
