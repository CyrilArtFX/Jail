using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Jail.Environment
{
    public class BezierCurve : MonoBehaviour
    {
        public Vector3[] points;

        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3( 1.0f, 0.0f, 0.0f ),
                new Vector3( 2.0f, 0.0f, 0.0f ),
                new Vector3( 3.0f, 0.0f, 0.0f ),
            };
        }

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(
                Bezier.GetPoint(points[0], points[1], points[2], t)
            );
        }

        public Vector3 GetVelocity(float t)
        {
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t)) -
                transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }
    }

    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            t = Mathf.Clamp01(t);
            float one_minus_t = 1.0f - t;
            return
                one_minus_t * one_minus_t * a +
                2.0f * one_minus_t * t * b +
                t * t * c;
        }

        public static Vector3 GetFirstDerivative(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return
                2f * (1f - t) * (b - a) +
                2f * t * (c - b);
        }
    }
}