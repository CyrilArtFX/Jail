using UnityEngine;
using Cinemachine;


namespace Jail.Interactables.Triggers.Camera
{
    public class CameraArea : BaseTrigger
    {
        public CinemachineVirtualCamera Camera => cvCamera;

        [SerializeField]
        CinemachineVirtualCamera cvCamera;

        protected override void Awake()
        {
            base.Awake();

            color = Color.yellow;
        }

        protected override void OnTrigger(Collider other, bool is_enter)
        {
            if (is_enter)
            {
                CameraManager.SwitchArea(this);
            }
            else
            {
                if (this == CameraManager.CurrentArea)
                {
                    CameraManager.SwitchArea(CameraManager.PreviousArea);
                    print("CameraArea: detected coming back in previous area, switching back..");
                }
            }
        }
    }
}