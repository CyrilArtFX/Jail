using UnityEngine;

namespace Jail.Interactables.Triggers.Camera
{
    public sealed class CameraManager : MonoBehaviour
    {
        public static CameraArea CurrentArea { get; private set; }
        public static CameraArea PreviousArea { get; private set; }
        public static CameraManager instance;

        void Awake()
        {
            instance = this;
        }

        public static void SwitchArea(CameraArea area)
        {
            if (area == null)
            {
                Debug.LogError("CameraManager: can't SwitchArea, no camera was given! You probably only have one CameraArea in the scene.");
                return;
            }

            //  switch area
            PreviousArea = CurrentArea;
            CurrentArea = area;

            //  switch priority
            if (PreviousArea != null)
            {
                PreviousArea.Camera.Priority = 10;
            }
            CurrentArea.Camera.Priority = 11;
        }
    }
}