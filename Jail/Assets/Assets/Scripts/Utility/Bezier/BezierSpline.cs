using System;
using UnityEngine;
using UnityEngine.Events;

namespace Jail.Utility.Bezier
{
    public class BezierSpline : MonoBehaviour
    {
        const float STEPS = 1.0f / 30.0f;

        public BezierSplineData Data => data;
        BezierSplineData data = new BezierSplineData();

        public UnityEvent OnSplineEdited;

        public float Length { get; private set; }
        public int ControlPointCount => data.points.Length;
        public int CurveCount => (data.points.Length - 1) / 3;

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
            return data.points[id];
        }

        public void SetControlPoint(int id, Vector3 point)
        {
            //  move tangent points
            if (id % 3 == 0)
            {
                Vector3 delta = point - data.points[id];
                if (id > 0)
                {
                    data.points[id - 1] += delta;
                }
                else if (id + 1 < data.points.Length) 
                {
                    data.points[id + 1] += delta;
                }
            }

            //  set point
            data.points[id] = point;

            //  enforce mode
            EnforceMode(id);

            //  update
            UpdateSpline();
        }

        public BezierControlPointMode GetControlPointMode(int id)
        {
            return data.modes[(id + 1) / 3];
        }

        public void SetControlPointMode(int id, BezierControlPointMode mode)
        {
            //  set mode
            data.modes[(id + 1) / 3] = mode;

            //  enforce mode
            EnforceMode(id);

            //  update
            UpdateSpline();
        }

        void EnforceMode(int id)
        {
            int mode_id = (id + 1) / 3;
            if (mode_id == 0 || mode_id == data.modes.Length - 1) return;

            //  get current mode
            BezierControlPointMode mode = data.modes[mode_id];
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
            Vector3 middle = data.points[middle_id];
            Vector3 enforced_tangent = middle - data.points[fixed_id];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforced_tangent = enforced_tangent.normalized * Vector3.Distance(middle, data.points[enforced_id]);
            }
            data.points[enforced_id] = middle + enforced_tangent;
        }

        public void AddCurve()
        {
            Vector3 point = data.points[data.points.Length - 1];

            #region AddPoints
            //  resize array
            Array.Resize(ref data.points, data.points.Length + 3);

            //  add new points
            point.x += 1.0f;
            data.points[data.points.Length - 3] = point;
            point.x += 1.0f;
            data.points[data.points.Length - 2] = point;
            point.x += 1.0f;
            data.points[data.points.Length - 1] = point;
            #endregion

            #region AddMode
            //  resize modes
            Array.Resize(ref data.modes, data.modes.Length + 1);

            //  add new mode
            data.modes[data.modes.Length - 1] = data.modes[data.modes.Length - 2];

            //  enforce mode
            EnforceMode(data.points.Length - 4);
            #endregion

            //  update
            UpdateSpline();
        }

        public void Reset()
        {
            data.points = new Vector3[]
            {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(2.0f, 0.0f, 0.0f),
                new Vector3(3.0f, 0.0f, 0.0f),
                new Vector3(4.0f, 0.0f, 0.0f)
            };

            data.modes = new BezierControlPointMode[]
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
                i = data.points.Length - 4;
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
                Bezier.GetPoint(data.points[i], data.points[i + 1], data.points[i + 2], data.points[i + 3], t)
            );
        }

        public Vector3 GetVelocity(float t)
        {
            int i = GetCurveIndexByTime(ref t);

            return transform.TransformPoint(
                        Bezier.GetFirstDerivative(data.points[i], data.points[i + 1], data.points[i + 2], data.points[i + 3], t)
                    ) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }
    }
}