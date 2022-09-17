using UnityEditor;
using UnityEngine;

using Jail.Utility.Bezier;

namespace Jail.Unity
{
    [CustomEditor(typeof(BezierSplineChainer))]
    public class BezierSplineChainerInspector : Editor
    {
        BezierSplineChainer chainer;

        public override void OnInspectorGUI()
        {
            chainer = (BezierSplineChainer) target;

            //  draw default inspector
            DrawDefaultInspector();

            //  update
            if (GUILayout.Button("Update"))
            {
                chainer.DoUpdate();
            }

            //  delete generated items
            if (GUILayout.Button("Delete Generated Items"))
            {
                chainer.RemoveItemsAtRange(0, chainer.Items.Count - 1);
            }

            //  force update
            if (GUILayout.Button("Re-Generate All"))
            {
                chainer.RemoveItemsAtRange(0, chainer.Items.Count - 1);
                chainer.DoUpdate();
            }
        }
    }
}