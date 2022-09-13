using UnityEngine;
using Cinemachine;

namespace Jail.Utility.Camera
{
    [RequireComponent(typeof(BoxCollider))]
    public class CameraArea : MonoBehaviour
    {
        public CinemachineVirtualCamera Camera => cvCamera;
        public BoxCollider Collider { get; private set; }

        [SerializeField]
        LayerMask triggerMask;

        [SerializeField]
        CinemachineVirtualCamera cvCamera;

        void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!LayerMaskUtils.HasLayer(triggerMask, other.gameObject.layer)) return;

            CameraManager.SwitchArea(this);
        }

        void OnTriggerExit(Collider other)
        {
            if (this == CameraManager.CurrentArea)
            {
                CameraManager.SwitchArea(CameraManager.PreviousArea);
                print("CameraArea: detected coming back in previous area, switching back..");
            }
        }

        void OnDrawGizmos()
        {
            if (Collider == null)
            {
                Awake();
            }
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Collider.bounds.center, Collider.bounds.size);
        }
    }
}