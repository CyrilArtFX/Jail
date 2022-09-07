using UnityEditor;
using UnityEngine;

using Jail.Environment;

namespace Jail.Debug
{
    [CustomEditor(typeof(BezierCurve))]
    public class CurvedLineInspector : Editor
    {
        const int iterSteps = 10;

        BezierCurve curve;
        Transform handleTransform;
        Quaternion handleRotation;

        void OnSceneGUI()
        {
            curve = (BezierCurve) target;

            //  get transform
            handleTransform = curve.transform;

            //  get handle rotation depending on tool's pivot mode
            handleRotation = handleTransform.rotation;
            if (Tools.pivotRotation == PivotRotation.Global)
                handleRotation = Quaternion.identity;

            Vector3 last_point = curve.GetPoint(0.0f);

            //  draw first velocity
            Handles.color = Color.green;
            Handles.DrawLine(last_point, last_point + curve.GetDirection(0.0f));

            //  draw curve
            for (int i = 1; i <= iterSteps; i++)
            {
                float steps = iterSteps;
                Vector3 point = curve.GetPoint(i / steps);

                //  draw line
                Handles.color = Color.white;
                Handles.DrawLine(last_point, point, 3.0f);
                last_point = point;

                //  draw velocity
                Handles.color = Color.green;
                Handles.DrawLine(last_point, point + curve.GetDirection(i / steps));
            }

            //  handle point
            DoHandlePoint(0);
            DoHandlePoint(1);
            DoHandlePoint(2);

            //  draw points handles
            /*Handles.color = Color.red;
            Handles.DrawWireCube(curve.A, new Vector3(0.1f, 0.1f, 0.1f));
            Handles.DrawWireCube(curve.B, new Vector3(0.1f, 0.1f, 0.1f));*/
        }

        Vector3 DoHandlePoint(int id)
        {
            Vector3 point = curve.points[id];

            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "MovePoint");
                EditorUtility.SetDirty(curve);
                curve.points[id] = handleTransform.InverseTransformPoint(point);
            }

            return point;
        }
    }
}