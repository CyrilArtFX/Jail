using System;
using UnityEngine;
using UnityEngine.Events;

namespace Jail.Utility.Bezier
{
    public class BezierSpline : MonoBehaviour
    {
        const float STEPS = 1.0f / 30.0f;

        [SerializeField]
        Vector3[] points;
        [SerializeField]
        BezierControlPointMode[] modes;

        public UnityEvent OnSplineEdited;

        public float Length { get; private set; }
        public int ControlPointCount => points.Length;
        public int CurveCount => (points.Length - 1) / 3;

        public void UpdateSpline()
        {
            ComputeLength();

            OnSplineEdited.Invoke();
        }

        public void ComputeLength()
        {
            Length = 0.0f;

            Vector3 last_point = GetPoint(0.0f);
            for (float t = 0.0f; t < 1.0f; t += STEPS)
            {
                Vector3 point = GetPoint(t);
                Length += Vector3.Distance(last_point, point);
                last_point = point;
            }
        }

        public Vector3 GetControlPoint(int id)
        {
            return points[id];
        }

        public void SetControlPoint(int id, Vector3 point)
        {
            //  move tangent points
            if (id % 3 == 0)
            {
                Vector3 delta = point - points[id];
                if (id > 0)
                {
                    points[id - 1] += delta;
                }
                else if (id + 1 < points.Length) 
                {
                    points[id + 1] += delta;
                }
            }

            //  set point
            points[id] = point;

            //  enforce mode
            EnforceMode(id);

            //  update
            UpdateSpline();
        }

        public BezierControlPointMode GetControlPointMode(int id)
        {
            return modes[(id + 1) / 3];
        }

        public void SetControlPointMode(int id, BezierControlPointMode mode)
        {
            //  set mode
            modes[(id + 1) / 3] = mode;

            //  enforce mode
            EnforceMode(id);

            //  update
            UpdateSpline();
        }

        void EnforceMode(int id)
        {
            int mode_id = (id + 1) / 3;
            if (mode_id == 0 || mode_id == modes.Length - 1) return;

            //  get current mode
            BezierControlPointMode mode = modes[mode_id];
            if (mode == BezierControlPointMode.Free) return;

            //  get indexes
            int middle_id = mode_id * 3;
            int fixed_id, enforced_id;
            if (id <= middle_id)
            {
                fixed_id = middle_id - 1;
                enforced_id = middle_id + 1;
            }
            else
            {
                fixed_id = middle_id + 1;
                enforced_id = middle_id - 1;
            }

            //  enforce mode
            Vector3 middle = points[middle_id];
            Vector3 enforced_tangent = middle - points[fixed_id];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforced_tangent = enforced_tangent.normalized * Vector3.Distance(middle, points[enforced_id]);
            }
            points[enforced_id] = middle + enforced_tangent;
        }

        public void AddCurve()
        {
            Vector3 point = points[points.Length - 1];

            #region AddPoints
            //  resize array
            Array.Resize(ref points, points.Length + 3);

            //  add new points
            point.x += 1.0f;
            points[points.Length - 3] = point;
            point.x += 1.0f;
            points[points.Length - 2] = point;
            point.x += 1.0f;
            points[points.Length - 1] = point;
            #endregion

            #region AddMode
            //  resize modes
            Array.Resize(ref modes, modes.Length + 1);

            //  add new mode
            modes[modes.Length - 1] = modes[modes.Length - 2];

            //  enforce mode
            EnforceMode(points.Length - 4);
            #endregion

            //  update
            UpdateSpline();
        }

        public void RemoveCurve()
        {
            if (points.Length - 3 <= 1) return;

            //  resize arrays
            Array.Resize(ref points, points.Length - 3);
            Array.Resize(ref modes, modes.Length - 1);

             //  update
            UpdateSpline();
        }

        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(2.0f, 0.0f, 0.0f),
                new Vector3(3.0f, 0.0f, 0.0f),
                new Vector3(4.0f, 0.0f, 0.0f)
            };

            modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free,
            };
        }

        int GetCurveIndexByTime(ref float t)
        {
            int i;
            if (t >= 1.0f)
            {
                t = 1.0f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return i;
        }

        public Vector3 GetPoint(float t)
        {
            int i = GetCurveIndexByTime(ref t);

            return transform.TransformPoint(
                Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t)
            );
        }

        public Vector3 GetVelocity(float t)
        {
            int i = GetCurveIndexByTime(ref t);

            return transform.TransformPoint(
                        Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)
                    ) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }
    }
}