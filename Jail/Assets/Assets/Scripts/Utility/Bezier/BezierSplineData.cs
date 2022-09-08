using System.Collections;
using UnityEngine;

namespace Jail.Utility.Bezier
{
    public struct BezierSplineData
    {
        [SerializeField]
        public Vector3[] points;
        [SerializeField]
        public BezierControlPointMode[] modes;
    }
}