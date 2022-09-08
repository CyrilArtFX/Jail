using UnityEditor;
using UnityEngine;

using Jail.Utility.Bezier;

namespace Jail.Debug
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : Editor
    {
        const int STEPS_PER_CURVE = 10;

        const float HANDLE_SIZE = 0.04f;
        const float PICK_SIZE = 0.06f;

        int selectedId = -1;

        BezierSpline spline;
        Transform handleTransform;
        Quaternion handleRotation;

        Color[] modesColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan,
        };

        public override void OnInspectorGUI()
        {
            spline = (BezierSpline) target;

            //  show selected point
            if (selectedId >= 0 && selectedId <= spline.ControlPointCount)
            {
                DrawSelectedPointInspector();
            }

            //  add curve button
            if (GUILayout.Button("Add Curve"))
            {
                //  record undo
                Undo.RecordObject(spline, "Add Curve");

                //  set object as dirty
                EditorUtility.SetDirty(spline);

                //  add curve
                spline.AddCurve();
            }
        }

        void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");

            //  show position
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedId));
            if (EditorGUI.EndChangeCheck())
            {
                //  record undo
                Undo.RecordObject(spline, "Move Point");

                //  set object as dirty
                EditorUtility.SetDirty(spline);

                //  change point
                spline.SetControlPoint(selectedId, point);
            }

            //  show mode
            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedId));
            if (EditorGUI.EndChangeCheck())
            {
                //  record undo
                Undo.RecordObject(spline, "Change Point Mode");

                //  set object as dirty
                EditorUtility.SetDirty(spline);

                //  change point mode
                spline.SetControlPointMode(selectedId, mode);
            }
        }

        void OnSceneGUI()
        {
            spline = (BezierSpline) target;

            //  get transform
            handleTransform = spline.transform;

            //  get handle rotation depending on tool's pivot mode
            handleRotation = handleTransform.rotation;
            if (Tools.pivotRotation == PivotRotation.Global)
                handleRotation = Quaternion.identity;

            //  draw spline
            Vector3 last_point = DoHandlePoint(0);
            for (int i = 1; i < spline.ControlPointCount; i += 3)
            {
                //  get points
                Vector3 a = DoHandlePoint(i), 
                        b = DoHandlePoint(i + 1), 
                        c = DoHandlePoint(i + 2);
                
                //  draw lines
                Handles.color = Color.gray;
                Handles.DrawLine(last_point, a);
                Handles.DrawLine(b, c);

                //  draw bezier curve
                Handles.DrawBezier(last_point, c, a, b, Color.white, null, 3.0f);
                last_point = c;
            }

            //  draw directions
            //ShowDirections();
        }

        void ShowDirections()
        {
            float steps = STEPS_PER_CURVE * spline.CurveCount;

            //  change color
            Handles.color = Color.green;

            //  draw first direction
            Vector3 point = spline.GetPoint(0.0f);
            Handles.DrawLine(point, point + spline.GetDirection(0.0f));

            //  draw all directions
            for (int i = 1; i <= steps; i++)
            {
                float next_step = i / steps;
                point = spline.GetPoint(next_step);
                Handles.DrawLine(point, point + spline.GetDirection(next_step));
            }
        }

        Vector3 DoHandlePoint(int id)
        {
            Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(id));

            //  attempt a selection
            float scale = HandleUtility.GetHandleSize(point);
            Handles.color = modesColors[(int) spline.GetControlPointMode(id)];
            if (Handles.Button(point, handleRotation, scale * HANDLE_SIZE, scale * PICK_SIZE, Handles.DotHandleCap))
            {
                selectedId = id;
                Repaint();  //  update inspector
            }

            //  move selected
            if (selectedId == id)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    //  record undo
                    Undo.RecordObject(spline, "Move Point");
                
                    //  set object as dirty
                    EditorUtility.SetDirty(spline);

                    //  change point
                    spline.SetControlPoint(id, handleTransform.InverseTransformPoint(point));
                }
            }

            return point;
        }
    }
}