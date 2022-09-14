using System.Collections;
using UnityEngine;

namespace Jail.Utility.Bezier
{
    public class BezierSplineFollower : MonoBehaviour
    {
        [SerializeField]
        BezierSpline spline;
        
        [SerializeField]
        Transform follower;
        [SerializeField]
        float moveSpeed = 10.0f;
        [SerializeField]
        Vector3 normalOffset = Vector3.up;
        //[SerializeField]
        //bool speedOnSlope = false;

        float dist = 0.0f;

        void Reset()
        {
            //  try to retrieve spline automatically
            TryGetComponent(out spline);
        }

        void Update()
        {
            float t = dist / spline.Length;
            Vector3 point = spline.GetPoint(t);
            Vector3 direction = spline.GetDirection(t);

            //  set position
            follower.position = point + Vector3.Cross(direction, normalOffset);

            //  set angles
            follower.LookAt(point + direction);

            //  move
            //float dot = Vector3.Dot();
            dist = (dist + moveSpeed * Time.deltaTime) % spline.Length;
        }
    }
}