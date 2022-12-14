using UnityEngine;

using Jail.Utility.Bezier;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretChainer : MonoBehaviour
    {
        public ZapTurretProjectile Projectile { get; set; }
        public Vector3 Target { get; set; }
        public BezierSplineChainer SplineChainer => splineChainer;

        [SerializeField]
        float smoothSpeed = 5.0f;
        [SerializeField]
        float tangentForce = 7.0f, idleTangentForce = 2.0f;
        
        [SerializeField]
        BezierSplineChainer splineChainer;

        void FixedUpdate()
        {
            if (Projectile.IsPulling && !Projectile.IsPaused) return;

            float tangent_force = Projectile.Target != null ? (Projectile.IsChasing ? tangentForce : idleTangentForce) : 0;
            BezierSpline spline = splineChainer.Spline;

            //  update last point
            Vector3 last_point_pos = spline.transform.InverseTransformPoint(Projectile.ChainerPoint.position);
            spline.SetControlPoint(spline.ControlPointCount - 1, last_point_pos);
            
            #region UpdateTangents
            Vector3 last_tangent_pos;

            if (Projectile.Target == null) 
            {
                last_tangent_pos = Vector3.zero;
            }
            else
            {
                Vector3 direction = (Target - Projectile.transform.position).normalized;
                last_tangent_pos = last_point_pos - spline.transform.InverseTransformDirection(direction) * tangent_force;
            }

            //  update first tangent
            Vector3 first_tangent_pos = Vector3.Lerp(spline.GetControlPoint(1), spline.GetControlPoint(0) + spline.transform.InverseTransformDirection(Projectile.transform.forward) * tangent_force, Time.fixedDeltaTime * smoothSpeed);
            spline.SetControlPoint(1, first_tangent_pos);

            //  update last tangent
            spline.SetControlPoint(spline.ControlPointCount - 2, Vector3.Lerp(spline.GetControlPoint(spline.ControlPointCount - 2), last_tangent_pos, Time.fixedDeltaTime * smoothSpeed));
            #endregion

        }
    }
}