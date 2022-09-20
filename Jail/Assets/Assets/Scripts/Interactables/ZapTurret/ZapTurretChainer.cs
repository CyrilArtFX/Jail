using UnityEngine;

using Jail.Utility.Bezier;

namespace Jail.Interactables.ZapTurret
{
    public class ZapTurretChainer : MonoBehaviour
    {
        public ZapTurretProjectile Projectile { get; set; }
        public BezierSplineChainer SplineChainer => splineChainer;

        [SerializeField]
        float smoothSpeed = 5.0f;
        [SerializeField]
        float tangentForce = 7.0f, idleTangentForce = 2.0f;
        
        [SerializeField]
        BezierSplineChainer splineChainer;

        void FixedUpdate()
        {
            if (Projectile.IsPulling) return;

            float tangent_force = Projectile.IsChasing ? tangentForce : idleTangentForce;
            BezierSpline spline = splineChainer.Spline;

            //  update last point
            Vector3 last_point_pos = spline.transform.InverseTransformPoint(Projectile.transform.position);
            spline.SetControlPoint(spline.ControlPointCount - 1, last_point_pos);
            
            #region UpdateTangents
            Vector3 first_tangent_pos, last_tangent_pos;
            Vector3 direction;

            if (Projectile.Target == null) 
            {
                last_tangent_pos = Vector3.up;
            }
            else
			{
                direction = (Projectile.Target.position - Projectile.transform.position).normalized;
                last_tangent_pos = last_point_pos + direction * tangent_force;
			}

            //  update last tangent
            spline.SetControlPoint(spline.ControlPointCount - 2, Vector3.Lerp(spline.GetControlPoint(spline.ControlPointCount - 2), last_tangent_pos, Time.fixedDeltaTime * smoothSpeed));
            #endregion

            //  update tangents
            direction = Vector3.up /*(transform.position - Projectile.Target.position).normalized*/;
            first_tangent_pos = Vector3.Lerp(spline.GetControlPoint(1), spline.GetControlPoint(0) + direction * tangent_force, Time.fixedDeltaTime * smoothSpeed);
            spline.SetControlPoint(1, first_tangent_pos);
        }
    }
}