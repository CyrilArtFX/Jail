using UnityEngine;

namespace Jail.Utility.Bezier
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            t = Mathf.Clamp01(t);
            float one_minus_t = 1.0f - t;
            return
                one_minus_t * one_minus_t * one_minus_t * a +
                3.0f * one_minus_t * one_minus_t * t * b +
                3.0f * one_minus_t * t * t * c +
                t * t * t * d;
        }

        public static Vector3 GetFirstDerivative(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            t = Mathf.Clamp01(t);
            float one_minus_t = 1.0f - t;
            return
                3.0f * one_minus_t * one_minus_t * (b - a) +
                6.0f * one_minus_t * t * (c - b) +
                3.0f * t * t * (d - c);
        }
    }
}