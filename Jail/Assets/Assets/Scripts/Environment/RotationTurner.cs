using UnityEngine;

using Jail.Utility;

namespace Jail.Environment
{
    public class RotationTurner : MonoBehaviour
    {
        [SerializeField]
        SnapAxis axis;
        [SerializeField]
        float moveSpeed = 4.0f;

        void Update()
        {
            if (axis == SnapAxis.None) return;

            int layer = (int) axis;
            float speed = Time.deltaTime * moveSpeed;

            //  get rotation to apply
            Vector3 angles = Vector3.zero;
            if (LayerMaskUtils.HasFlag(layer, (int) SnapAxis.X))
            {
                angles.x += speed;
            }
            if (LayerMaskUtils.HasFlag(layer, (int)SnapAxis.Y))
            {
                angles.y += speed;
            }
            if (LayerMaskUtils.HasFlag(layer, (int)SnapAxis.Z))
            {
                angles.z += speed;
            }

            //  apply rotation
            transform.Rotate(angles, Space.Self);
        }
    }
}