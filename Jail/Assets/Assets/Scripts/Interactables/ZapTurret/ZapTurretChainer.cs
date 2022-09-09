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
		float tangentForce = 7.0f;
		
		[SerializeField]
		BezierSplineChainer splineChainer;

        void FixedUpdate()
        {
			if (Projectile == null || Projectile.Target == null || Projectile.IsPulling) return;
			
			BezierSpline spline = splineChainer.Spline;
			Vector3 smooth_point;

			//  update last point
			Vector3 point = spline.transform.InverseTransformPoint(Projectile.transform.position);
			spline.SetControlPoint(spline.ControlPointCount - 1, point);

			//  update first tangent
            Vector3 direction = (transform.position - Projectile.Target.position).normalized;
			smooth_point = Vector3.Lerp(spline.GetControlPoint(1), spline.GetControlPoint(0) + direction * tangentForce, Time.fixedDeltaTime * smoothSpeed);
			spline.SetControlPoint(1, smooth_point);

            //  update last tangent
            direction = (Projectile.Target.position - Projectile.transform.position).normalized;
			smooth_point = Vector3.Lerp(spline.GetControlPoint(spline.ControlPointCount - 2), point + direction * tangentForce, Time.fixedDeltaTime * smoothSpeed);
			spline.SetControlPoint(spline.ControlPointCount - 2, smooth_point);
        }
    }
}