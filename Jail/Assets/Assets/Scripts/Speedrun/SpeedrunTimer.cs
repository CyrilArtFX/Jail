using TMPro;
using UnityEngine;

using Jail.Utility;

namespace Jail.Speedrun
{
    public class SpeedrunTimer : MonoBehaviour
    {
        public double CurrentTime { set; get; }

        [SerializeField]
        TMP_Text currentTimeTMP, lastTimeTMP;

        void Update()
        {
            CurrentTime += Time.unscaledDeltaTime;
            VariousUtility.SetTMPTime(currentTimeTMP, CurrentTime);
        }

        public void SplitTime()
        {
            VariousUtility.SetTMPTime(lastTimeTMP, CurrentTime);
            CurrentTime = 0.0d;
        }

        public void ResetTime()
        {
            VariousUtility.SetTMPTime(lastTimeTMP, 0.0d);
            CurrentTime = 0.0d;
        }
    }
}